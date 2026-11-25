using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace StoneToolkit.Extension.Utilities
{
    public static class AutoStartUtility2
    {
        private static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name;
        private static readonly string AppPath = Process.GetCurrentProcess().MainModule.FileName;

        public static void SetAutoStartWithAdmin(bool isEnabled)
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    string taskName = $"{AppName}_AutoStart";

                    if (isEnabled)
                    {
                        // 创建任务定义
                        TaskDefinition td = ts.NewTask();
                        td.Principal.RunLevel = TaskRunLevel.Highest;
                        td.RegistrationInfo.Description = $"自动启动 {AppName}";
                        td.Triggers.Add(new LogonTrigger());
                        td.Actions.Add(new ExecAction(AppPath, null, Path.GetDirectoryName(AppPath)));

                        // 设置任务为最高权限运行
                        td.Principal.RunLevel = TaskRunLevel.Highest;

                        // 注册任务
                        ts.RootFolder.RegisterTaskDefinition(taskName, td);
                        Console.WriteLine("已创建管理员权限的自动启动任务");
                    }
                    else
                    {
                        // 删除任务
                        ts.RootFolder.DeleteTask(taskName, false);
                        Console.WriteLine("已删除自动启动任务");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置自动启动失败: {ex.Message}");
            }
        }

        public static bool IsAutoStartWithAdminEnabled()
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    string taskName = $"{AppName}_AutoStart";
                    return ts.GetTask(taskName) != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
