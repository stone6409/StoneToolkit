using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Management;
using System.Threading.Tasks;

namespace StoneToolkit.Web.Utilities
{
    public class ApplicationLifecycleManager
    {
        private readonly IHostApplicationLifetime _lifetime;

        // 定义一个委托，用于允许主程序实现 Cleanup 逻辑
        public delegate Task CleanupDelegate();

        // 定义一个事件，用于在应用程序停止时触发 Cleanup
        public event CleanupDelegate OnCleanup;

        public ApplicationLifecycleManager(IHostApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        // 注册应用程序生命周期事件
        public void RegisterApplicationLifecycleEvents()
        {
            // 注册应用程序停止事件
            _lifetime.ApplicationStopping.Register(async () =>
            {
                Console.WriteLine("应用程序正在停止，执行清理操作...");
                await Cleanup(); // 执行清理逻辑
            });

            // 注册应用程序已停止事件
            _lifetime.ApplicationStopped.Register(() =>
            {
                Console.WriteLine("应用程序已停止。");
            });

            // 启动跨进程通信监听
            StartInterProcessCommunication();
        }

        // 自定义清理逻辑，触发 OnCleanup 事件
        private async Task Cleanup()
        {
            if (OnCleanup != null)
            {
                await OnCleanup.Invoke(); // 触发主程序实现的 Cleanup 逻辑
            }
            else
            {
                Console.WriteLine("未注册清理逻辑。");
            }
        }

        /// <summary>
        /// 生成唯一的管道名称，基于应用程序名称和命令行参数。
        /// </summary>
        /// <param name="appName">应用程序名称。</param>
        /// <param name="commandLine">命令行参数。</param>
        /// <param name="processPath">进程的完整路径。</param>
        /// <returns>生成的管道名称。</returns>
        private static string GeneratePipeName(string appName, string commandLine, string processPath)
        {
            var pipeName = $"Pipe_{appName}";
            if (!string.IsNullOrEmpty(commandLine))
            {
                var args = CommandLineHelper.ExtractCommandLineArgs(commandLine, processPath);
                if (!string.IsNullOrEmpty(args))
                {
                    pipeName += $"_{args}";
                }
            }
            return pipeName;
        }

        /// <summary>
        /// 启动进程间通信，等待主进程发送关闭信号。
        /// </summary>
        private void StartInterProcessCommunication()
        {
            Task.Run(() =>
            {
                var appName = AppDomain.CurrentDomain.FriendlyName;
                var process = Process.GetCurrentProcess();
                var commandLine = GetProcessCommandLine(process);
                var processPath = process.MainModule.FileName;

                // 生成管道名称
                var pipeName = GeneratePipeName(appName, commandLine, processPath);
                Console.WriteLine($"命令行: {commandLine}...");
                Console.WriteLine($"等待主进程发送关闭信号，管道名称: {pipeName}...");

                // 创建命名管道并等待连接
                var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);
                pipeServer.WaitForConnection();

                using (var reader = new StreamReader(pipeServer))
                {
                    var signal = reader.ReadLine();
                    if (signal == "SHUTDOWN")
                    {
                        Console.WriteLine("接收到关闭信号，准备关闭应用程序...");
                        _lifetime.StopApplication();
                    }
                }
            });
        }

        // 获取进程的命令行参数
        private static string GetProcessCommandLine(Process process)
        {
            try
            {
                // 使用 WMI 查询进程的命令行
                using (var searcher = new ManagementObjectSearcher(
                    $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["CommandLine"]?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取进程命令行失败: {ex.Message}");
            }
            return null;
        }
    }
}