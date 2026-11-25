using SplitLayoutDemo.Contents.Views;
using StoneToolkit.WpfControl.SplitLayoutControl;

namespace SplitLayoutDemo.Contents
{
    /// <summary>事件日志内容工厂。</summary>
    public class EventLogContentFactory : IContentFactory
    {
        public string Key => "EventLog";

        public string DisplayName => "事件日志";

        public System.Windows.FrameworkElement CreateView()
        {
            return new EventLogView();
        }
    }
}
