/// <summary>
    /// 文件树项目改变事件参数
    /// </summary>

namespace StoneToolkit.WpfUserControl.FileTree
{
    public class FileTreeItemChangedEventArgs
    {
        /// <summary>
        /// 选中的项目路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile { get; }

        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public bool IsFolder => !IsFile;

        /// <summary>
        /// 是否有选中的项目
        /// </summary>
        public bool HasSelection => !string.IsNullOrEmpty(Path);

        public FileTreeItemChangedEventArgs(string path, bool isFile)
        {
            Path = path;
            IsFile = isFile;
        }
    }
}
