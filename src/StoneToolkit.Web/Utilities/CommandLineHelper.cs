using System;
using System.IO;

namespace StoneToolkit.Web.Utilities
{
    /// <summary>
    /// 提供命令行参数解析的辅助方法。
    /// </summary>
    public static class CommandLineHelper
    {
        /// <summary>
        /// 从命令行中提取参数部分，忽略可执行程序路径。
        /// </summary>
        /// <param name="commandLine">完整的命令行字符串。</param>
        /// <param name="processPath">当前进程的完整路径。</param>
        /// <returns>提取的参数部分，如果无法提取则返回原命令行。</returns>
        public static string ExtractCommandLineArgs(string commandLine, string processPath)
        {
            if (string.IsNullOrEmpty(commandLine))
                return string.Empty;

            // 规范化进程路径（移除尾部斜杠和引号）
            processPath = processPath?.Trim('"').TrimEnd('\\', '/') ?? string.Empty;
            var processPathQuoted = $"\"{processPath}\"";

            // 处理带引号或不带引号的完整路径
            if (commandLine.StartsWith(processPathQuoted))
            {
                return commandLine.Substring(processPathQuoted.Length).Trim();
            }
            if (commandLine.StartsWith(processPath))
            {
                return commandLine.Substring(processPath.Length).Trim();
            }

            // 处理带引号或不带引号的文件名
            var processFileName = Path.GetFileName(processPath);
            var processFileNameQuoted = $"\"{processFileName}\"";
            if (commandLine.StartsWith(processFileNameQuoted))
            {
                return commandLine.Substring(processFileNameQuoted.Length).Trim();
            }
            if (commandLine.StartsWith(processFileName))
            {
                return commandLine.Substring(processFileName.Length).Trim();
            }

            // 处理带引号或不带引号的相对路径
            var relativePath = GetRelativePath(processPath);
            if (!string.IsNullOrEmpty(relativePath))
            {
                var relativePathQuoted = $"\"{relativePath}\"";
                if (commandLine.StartsWith(relativePathQuoted))
                {
                    return commandLine.Substring(relativePathQuoted.Length).Trim();
                }
                if (commandLine.StartsWith(relativePath))
                {
                    return commandLine.Substring(relativePath.Length).Trim();
                }
            }

            // 如果都不匹配，直接返回原命令行
            return commandLine;
        }

        /// <summary>
        /// 获取进程路径的相对路径形式（如 "OpenSilverHost\\OpenSilverHost.exe"）。
        /// </summary>
        private static string GetRelativePath(string fullPath)
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var relativePath = Path.GetRelativePath(currentDir, fullPath);
                return relativePath;
            }
            catch
            {
                return null;
            }
        }
    }
}