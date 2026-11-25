using SplitLayoutDemo.Contents.Views;
using StoneToolkit.WpfControl.SplitLayoutControl;

namespace SplitLayoutDemo.Contents
{
    /// <summary>设备状态内容工厂。</summary>
    public class DeviceListContentFactory : IContentFactory
    {
        public string Key => "DeviceList";

        public string DisplayName => "设备状态";

        public System.Windows.FrameworkElement CreateView()
        {
            return new DeviceListView();
        }
    }
}
