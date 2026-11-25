using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    public class FolderNode : FileItemNode
    {
        public FolderNode(string folderPath)
            : this(folderPath, null)
        {
        }

        public FolderNode(string folderPath, FolderNode parent)
            : base(folderPath, parent)
        {
        }

        private IFileFilter _fileFilter;
        private IFolderFilter _folderFilter;

        /// <summary>
        /// 文件过滤器
        /// </summary>
        public IFileFilter FileFilter
        {
            get => _fileFilter;
            set
            {
                _fileFilter = value;
                RefreshChildren();
            }
        }

        /// <summary>
        /// 文件夹过滤器
        /// </summary>
        public IFolderFilter FolderFilter
        {
            get => _folderFilter;
            set
            {
                _folderFilter = value;
                RefreshChildren();
            }
        }

        protected ObservableCollection<FileItemNode> _children;

        public ObservableCollection<FileItemNode> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = LoadChildren(); 
                }
                return _children;
            }
        }

        public ObservableCollection<FolderNode> Folders
        {
            get
            {
                return new ObservableCollection<FolderNode>(
                    Children.OfType<FolderNode>()
                );
            }
        }

        public ObservableCollection<FileNode> Files
        {
            get
            {
                return new ObservableCollection<FileNode>(
                    Children.OfType<FileNode>()
                );
            }
        }

        public override string Name
        {
            get
            {
                return new DirectoryInfo(FullPath).Name;
            }
        }

        protected virtual ObservableCollection<FileItemNode> LoadChildren()
        {
            var children = new ObservableCollection<FileItemNode>();

            try
            {
                // 加载子文件夹
                var subFolders = Directory.GetDirectories(FullPath)
                    .Select(subFolder => new FolderNode(subFolder, this) 
                    { 
                        FileFilter = this.FileFilter,
                        FolderFilter = this.FolderFilter 
                    });

                // 应用文件夹过滤
                var filteredFolders = _folderFilter?.Filter(subFolders) ?? subFolders;

                // 加载文件
                var files = Directory.GetFiles(FullPath)
                    .Select(file => new FileNode(file, this));

                // 应用文件过滤
                var filteredFiles = _fileFilter?.Filter(files) ?? files;

                // 先添加过滤后的文件夹，再添加过滤后的文件
                foreach (var folder in filteredFolders)
                    children.Add(folder);

                foreach (var file in filteredFiles)
                    children.Add(file);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading children for {FullPath}: {ex.Message}");
            }

            return children;
        }

        public void RefreshChildren()
        {
            _children = null;
            RaisePropertyChanged(nameof(Children));
            RaisePropertyChanged(nameof(Folders));
            RaisePropertyChanged(nameof(Files));
        }
    }
}