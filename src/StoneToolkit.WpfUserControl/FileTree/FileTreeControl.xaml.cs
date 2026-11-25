using System.Windows;
using System.Windows.Controls;
using StoneToolkit.WpfUserControl.FileSystemNodes;
using System.IO;
using System.Linq;
using System;

namespace StoneToolkit.WpfUserControl.FileTree
{
    /// <summary>
    /// Interaction logic for FileTreeControl.xaml
    /// </summary>
    public partial class FileTreeControl : UserControl
    {
        private FolderNode _internalRootFolder;

        public FileTreeControl()
        {
            InitializeComponent();
        }

        // 定义选中项目改变的事件 - 使用自定义的事件参数
        public static readonly RoutedEvent SelectedItemChangedEvent = 
            EventManager.RegisterRoutedEvent(
                "SelectedItemChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<FileTreeItemChangedEventArgs>),
                typeof(FileTreeControl));

        /// <summary>
        /// 选中项目改变事件
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FileTreeItemChangedEventArgs> SelectedItemChanged
        {
            add { AddHandler(SelectedItemChangedEvent, value); }
            remove { RemoveHandler(SelectedItemChangedEvent, value); }
        }

        public static readonly DependencyProperty RootFolderProperty =
            DependencyProperty.Register("RootFolder", typeof(string), typeof(FileTreeControl), 
                new PropertyMetadata(null, OnRootFolderChanged));

        public string RootFolder
        {
            get { return (string)GetValue(RootFolderProperty); }
            set { SetValue(RootFolderProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(string), typeof(FileTreeControl), 
                new PropertyMetadata(null, OnSelectedItemChanged));

        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof(string), typeof(FileTreeControl), 
                new PropertyMetadata(null, OnFileFilterChanged));

        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }

        public static readonly DependencyProperty FolderFilterProperty =
            DependencyProperty.Register("FolderFilter", typeof(string), typeof(FileTreeControl), 
                new PropertyMetadata(null, OnFolderFilterChanged));

        public string FolderFilter
        {
            get { return (string)GetValue(FolderFilterProperty); }
            set { SetValue(FolderFilterProperty, value); }
        }

