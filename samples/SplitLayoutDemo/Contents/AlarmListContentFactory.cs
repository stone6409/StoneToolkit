using SplitLayoutDemo.Contents.Views;
using StoneToolkit.WpfControl.SplitLayoutControl;

namespace SplitLayoutDemo.Contents
{
    /// <summary>报警列表内容工厂。</summary>
    public class AlarmListContentFactory : IContentFactory
    {
        public string Key => "AlarmList";

        public string DisplayName => "报警列表";

        public System.Windows.FrameworkElement CreateView()
        {
            return new AlarmListView();
        }
    }
}
