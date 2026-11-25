using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    /// <summary>
    /// 文件夹名称过滤器
    /// </summary>
    public class FolderNameFilter : IFolderFilter
    {
        private readonly HashSet<string> _excludedFolderNames;

        /// <summary>
        /// 创建排除指定文件夹名称的过滤器
        /// </summary>
        /// <param name="excludedFolderNames">要排除的文件夹名称列表</param>
        public FolderNameFilter(IEnumerable<string> excludedFolderNames)
        {
            _excludedFolderNames = new HashSet<string>(
                excludedFolderNames?.Select(name => name?.Trim() ?? string.Empty)
                ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 创建排除多个文件夹名称的过滤器
        /// </summary>
        /// <param name="excludedFolderNames">要排除的文件夹名称字符串，支持逗号、分号分隔</param>
        public FolderNameFilter(string excludedFolderNames)
            : this(ParseFolderNames(excludedFolderNames))
        {
        }

        /// <summary>
        /// 解析文件夹名称字符串，支持逗号、分号分隔的多个文件夹名称
        /// </summary>
        /// <param name="folderNames">文件夹名称字符串，例如："bin,obj,.git" 或 "bin;obj;.git"</param>
        /// <returns>解析后的文件夹名称集合</returns>
        private static IEnumerable<string> ParseFolderNames(string folderNames)
        {
            if (string.IsNullOrEmpty(folderNames))
                return Enumerable.Empty<string>();

            return folderNames.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name));
        }

        public IEnumerable<FolderNode> Filter(IEnumerable<FolderNode> folders)
        {
            return folders?.Where(IsMatch) ?? Enumerable.Empty<FolderNode>();
        }

        public bool IsMatch(FolderNode folder)
        {
            if (folder == null || string.IsNullOrEmpty(folder.FullPath))
                return false;

            var folderName = folder.Name;

            return !_excludedFolderNames.Contains(folderName);
        }

        /// <summary>
        /// 添加要排除的文件夹名称
        /// </summary>
        public void AddExcludedFolder(string folderName)
        {
            if (!string.IsNullOrEmpty(folderName))
            {
                _excludedFolderNames.Add(folderName.Trim());
            }
        }

        /// <summary>
        /// 移除要排除的文件夹名称
        /// </summary>
        public void RemoveExcludedFolder(string folderName)
        {
            if (!string.IsNullOrEmpty(folderName))
            {
                _excludedFolderNames.Remove(folderName.Trim());
            }
        }

        /// <summary>
        /// 清空所有过滤条件
        /// </summary>
        public void Clear()
        {
            _excludedFolderNames?.Clear();
        }

        /// <summary>
        /// 获取当前排除的文件夹名称列表
        /// </summary>
        public IReadOnlyCollection<string> GetExcludedFolders()
        {
            return _excludedFolderNames?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        }

        /// <summary>
        /// 检查是否排除指定的文件夹名称
        /// </summary>
        public bool IsExcluded(string folderName)
        {
            return _excludedFolderNames.Contains(folderName?.Trim() ?? string.Empty);
        }
    }
}