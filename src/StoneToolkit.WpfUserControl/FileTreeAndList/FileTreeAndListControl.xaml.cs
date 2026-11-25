using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StoneToolkit.Common.Utilities;
using StoneToolkit.WpfUserControl.FileSystemNodes;

namespace StoneToolkit.WpfUserControl.FileTreeAndList
{
    public partial class FileTreeAndListControl : UserControl
    {
        public FileTreeAndListControl()
        {
            InitializeComponent();
            FileListView.SelectionChanged += FileListView_SelectionChanged;
        }

        #region Dependency Properties

        public static readonly DependencyProperty RootFolderProperty =
            DependencyProperty.Register(nameof(RootFolder), typeof(string), typeof(FileTreeAndListControl),
                new FrameworkPropertyMetadata(null, OnRootFolderChanged));

        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register(nameof(FileFilter), typeof(string), typeof(FileTreeAndListControl),
                new FrameworkPropertyMetadata(null, OnFileFilterChanged));

        public static readonly DependencyProperty FolderFilterProperty =
            DependencyProperty.Register(nameof(FolderFilter), typeof(string), typeof(FileTreeAndListControl),
                new FrameworkPropertyMetadata(null, OnFolderFilterChanged));

        public static readonly DependencyProperty SelectedFilesProperty =
            DependencyProperty.Register(nameof(SelectedFiles), typeof(ObservableCollection<string>), 
                typeof(FileTreeAndListControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty RootFolderNodeProperty =
            DependencyProperty.Register(nameof(RootFolderNode), typeof(FolderNode), typeof(FileTreeAndListControl),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register(nameof(Files), typeof(ObservableCollection<FileNode>), typeof(FileTreeAndListControl),
                new FrameworkPropertyMetadata(null));

        public string RootFolder
        {
            get => (string)GetValue(RootFolderProperty);
            set => SetValue(RootFolderProperty, value);
        }

        public string FileFilter
        {
            get => (string)GetValue(FileFilterProperty);
            set => SetValue(FileFilterProperty, value);
        }

        public string FolderFilter
        {
            get => (string)GetValue(FolderFilterProperty);
            set => SetValue(FolderFilterProperty, value);
        }

        public ObservableCollection<string> SelectedFiles
        {
            get => (ObservableCollection<string>)GetValue(SelectedFilesProperty);
            set => SetValue(SelectedFilesProperty, value);
        }

        public FolderNode RootFolderNode
        {
            get => (FolderNode)GetValue(RootFolderNodeProperty);
            private set => SetValue(RootFolderNodeProperty, value);
        }

        public ObservableCollection<FileNode> Files
        {
            get => (ObservableCollection<FileNode>)GetValue(FilesProperty);
            private set => SetValue(FilesProperty, value);
        }

        #endregion

        #region Event Handlers

        private static void OnRootFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeAndListControl)d;
            control.LoadRootFolder((string)e.NewValue);
        }

        private static void OnFileFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeAndListControl)d;
            control.UpdateFileFilter((string)e.NewValue);
        }

        private static void OnFolderFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FileTreeAndListControl)d;
            control.UpdateFolderFilter((string)e.NewValue);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FolderNode selectedFolder)
            {
                Files = selectedFolder.Files;
            }
        }

        private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedFiles();
        }

        private void FileListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DoubleClickConfirmed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        private void LoadRootFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                RootFolderNode = null;
                Files = null;
                return;
            }

            var newFolder = new FolderNode(folderPath);
            UpdateFileFilterForFolder(newFolder);
            UpdateFolderFilterForFolder(newFolder);
            RootFolderNode = newFolder;
            Files = newFolder.Files;
        }

        private void UpdateFileFilter(string fileFilter)
        {
            if (RootFolderNode != null)
            {
                UpdateFileFilterForFolder(RootFolderNode);
                RefreshTreeView();
            }
        }

        private void UpdateFolderFilter(string folderFilter)
        {
            if (RootFolderNode != null)
            {
                UpdateFolderFilterForFolder(RootFolderNode);
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

        private void UpdateSelectedFiles()
        {
            if (FileListView.SelectedItems == null || string.IsNullOrEmpty(RootFolder))
            {
                SelectedFiles = new ObservableCollection<string>();
                return;
            }

            var selectedFiles = FileListView.SelectedItems
                .Cast<FileNode>()
                .Select(file => FilePathUtility.GetRelativePath(RootFolder, file.FullPath))
                .ToList();

            SelectedFiles = new ObservableCollection<string>(selectedFiles);
        }

        /// <summary>
        /// 刷新树视图以应用新的过滤器
        /// </summary>
        private void RefreshTreeView()
        {
            if (RootFolderNode != null)
            {
                // 保存当前选中的文件夹
                var selectedFolder = FileTreeView.SelectedItem as FolderNode;

                // 刷新根文件夹的子节点
                RootFolderNode.RefreshChildren();

                // 如果之前有选中的文件夹，尝试重新选中
                if (selectedFolder == null)
                {
                    selectedFolder = RootFolderNode;
                }

                Files = selectedFolder.Files;
            }
        }

        #endregion

        public event EventHandler DoubleClickConfirmed;

        /// <summary>
        /// 手动刷新控件以应用最新的过滤器
        /// </summary>
        public void Refresh()
        {
            if (RootFolderNode != null)
            {
                RootFolderNode.RefreshChildren();
            }
        }
    }
}