using System.Collections.ObjectModel;
using StoneToolkit.Common.ComponentModel;
using StoneToolkit.WpfUserControl.FileSystemNodes;

namespace StoneToolkit.WpfUserControl.FileTreeAndList
{
    public class FileTreeAndListViewModel : ObservableObject
    {
        
        private FolderNode _rootFolder;

        public FolderNode RootFolder
        {
            get
            {
                return _rootFolder;
            }
            set
            {
                _rootFolder = value;
                RaisePropertyChanged(nameof(RootFolder));
            }
        }
        
        private ObservableCollection<FileNode> _files;

        public ObservableCollection<FileNode> Files
        {
            get
            {
                return _files;
            }
            set
            {
                _files = value;
                RaisePropertyChanged(nameof(Files));
            }
        }
        
        
        private ObservableCollection<FileNode> _selectedFiles;

        public ObservableCollection<FileNode> SelectedFiles
        {
            get
            {
                return _selectedFiles;
            }
            set
            {
                _selectedFiles = value;
                RaisePropertyChanged(nameof(SelectedFiles));
            }
        }
    }
}