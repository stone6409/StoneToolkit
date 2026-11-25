using System.Collections.Generic;

namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    /// <summary>
        /// 文件过滤器接口
        /// </summary>
        public interface IFileFilter
        {
            /// <summary>
            /// 过滤文件集合
            /// </summary>
            /// <param name="files">原始文件集合</param>
            /// <returns>过滤后的文件集合</returns>
            IEnumerable<FileNode> Filter(IEnumerable<FileNode> files);

            /// <summary>
            /// 检查单个文件是否符合过滤条件
            /// </summary>
            /// <param name="file">文件节点</param>
            /// <returns>是否符合条件</returns>
            bool IsMatch(FileNode file);
        }
}
