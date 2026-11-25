using System;
using System.IO;

namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    public class FileNode : FileItemNode
    {
        public FileNode(string filePath)
            : this(filePath, null)
        {
        }

        public FileNode(string filePath, FolderNode parent)
            : base(filePath, parent)
        {
        }

        public override string Name
        {
            get
            {
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(FullPath);
                return fileNameWithoutExtension;
            }
        }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension => Path.GetExtension(FullPath);

        /// <summary>
        /// 文件名（包含扩展名）
        /// </summary>
        public string FileName => Path.GetFileName(FullPath);

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize
        {
            get
            {
                try
                {
                    return new FileInfo(FullPath).Length;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                try
                {
                    return File.GetLastWriteTime(FullPath);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
    }
}
