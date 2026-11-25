using SplitLayoutDemo.Contents.Views;
using StoneToolkit.WpfControl.SplitLayoutControl;

namespace SplitLayoutDemo.Contents
{
    /// <summary>负荷曲线内容工厂。</summary>
    public class LoadCurveContentFactory : IContentFactory
    {
        public string Key => "LoadCurve";

        public string DisplayName => "负荷曲线";

        public System.Windows.FrameworkElement CreateView()
        {
            return new LoadCurveView();
        }
    }
}
