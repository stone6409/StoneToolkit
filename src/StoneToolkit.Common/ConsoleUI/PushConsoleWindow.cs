using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading;

namespace StoneToolkit.Common.ConsoleUI
{
    /// <summary>
    /// 将推送信息转发到独立的控制台窗口显示。
    /// 同一进程无法同时拥有两个控制台窗口，故采用「当前进程自拉起一个查看器子进程 + 命名管道传输」的方式：
    /// 主进程作为命名管道服务端写出文本，查看器子进程作为客户端逐行打印，天然具备独立滚动缓冲。
    /// 查看器窗口惰性启动：仅在首次收到可触发弹出的推送数据时才拉起，无推送数据时不弹窗。
    /// 输出被重定向（管道/CI）时不会启用；启用失败时可通过 <see cref="Fallback"/> 降级到主窗口输出。
    /// </summary>
    public sealed class PushConsoleWindow : IDisposable
    {
        private const string ViewerSwitch = "--push-viewer";
        private const int FlushIntervalMs = 50;    // 写出节奏：合并高频推送，避免刷屏与阻塞推送线程
        private const int ConnectTimeoutMs = 5000; // 等待查看器连接的超时

        private readonly string _title;
        private readonly ConcurrentQueue<PushItem> _queue = new ConcurrentQueue<PushItem>();

        private NamedPipeServerStream _pipe;
        private StreamWriter _writer;
        private Process _viewer;
        private Thread _writerThread;
        private volatile bool _started;   // 已登记启用：Write 入队而非降级
        private volatile bool _connected; // 是否已连通并可投递
        private volatile bool _running;   // 写线程是否继续消费

        public PushConsoleWindow(string title = "推送")
        {
            _title = title ?? "推送";
        }

        /// <summary>是否已成功建立独立推送窗口。为 false 时调用方应自行降级。</summary>
        public bool Enabled
        {
            get { return _connected; }
        }

        /// <summary>未启用（输出重定向 / 启动失败）时的降级回调，通常指向主窗口的 WriteLine。</summary>
        public Action<string> Fallback { get; set; }

        /// <summary>
        /// 登记启用推送窗口（不拉起进程）。查看器子进程在首次收到可触发弹出的推送数据时才惰性启动。
        /// 输出重定向时静默降级（<see cref="Enabled"/> 保持 false）。
        /// </summary>
        public void Start()
        {
            if (Console.IsOutputRedirected)
                return;

            _started = true;
            _running = true;

            _writerThread = new Thread(WriterLoop)
            {
                IsBackground = true,
                Name = "PushConsoleWindow.Writer",
            };
            _writerThread.Start();
        }

        /// <summary>追加单行推送（可触发窗口弹出）。窗口不可用时走降级回调。</summary>
        public void Write(string line)
        {
            Write(line, autoOpen: true);
        }

        /// <summary>
        /// 追加单行推送。<paramref name="autoOpen"/> 为 true 时该行可触发窗口弹出；
        /// 为 false 时仅在窗口已弹出后显示（窗口未弹出则丢弃），适用于心跳等保活帧。
        /// </summary>
        public void Write(string line, bool autoOpen)
        {
            if (line == null)
                return;

            if (_started)
            {
                _queue.Enqueue(new PushItem(line, autoOpen));
                return;
            }

            var fallback = Fallback;
            if (fallback != null)
                fallback(line);
        }

        /// <summary>追加多行推送（默认可触发窗口弹出）。</summary>
        public void Write(IEnumerable<string> lines, bool autoOpen = true)
        {
            if (lines == null)
                return;

            foreach (var line in lines)
                Write(line, autoOpen);
        }

        public void Dispose()
        {
            _connected = false;
            _started = false;
            _running = false;

            if (_writerThread != null)
            {
                try { _writerThread.Join(1000); } catch { }
                _writerThread = null;
            }

            Cleanup();

            if (_viewer != null)
            {
                try
                {
                    if (!_viewer.HasExited && !_viewer.WaitForExit(1000))
                        _viewer.Kill();
                }
                catch { }

                try { _viewer.Dispose(); } catch { }
                _viewer = null;
            }
        }

