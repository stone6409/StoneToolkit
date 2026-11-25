
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 复合文件过滤器
/// </summary>
namespace StoneToolkit.WpfUserControl.FileSystemNodes
{
    public class CompositeFileFilter : IFileFilter
    {
        private readonly List<IFileFilter> _filters = new List<IFileFilter>();

        public void AddFilter(IFileFilter filter)
        {
            if (filter != null)
                _filters.Add(filter);
        }

        public void RemoveFilter(IFileFilter filter)
        {
            _filters.Remove(filter);
        }

        public IEnumerable<FileNode> Filter(IEnumerable<FileNode> files)
        {
            var result = files ?? Enumerable.Empty<FileNode>();

            foreach (var filter in _filters)
            {
                result = filter.Filter(result);
            }

            return result;
        }

        public bool IsMatch(FileNode file)
        {
            return _filters.All(filter => filter.IsMatch(file));
        }
    }
}
