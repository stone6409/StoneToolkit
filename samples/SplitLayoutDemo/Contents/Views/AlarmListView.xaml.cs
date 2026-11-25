using System.Windows.Controls;

namespace SplitLayoutDemo.Contents.Views
{
    public partial class AlarmListView : UserControl
    {
        public AlarmListView()
        {
            InitializeComponent();
            AlarmGrid.ItemsSource = MockDataProvider.CreateAlarms();
        }
    }
}
