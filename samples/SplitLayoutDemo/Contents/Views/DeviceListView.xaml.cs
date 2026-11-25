using System.Windows.Controls;

namespace SplitLayoutDemo.Contents.Views
{
    public partial class DeviceListView : UserControl
    {
        public DeviceListView()
        {
            InitializeComponent();
            DeviceGrid.ItemsSource = MockDataProvider.CreateDevices();
        }
    }
}
