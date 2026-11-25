using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StoneToolkit.Common.ConsoleUI
{
    /// <summary>
    /// 控制台封装：输出走原生 <see cref="Console.WriteLine(string)"/>，保持与普通控制台一致的
    /// 回滚缓冲区（支持鼠标滚轮 / 滚动条 / PageUp 上翻查看历史）。
    /// 输入为内联式（提示符打印在当前光标处、逐字符回显），并保留“未输入时按 P 触发热键”的能力。
    /// 当输出被重定向（管道、文件）时自动退化为普通控制台（<see cref="ScreenEnabled"/> 为 false）。
    /// </summary>
    /// <remarks>
    /// 输入进行中到达的 <see cref="WriteLine"/>（多来自后台线程的系统消息 / 推送）会先暂存，
    /// 待本次输入回车后再按序刷出，避免撕裂当前输入行。
    /// </remarks>
    public sealed class ConsoleScreen : IDisposable
    {
        private readonly object _consoleLock = new object();

        private readonly bool _screenReady;      // 是否可用原生控制台（输出被重定向时为 false）
        private bool _inputActive;               // 是否正在读取输入
        private readonly List<string> _pendingOutput = new List<string>(); // 输入期间暂存的输出

        public ConsoleScreen()
        {
            _screenReady = !Console.IsOutputRedirected;
        }

        /// <summary>是否启用原生控制台输出（输出被重定向时为 false）。</summary>
        public bool ScreenEnabled
        {
            get { return _screenReady; }
        }

        /// <summary>
        /// 读取一行输入。onHotkey 非空时，在尚未输入任何内容时按 P 会触发该回调（用于切换推送显示）。
        /// 返回 null 表示输入流已结束（EOF）。
        /// </summary>
        public string ReadInput(string prompt, Action onHotkey = null)
        {
            // 输出/输入被重定向（管道、文件）时无法逐键交互，退化为普通读取，保证脚本可用。
            if (!_screenReady || Console.IsInputRedirected)
            {
                lock (_consoleLock)
                {
                    FlushPendingLocked();
                }
                Console.Write(prompt);
                return Console.ReadLine();
            }

            lock (_consoleLock)
            {
                _inputActive = true;
                Console.Write(prompt);
            }

            var buffer = new StringBuilder();
            try
            {
                while (true)
                {
                    if (!Console.KeyAvailable)
                    {
                        Thread.Sleep(20);
                        continue;
                    }

                    var key = Console.ReadKey(intercept: true);

                    // 仅在尚未输入内容时响应热键，避免与股票代码中的字母冲突。
                    if (onHotkey != null && key.Key == ConsoleKey.P && buffer.Length == 0)
                    {
                        onHotkey();
                        continue;
                    }

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            lock (_consoleLock)
                            {
                                _inputActive = false;
                                Console.WriteLine();
                                var entered = buffer.ToString();
                                FlushPendingLocked();
                                return entered;
                            }
                        case ConsoleKey.Backspace:
                            if (buffer.Length > 0)
                            {
                                buffer.Length--;
                                Console.Write("\b \b");
                            }
                            break;
                        default:
                            if (!char.IsControl(key.KeyChar))
                            {
                                buffer.Append(key.KeyChar);
                                Console.Write(key.KeyChar);
                            }
                            break;
                    }
                }
            }
            catch
            {
                // 输入流异常/中断：刷新暂存输出后按 EOF 处理。
                lock (_consoleLock)
                {
                    _inputActive = false;
                    FlushPendingLocked();
                }
                return null;
            }
            finally
            {
                lock (_consoleLock)
                {
                    _inputActive = false;
                }
            }
        }

        /// <summary>
        /// 输出一行。缓冲区自然增长，支持滚动查看历史。
        /// 输入进行中时先暂存，回车后统一刷出，避免打断当前输入行。
        /// </summary>
        public void WriteLine(string text)
        {
            lock (_consoleLock)
            {
                if (!_screenReady)
                {
                    Console.WriteLine(text);
                    return;
                }

                if (_inputActive)
                {
                    _pendingOutput.Add(text);
                    return;
                }

                WriteLinesLocked(text);
            }
        }

        public void Dispose()
        {
            if (!_screenReady)
                return;

            try
            {
                Console.CursorVisible = true;
            }
            catch
            {
            }
        }

        /// <summary>将暂存的输出按序刷出（调用方需持有 <see cref="_consoleLock"/>）。</summary>
        private void FlushPendingLocked()
        {
            if (_pendingOutput.Count == 0)
                return;

            var pending = _pendingOutput.ToArray();
            _pendingOutput.Clear();
            foreach (var text in pending)
                WriteLinesLocked(text);
        }

        /// <summary>按 '\n' 拆分并逐行写出（调用方需持有 <see cref="_consoleLock"/>）。</summary>
        private void WriteLinesLocked(string text)
        {
            if (text == null)
            {
                Console.WriteLine();
                return;
            }

            foreach (var raw in text.Split('\n'))
                Console.WriteLine(raw.TrimEnd('\r'));
        }
    }
}
