using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace StoneToolkit.Common.Utilities
{
    public static class ProcessUtility
    {
        // Windows API 函数
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOWMINIMIZED = 2; // 最小化窗口
        private const int SW_HIDE = 0; // 隐藏窗口

        public static bool IsProcessRunningByPath(string executeFile)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                try
                {
                    // 获取进程的可执行文件路径
                    if (process.MainModule?.FileName == executeFile)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // 忽略无权限访问的进程
                }
            }
            return false;
        }

        public static bool IsProcessRunningByPath2(string executeFile)
        {
            string processName = Path.GetFileNameWithoutExtension(executeFile);
            var processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                try
                {
                    if (process.MainModule?.FileName == executeFile)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // 忽略无权限访问的进程
                }
            }

            return false;
        }

        public static Process RunExecuteFile(string executeFile, string args = null, bool visible = false)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = executeFile;
            processStartInfo.Arguments = args;

            // 获取 executeFile 所在的文件夹路径，并设置为当前工作目录
            string workingDirectory = Path.GetDirectoryName(executeFile);
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }

            if (!visible)
            {
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.CreateNoWindow = true;
            }

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();

            return process;
        }

        public static Process RunExecuteFileWithoutFocus(string executeFile, string args = null)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = executeFile,
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Minimized, // 最小化窗口
                UseShellExecute = true
            };

            var process = Process.Start(processStartInfo);

            // 使用 Windows API 进一步隐藏窗口
            if (process != null && process.MainWindowHandle != IntPtr.Zero)
            {
                //ShowWindow(process.MainWindowHandle, SW_HIDE); // 隐藏窗口
                SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle); // 将焦点返回到当前进程
            }

            return process;
        }

        public static void RunBatchFile(string batFile, string args = null)
        {
            Process process = new Process();
            process.StartInfo.FileName = batFile;
            process.StartInfo.UseShellExecute = true;

            //这里相当于传参数 
            process.StartInfo.Arguments = args;
            process.Start();

            //测试同步执行 
            process.WaitForExit();
        }
        
        public static void RunBatchFile2(string batFile)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{batFile}\"", // 使用 /c 参数执行批处理文件并自动关闭
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }
    }
}
