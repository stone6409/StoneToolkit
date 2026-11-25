using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 版面布局节点基类。整棵树由分支节点 <see cref="SplitNode"/> 与叶子节点 <see cref="ContentNode"/> 组成。
    /// </summary>
    public abstract class LayoutNode : INotifyPropertyChanged
    {
        private SplitNode _parent;

        /// <summary>父分割节点；根节点为 null。</summary>
        public SplitNode Parent
        {
            get => _parent;
            internal set
            {
                if (ReferenceEquals(_parent, value))
                {
                    return;
                }

                _parent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
