using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace StoneToolkit.Extension.Utilities
{
    /// <summary>
    /// VSCode打开工具类
    /// </summary>
    public static class VSCodeUtility
    {
        private static string _vsCodeExecutable;

        public static string VSCodeExecutable
        {
            get
            {
                if (_vsCodeExecutable == null)
                {
                    _vsCodeExecutable = FindVSCodeExecutable();
                }
                return _vsCodeExecutable;
            }
        }

        /// <summary>
        /// 自动查找VSCode可执行文件路径
        /// </summary>
        private static string FindVSCodeExecutable()
        {
            // 1. 检查PATH环境变量
            if (CheckCommandInPath("code"))
            {
                return "code";
            }

            // 2. 检查常见安装路径
            var paths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft VS Code", "Code.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Microsoft VS Code", "Code.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft VS Code", "Code.exe"),
                @"C:\Program Files\Microsoft VS Code\Code.exe"
            };

            // 3. 从注册表查找
            var registryPaths = FindVSCodeFromRegistry();
            var allPaths = paths.Concat(registryPaths);

            foreach (var path in allPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return "code";
        }

        private static bool CheckCommandInPath(string command)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = "--version",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    if (process != null && process.WaitForExit(2000) && process.ExitCode == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static List<string> FindVSCodeFromRegistry()
        {
            var paths = new List<string>();
            var registryKeys = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{771FD6B0-FA20-440A-A002-3B3BAC16DC50}_is1",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{771FD6B0-FA20-440A-A002-3B3BAC16DC50}_is1"
            };

            foreach (var keyPath in registryKeys)
            {
                try
                {
                    var key = Registry.LocalMachine.OpenSubKey(keyPath);
                    if (key == null)
                    {
                        key = Registry.CurrentUser.OpenSubKey(keyPath);
                    }

                    if (key != null)
                    {
                        var installLocation = key.GetValue("InstallLocation") as string;
                        if (!string.IsNullOrEmpty(installLocation))
                        {
                            var exePath = Path.Combine(installLocation, "Code.exe");
                            if (File.Exists(exePath))
                            {
                                paths.Add(exePath);
                            }
                        }
                        key.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"注册表查找失败: {ex.Message}");
                }
            }

            return paths;
        }

        public static bool IsVSCodeInstalled()
        {
            try
            {
                if (VSCodeExecutable == "code")
                {
                    return CheckCommandInPath("code");
                }
                else
                {
                    return File.Exists(VSCodeExecutable);
                }
            }
            catch
            {
                return false;
            }
        }

        public static void SetVSCodePath(string customPath)
        {
            if (File.Exists(customPath))
            {
                _vsCodeExecutable = customPath;
            }
            else
            {
                throw new FileNotFoundException($"VSCode路径不存在: {customPath}");
            }
        }

        public static void ResetVSCodePath()
        {
            _vsCodeExecutable = null;
        }

        /// <summary>
        /// 使用VSCode打开文件夹
        /// </summary>
        public static async Task<OpenResult> OpenFolderWithVSCodeAsync(string folderPath, VSCodeOpenOptions options = null)
        {
            return await OpenWithVSCodeAsync(new[] { folderPath }, options);
        }

        /// <summary>
        /// 使用VSCode打开文件
        /// </summary>
        public static async Task<OpenResult> OpenFileWithVSCodeAsync(string filePath, VSCodeOpenOptions options = null)
        {
            return await OpenWithVSCodeAsync(new[] { filePath }, options);
        }

        /// <summary>
        /// 使用VSCode打开文件夹和文件
        /// </summary>
        public static async Task<OpenResult> OpenFolderWithFilesAsync(string folderPath, IEnumerable<string> filePaths = null, VSCodeOpenOptions options = null)
        {
            var paths = new List<string> { folderPath };
            if (filePaths != null)
            {
                paths.AddRange(filePaths.Where(File.Exists));
            }
            return await OpenWithVSCodeAsync(paths, options);
        }

        /// <summary>
        /// 统一的打开方法
        /// </summary>
        private static async Task<OpenResult> OpenWithVSCodeAsync(IEnumerable<string> paths, VSCodeOpenOptions options)
        {
            if (options == null)
            {
                options = VSCodeOpenOptions.Default;
            }

            var result = new OpenResult();

            try
            {
                var validPaths = paths.Where(p => Directory.Exists(p) || File.Exists(p)).ToList();
                if (!validPaths.Any())
                {
                    result.Success = false;
                    result.Message = "路径不存在或为空";
                    return result;
                }

                if (!IsVSCodeInstalled())
                {
                    result.Success = false;
                    result.Message = "未检测到VSCode安装";
                    return result;
                }

                var arguments = BuildArguments(validPaths, options);
                var workingDir = validPaths.FirstOrDefault(p => Directory.Exists(p));
                if (workingDir == null)
                {
                    workingDir = Path.GetDirectoryName(validPaths.First());
                }

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = VSCodeExecutable,
                        Arguments = arguments,
                        UseShellExecute = options.RunAsAdministrator,
                        CreateNoWindow = true,
                        WorkingDirectory = workingDir
                    };

                    if (options.RunAsAdministrator)
                    {
                        process.StartInfo.Verb = "runas";
                    }
                    else
                    {
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                    }

                    process.Start();

                    if (!options.RunAsAdministrator)
                    {
                        var outputTask = process.StandardOutput.ReadToEndAsync();
                        var errorTask = process.StandardError.ReadToEndAsync();
                        await Task.WhenAll(outputTask, errorTask);
                    }

                    var exited = process.WaitForExit(options.TimeoutMilliseconds);
                    result.Success = true;
                    result.Message = $"VSCode已启动，打开 {validPaths.Count} 个路径" + 
                                    (options.SkipAddToRecentlyOpened ? " (不保存到最近记录)" : "");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"发生错误: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 构建命令行参数
        /// </summary>
        private static string BuildArguments(IEnumerable<string> paths, VSCodeOpenOptions options)
        {
            var arguments = string.Join(" ", paths.Select(p => $"\"{p}\""));

            if (options.OpenInNewWindow) arguments += " --new-window";
            if (options.ReuseWindow) arguments += " --reuse-window";
            if (options.SkipAddToRecentlyOpened) arguments += " --skip-add-to-recently-opened";
            if (options.DisableExtensions) arguments += " --disable-extensions";
            if (options.DisableGpu) arguments += " --disable-gpu";

            // 新增：禁用工作区信任检查（独立于临时用户数据）
            if (options.DisableWorkspaceTrust)
            {
                arguments += " --disable-workspace-trust";
            }

            if (options.DisableWelcomePage) arguments += " --disable-welcome-page";

            // 使用临时用户数据目录
            if (options.UseTemporaryUserData)
            {
                var tempUserData = Path.Combine(Path.GetTempPath(), "vscode_utility_temp");

                // 确保目录存在
                if (!Directory.Exists(tempUserData))
                {
                    Directory.CreateDirectory(tempUserData);
                }

                arguments += $" --user-data-dir \"{tempUserData}\"";
            }

            if (options.GotoLine.HasValue)
            {
                arguments += $" --goto {options.GotoLine.Value}";
                if (options.GotoColumn.HasValue) arguments += $":{options.GotoColumn.Value}";
            }

            if (!string.IsNullOrEmpty(options.CustomArguments))
            {
                arguments += $" {options.CustomArguments}";
            }

            return arguments;
        }

        /// <summary>
        /// 兼容旧方法 - 优先使用VSCode打开文件；若VSCode未安装，则用资源管理器打开文件夹
        /// </summary>
        public static async Task OpenWithVSCodeAsync(string folderPath, string filePath)
        {
            try
            {
                // 如果VSCode未安装，回退到文件资源管理器打开文件夹
                if (!IsVSCodeInstalled())
                {
                    Process.Start("explorer.exe", "/select, " + filePath);
                    return;
                }

                var options = new VSCodeOpenOptions
                {
                    SkipAddToRecentlyOpened = true,
                    DisableWorkspaceTrust = true,  // 禁用信任提示
                    UseTemporaryUserData = true,  // 这会创建完全隔离的VSCode实例
                    DisableWelcomePage = true,
                };

                var result = await OpenFolderWithFilesAsync(folderPath, new[] { filePath }, options);

                if (!result.Success)
                {
                    // VSCode启动失败时也回退到资源管理器
                    Process.Start("explorer.exe", "/select, " + filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开文件失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}