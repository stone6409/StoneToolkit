using SplitLayoutDemo.Contents.Views;
using StoneToolkit.WpfControl.SplitLayoutControl;

namespace SplitLayoutDemo.Contents
{
    /// <summary>实时趋势内容工厂。</summary>
    public class TrendChartContentFactory : IContentFactory
    {
        public string Key => "TrendChart";

        public string DisplayName => "实时趋势";

        public System.Windows.FrameworkElement CreateView()
        {
            return new TrendChartView();
        }
    }
}
