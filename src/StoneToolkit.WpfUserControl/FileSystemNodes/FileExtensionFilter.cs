using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 文件扩展名过滤器
/// </summary>
namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    public class FileExtensionFilter : IFileFilter
    {
        private readonly HashSet<string> _allowedExtensions;

        public FileExtensionFilter(IEnumerable<string> extensions)
        {
            _allowedExtensions = new HashSet<string>(
                extensions?.Select(ext => ext.StartsWith(".") ? ext : "." + ext) 
                ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
        }

        public FileExtensionFilter(string extensions) 
            : this(ParseExtensions(extensions))
        {
        }

        /// <summary>
        /// 解析扩展名字符串，支持逗号、分号分隔的多个扩展名
        /// </summary>
        /// <param name="extensions">扩展名字符串，例如："*.cs,*.xaml,*.xml" 或 ".cs;.xaml"</param>
        /// <returns>解析后的扩展名集合</returns>
        private static IEnumerable<string> ParseExtensions(string extensions)
        {
            if (string.IsNullOrEmpty(extensions))
                return Enumerable.Empty<string>();

            return extensions.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim())
                .Where(ext => !string.IsNullOrEmpty(ext))
                .Select(ext => NormalizeExtension(ext));
        }

        /// <summary>
        /// 标准化扩展名格式
        /// </summary>
        /// <param name="extension">扩展名</param>
        /// <returns>标准化的扩展名（以点开头）</returns>
        private static string NormalizeExtension(string extension)
        {
            // 移除通配符
            extension = extension.Replace("*", "").Trim();

            // 确保以点开头
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            return extension;
        }

        public IEnumerable<FileNode> Filter(IEnumerable<FileNode> files)
        {
            return files?.Where(IsMatch) ?? Enumerable.Empty<FileNode>();
        }

        public bool IsMatch(FileNode file)
        {
            if (file == null || string.IsNullOrEmpty(file.FullPath))
                return false;

            if (_allowedExtensions.Count == 0)
                return true;

            var fileExtension = Path.GetExtension(file.FullPath);
            return _allowedExtensions.Contains(fileExtension);
        }

        /// <summary>
        /// 添加支持的扩展名
        /// </summary>
        public void AddExtension(string extension)
        {
            var normalizedExtension = NormalizeExtension(extension);
            _allowedExtensions.Add(normalizedExtension);
        }

        /// <summary>
        /// 移除支持的扩展名
        /// </summary>
        public void RemoveExtension(string extension)
        {
            var normalizedExtension = NormalizeExtension(extension);
            _allowedExtensions.Remove(normalizedExtension);
        }

        /// <summary>
        /// 清空所有扩展名限制
        /// </summary>
        public void Clear()
        {
            _allowedExtensions.Clear();
        }

        /// <summary>
        /// 获取所有支持的扩展名
        /// </summary>
        public IEnumerable<string> GetAllowedExtensions()
        {
            return _allowedExtensions.ToList();
        }

        /// <summary>
        /// 检查是否支持指定的扩展名
        /// </summary>
        public bool SupportsExtension(string extension)
        {
            var normalizedExtension = NormalizeExtension(extension);
            return _allowedExtensions.Contains(normalizedExtension);
        }
    }
}