using System.Collections.Generic;

namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    /// <summary>
    /// 文件夹过滤器接口
    /// </summary>
    public interface IFolderFilter
    {
        /// <summary>
        /// 过滤文件夹集合
        /// </summary>
        /// <param name="folders">原始文件夹集合</param>
        /// <returns>过滤后的文件夹集合</returns>
        IEnumerable<FolderNode> Filter(IEnumerable<FolderNode> folders);

        /// <summary>
        /// 检查单个文件夹是否符合过滤条件
        /// </summary>
        /// <param name="folder">文件夹节点</param>
        /// <returns>是否符合条件</returns>
        bool IsMatch(FolderNode folder);
    }
}