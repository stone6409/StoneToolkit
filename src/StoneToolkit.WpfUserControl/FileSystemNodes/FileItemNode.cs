using StoneToolkit.WpfCommon.TreeViewCustom;

namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    public abstract class FileItemNode : TreeViewNode
    {
        protected FileItemNode(string fullPath, FolderNode parent)
        {
            FullPath = fullPath;
            Parent = parent;
        }

        public FolderNode Parent { get; protected set; }

        public abstract string Name { get; }

        public string FullPath { get; protected set; }
    }
}