        private static void OnRootFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeControl)d;
            control.UpdateRootFolder();
            control.UpdateFileFilter();
            control.UpdateFolderFilter();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeControl)d;
            var oldValue = CreateEventArgs((string)e.OldValue);
            var newValue = CreateEventArgs((string)e.NewValue);
            control.RaiseSelectedItemChangedEvent(oldValue, newValue);
        }

        private static void OnFileFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeControl)d;
            control.UpdateFileFilter();
        }

        private static void OnFolderFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeControl)d;
            control.UpdateFolderFilter();
        }

        /// <summary>
        /// 创建事件参数
        /// </summary>
        private static FileTreeItemChangedEventArgs CreateEventArgs(string path, bool? isFile = null)
        {
            if (string.IsNullOrEmpty(path))
                return new FileTreeItemChangedEventArgs(null, false);

            // 如果没有指定isFile，则根据路径判断
            if (!isFile.HasValue)
            {
                isFile = File.Exists(path);
            }

            return new FileTreeItemChangedEventArgs(path, isFile.Value);
        }

        /// <summary>
        /// 根据节点类型创建事件参数
        /// </summary>
        private static FileTreeItemChangedEventArgs CreateEventArgsFromNode(FileItemNode node)
        {
            if (node == null)
                return new FileTreeItemChangedEventArgs(null, false);

            // 根据节点类型直接设置IsFile属性
            bool isFile = node is FileNode;
            return new FileTreeItemChangedEventArgs(node.FullPath, isFile);
        }

        /// <summary>
        /// 触发选中项目改变事件
        /// </summary>
        private void RaiseSelectedItemChangedEvent(FileTreeItemChangedEventArgs oldValue, FileTreeItemChangedEventArgs newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<FileTreeItemChangedEventArgs>(oldValue, newValue)
            {
                RoutedEvent = SelectedItemChangedEvent
            };
            RaiseEvent(args);
        }

        private void UpdateRootFolder()
        {
            if (string.IsNullOrEmpty(RootFolder))
            {
                _internalRootFolder = null;
            }
            else
            {
                _internalRootFolder = new FolderNode(RootFolder);
                UpdateFileFilterForFolder(_internalRootFolder);
                UpdateFolderFilterForFolder(_internalRootFolder);
            }

            // 更新TreeView的数据上下文
            TreeView.DataContext = _internalRootFolder;
        }

        private void UpdateFileFilter()
        {
            if (_internalRootFolder != null)
            {
                UpdateFileFilterForFolder(_internalRootFolder);
                RefreshTreeView();
            }
        }

        private void UpdateFolderFilter()
        {
            if (_internalRootFolder != null)
            {
                UpdateFolderFilterForFolder(_internalRootFolder);
                RefreshTreeView();
            }
        }

        private void UpdateFileFilterForFolder(FolderNode folder)
        {
            if (string.IsNullOrEmpty(FileFilter))
            {
                folder.FileFilter = null;
            }
            else
            {
                folder.FileFilter = new FileExtensionFilter(FileFilter);
            }
        }

        private void UpdateFolderFilterForFolder(FolderNode folder)
        {
            if (string.IsNullOrEmpty(FolderFilter))
            {
                folder.FolderFilter = null;
            }
            else
            {
                folder.FolderFilter = new FolderNameFilter(FolderFilter);
            }
        }

        /// <summary>
        /// 刷新树视图以应用新的过滤器
        /// </summary>
        private void RefreshTreeView()
        {
            if (_internalRootFolder != null)
            {
                _internalRootFolder.RefreshChildren();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FileTreeItemChangedEventArgs oldEventArgs = CreateEventArgsFromNode(e.OldValue as FileItemNode);
            FileTreeItemChangedEventArgs newEventArgs = CreateEventArgsFromNode(e.NewValue as FileItemNode);

            // 触发自定义的选中项目改变事件
            RaiseSelectedItemChangedEvent(oldEventArgs, newEventArgs);

            // 只更新SelectedItem属性，避免重复触发事件
            if (e.NewValue is FileItemNode selectedNode)
            {
                SelectedItem = selectedNode.FullPath;
            }
            else
            {
                SelectedItem = null;
            }
        }

        /// <summary>
        /// 刷新树视图
        /// </summary>
        public void Refresh()
        {
            _internalRootFolder?.RefreshChildren();
        }

        /// <summary>
        /// 展开指定路径的节点
        /// </summary>
        public void SelectFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || _internalRootFolder == null)
                return;

            // 确保路径在根文件夹内
            if (!filePath.StartsWith(_internalRootFolder.FullPath, StringComparison.OrdinalIgnoreCase))
                return;

            // 使用递归查找文件节点
            var fileNode = FindFileNode(_internalRootFolder, filePath);

            if (fileNode != null)
            {
                fileNode.IsSelected = true;

                // 展开所有父节点
                ExpandParentNodes(fileNode);
            }
        }

        /// <summary>
        /// 递归查找文件节点
        /// </summary>
        /// <param name="currentNode">当前节点</param>
        /// <param name="filePath">文件的完整路径</param>
        /// <returns>找到的文件节点，如果未找到则返回 null</returns>
        private FileNode FindFileNode(FileItemNode currentNode, string filePath)
        {
            if (currentNode is FileNode fileNode &&
                string.Equals(fileNode.FullPath, filePath, StringComparison.OrdinalIgnoreCase))
            {
                return fileNode;
            }

            if (currentNode is FolderNode folderNode)
            {
                // 确保子节点已加载
                var children = folderNode.Children;

                foreach (var child in children)
                {
                    var foundNode = FindFileNode(child, filePath);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 展开文件节点的所有父节点，确保文件在树状视图中可见
        /// </summary>
        /// <param name="node">需要显示的文件节点</param>
        private void ExpandParentNodes(FileItemNode node)
        {
            // 检查是否有父节点
            if (node.Parent == null)
                return;

            // 先展开所有祖先节点（从根节点开始）
            ExpandParentNodes(node.Parent);

            // 展开当前父节点
            if (node.Parent is FolderNode parentFolder)
            {
                parentFolder.IsExpanded = true;
            }
        }

        /// <summary>
        /// 获取内部根文件夹节点（用于高级操作）
        /// </summary>
        public FolderNode GetInternalRootFolder()
        {
            return _internalRootFolder;
        }

        /// <summary>
        /// 设置自定义的文件夹过滤器
        /// </summary>
        /// <param name="folderFilter">自定义文件夹过滤器</param>
        public void SetCustomFolderFilter(IFolderFilter folderFilter)
        {
            if (_internalRootFolder != null)
            {
                _internalRootFolder.FolderFilter = folderFilter;
                RefreshTreeView();
            }
        }

        /// <summary>
        /// 设置自定义的文件过滤器
        /// </summary>
        /// <param name="fileFilter">自定义文件过滤器</param>
        public void SetCustomFileFilter(IFileFilter fileFilter)
        {
            if (_internalRootFolder != null)
            {
                _internalRootFolder.FileFilter = fileFilter;
                RefreshTreeView();
            }
        }
    }
}