using System;
using System.IO;
using System.Reflection;
using StoneToolkit.Common.Logging;

namespace StoneToolkit.WpfUserControl.AboutDialog
{
    public class AboutViewModel
    {
        public string AppName { get; }
        public string Version { get; }
        public string Copyright { get; }
        public string Company { get; }
        public string Description { get; }
        public string BuildTime { get; }
        public bool ShowUpdateButton { get; }
        public string LogsFolderPath { get; }

        public AboutViewModel()
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyName = assembly.GetName();

            // 应用程序名称
            AppName = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
                     ?? assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product
                     ?? assemblyName.Name;

            // 版本信息
            var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            Version = !string.IsNullOrEmpty(informationalVersion)
                ? $"版本: {informationalVersion}"
                : $"版本: {assemblyName.Version}";

            // 版权信息
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright
                       ?? $"Copyright © {DateTime.Now.Year}";

            // 公司信息
            Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company
                     ?? "Your Company";

            // 描述信息
            Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description
                         ?? "这是一个WPF应用程序";

            // 构建时间
            BuildTime = GetLinkerTime(assembly).ToString("yyyy-MM-dd HH:mm:ss");

            // 是否显示更新按钮（可根据需要调整逻辑）
            ShowUpdateButton = false;

            // 获取日志文件夹路径
            LogsFolderPath = GetLogsFolderPath();
        }

        // 获取程序集链接时间（构建时间）
        private static DateTime GetLinkerTime(Assembly assembly)
        {
            var location = assembly.Location;
            if (string.IsNullOrEmpty(location))
                return DateTime.MinValue;

            return File.GetLastWriteTime(location);
        }

        // 获取日志文件夹路径
        private static string GetLogsFolderPath()
        {
            try
            {
                // 使用 SystemLogger 的 LogsFolder 属性获取日志文件夹路径
                return SystemLogger.LogsFolder;
            }
            catch (Exception ex)
            {
                // 如果获取失败，记录错误并返回默认路径
                SystemLogger.Error($"获取日志文件夹路径失败: {ex.Message}");
                return string.Empty;
            }
        }
    }
}