using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 空白内容工厂，作为未注册任何内容时的兜底。
    /// </summary>
    public class EmptyContentFactory : IContentFactory
    {
        /// <summary>空白内容的固定标识。</summary>
        public const string KeyName = "Empty";

        public string Key => KeyName;

        public string DisplayName => "空";

        public FrameworkElement CreateView()
        {
            return new TextBlock
            {
                Text = "未选择内容（右键切换）",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8)),
                FontSize = 13
            };
        }
    }
}
