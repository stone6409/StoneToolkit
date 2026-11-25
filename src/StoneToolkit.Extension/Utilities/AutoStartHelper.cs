using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace StoneToolkit.Extension.Utilities
{
    public static class AutoStartUtility
    {
        private const string CurrentUserRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run"; // 普通用户注册表路径
        private const string LocalMachineRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run"; // 管理员注册表路径
        private static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name; // 应用程序名称
        private static readonly string AppPath = GetExecutablePath(); // 应用程序路径

        // 获取当前应用程序的可执行文件路径
        private static string GetExecutablePath()
        {
            string processPath = Process.GetCurrentProcess().MainModule.FileName;
            return $"\"{processPath}\"";
        }

        // 检查当前是否以管理员权限运行
        private static bool IsRunningAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // 设置自动启动（支持普通用户和管理员两种方式）
        public static void SetAutoStart(bool isEnabled, bool requireAdmin = false)
        {
            try
            {
                if (requireAdmin && !IsRunningAsAdministrator())
                {
                    Console.WriteLine("需要管理员权限才能设置全局自动启动。");
                    return;
                }

                // 根据权限选择注册表路径
                string registryPath = requireAdmin ? LocalMachineRegistryPath : CurrentUserRegistryPath;
                RegistryKey registryKey = requireAdmin
                    ? Registry.LocalMachine.OpenSubKey(registryPath, true)
                    : Registry.CurrentUser.OpenSubKey(registryPath, true);

                if (registryKey == null)
                {
                    Console.WriteLine("无法访问注册表项，请检查权限。");
                    return;
                }

                if (isEnabled)
                {
                    if (registryKey.GetValue(AppName) == null)
                    {
                        registryKey.SetValue(AppName, AppPath);
                        Console.WriteLine($"应用程序已成功添加到{(requireAdmin ? "全局" : "当前用户")}启动项！");
                    }
                    else
                    {
                        Console.WriteLine($"应用程序已经存在于{(requireAdmin ? "全局" : "当前用户")}启动项中。");
                    }
                }
                else
                {
                    if (registryKey.GetValue(AppName) != null)
                    {
                        registryKey.DeleteValue(AppName);
                        Console.WriteLine($"应用程序已从{(requireAdmin ? "全局" : "当前用户")}启动项中删除。");
                    }
                    else
                    {
                        Console.WriteLine($"应用程序未在{(requireAdmin ? "全局" : "当前用户")}启动项中。");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置自动启动失败: {ex.Message}");
            }
        }

        // 检查自动启动是否已启用（支持普通用户和管理员两种方式）
        public static bool IsAutoStartEnabled(bool checkAdmin = false)
        {
            try
            {
                string registryPath = checkAdmin ? LocalMachineRegistryPath : CurrentUserRegistryPath;
                RegistryKey registryKey = checkAdmin
                    ? Registry.LocalMachine.OpenSubKey(registryPath, false)
                    : Registry.CurrentUser.OpenSubKey(registryPath, false);

                if (registryKey == null)
                {
                    Console.WriteLine("无法访问注册表项，请检查权限。");
                    return false;
                }

                return registryKey.GetValue(AppName) != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查自动启动失败: {ex.Message}");
                return false;
            }
        }
    }
}