        /// <summary>
        /// 查看器模式入口：命令行命中查看器开关时阻塞运行并返回 true；否则返回 false。
        /// 各 App 应在 Main 最前面调用，命中后直接 return，不执行正常初始化。
        /// </summary>
        public static bool TryRunViewer(string[] args)
        {
            if (args == null || args.Length == 0 || !string.Equals(args[0], ViewerSwitch, StringComparison.Ordinal))
                return false;

            string pipeName = args.Length > 1 ? args[1] : string.Empty;
            string title = args.Length > 2 ? args[2] : "推送";

            try { Console.Title = title; } catch { }
            Console.WriteLine("=== " + title + " ===");

            try
            {
                using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.In))
                {
                    client.Connect(ConnectTimeoutMs);
                    using (var reader = new StreamReader(client, new UTF8Encoding(false)))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                            Console.WriteLine(line);
                    }
                }
            }
            catch
            {
                // 连接失败或管道中断：查看器直接退出
            }

            return true;
        }

        // ===================== 内部实现 =====================

        private void WriterLoop()
        {
            while (_running)
            {
                if (!_connected)
                {
                    // 未建窗口：丢弃排在触发项之前的非触发项（如心跳），直到遇到触发项才拉起窗口。
                    if (!WaitForTriggerOrEmpty())
                    {
                        Thread.Sleep(FlushIntervalMs);
                        continue;
                    }

                    if (!TryLaunchViewer())
                    {
                        // 启动失败：永久降级，已入队数据不丢失。
                        _started = false;
                        DrainAllToFallback();
                        return;
                    }
                }

                bool wrote = false;
                PushItem item;
                while (_queue.TryDequeue(out item))
                {
                    try
                    {
                        _writer.WriteLine(item.Line);
                        wrote = true;
                    }
                    catch
                    {
                        _connected = false;
                        _started = false;
                        DrainAllToFallback();
                        return;
                    }
                }

                if (wrote)
                {
                    try
                    {
                        _writer.Flush();
                    }
                    catch
                    {
                        _connected = false;
                        _started = false;
                        DrainAllToFallback();
                        return;
                    }
                }

                if (!_running && _queue.IsEmpty)
                    return;

                Thread.Sleep(FlushIntervalMs);
            }
        }

        /// <summary>
        /// 队首为触发项时返回 true；否则丢弃队首的非触发项后继续；队列为空返回 false。
        /// </summary>
        private bool WaitForTriggerOrEmpty()
        {
            PushItem item;
            while (_queue.TryPeek(out item))
            {
                if (item.AutoOpen)
                    return true;

                if (!_queue.TryDequeue(out _))
                    return false;
            }

            return false;
        }

        /// <summary>把剩余队列逐条降级到主窗口输出。</summary>
        private void DrainAllToFallback()
        {
            var fallback = Fallback;
            PushItem item;
            while (_queue.TryDequeue(out item))
            {
                if (fallback != null)
                    fallback(item.Line);
            }
        }

        /// <summary>
        /// 建立命名管道并拉起查看器子进程。失败时静默降级（<see cref="Enabled"/> 保持 false）。
        /// </summary>
        private bool TryLaunchViewer()
        {
            try
            {
                string pipeName = "StoneToolkit.PushViewer." + GetCurrentProcessId() + "." + Guid.NewGuid().ToString("N");

                _pipe = new NamedPipeServerStream(
                    pipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                ProcessStartInfo psi;
                if (!TryBuildViewerStartInfo(pipeName, out psi))
                {
                    Cleanup();
                    return false;
                }

                _viewer = Process.Start(psi);

                if (!WaitForConnection())
                {
                    Cleanup();
                    return false;
                }

                _writer = new StreamWriter(_pipe, new UTF8Encoding(false)) { AutoFlush = false };
                _connected = true;
                return true;
            }
            catch
            {
                Cleanup();
                return false;
            }
        }

        private bool WaitForConnection()
        {
            try
            {
                var task = _pipe.WaitForConnectionAsync();
                return task.Wait(ConnectTimeoutMs) && _pipe.IsConnected;
            }
            catch
            {
                return false;
            }
        }

        private bool TryBuildViewerStartInfo(string pipeName, out ProcessStartInfo psi)
        {
            psi = null;

            string exe = GetCurrentExecutablePath();
            if (string.IsNullOrEmpty(exe))
                return false;

            string arguments = ViewerSwitch + " \"" + pipeName + "\" \"" + _title + "\"";
            string fileName = exe;

            // 以 dotnet 主机启动（部分 dotnet run 场景）时，需显式带上入口程序集
            if (string.Equals(Path.GetFileNameWithoutExtension(exe), "dotnet", StringComparison.OrdinalIgnoreCase))
            {
                string entry = null;
                try { entry = Assembly.GetEntryAssembly() != null ? Assembly.GetEntryAssembly().Location : null; } catch { }
                if (string.IsNullOrEmpty(entry))
                    return false;

                arguments = "\"" + entry + "\" " + arguments;
            }

            psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = true,             // 关键：让子进程获得独立的新控制台窗口
                WindowStyle = ProcessWindowStyle.Normal,
            };
            return true;
        }

        private void Cleanup()
        {
            _running = false;

            if (_writer != null)
            {
                try { _writer.Flush(); } catch { }
                try { _writer.Dispose(); } catch { }
                _writer = null;
            }

            if (_pipe != null)
            {
                try { _pipe.Dispose(); } catch { }
                _pipe = null;
            }
        }

        private static string GetCurrentExecutablePath()
        {
            try
            {
                using (var proc = Process.GetCurrentProcess())
                {
                    var module = proc.MainModule;
                    return module != null ? module.FileName : null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static int GetCurrentProcessId()
        {
            try
            {
                using (var proc = Process.GetCurrentProcess())
                    return proc.Id;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>入队条目：文本 + 是否可触发窗口弹出。</summary>
        private readonly struct PushItem
        {
            public readonly string Line;
            public readonly bool AutoOpen;

            public PushItem(string line, bool autoOpen)
            {
                Line = line;
                AutoOpen = autoOpen;
            }
        }
    }
}
