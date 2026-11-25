using System.Windows;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 格子内容工厂。每种可放入格子的内容实现一个工厂。
    /// </summary>
    public interface IContentFactory
    {
        /// <summary>唯一标识，对应 <see cref="ContentNode.ContentKey"/>。</summary>
        string Key { get; }

        /// <summary>右键菜单中显示的名称。</summary>
        string DisplayName { get; }

        /// <summary>创建内容视图。</summary>
        FrameworkElement CreateView();
    }
}
