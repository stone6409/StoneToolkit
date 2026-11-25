using System.Windows.Controls;

namespace SplitLayoutDemo.Contents.Views
{
    public partial class EventLogView : UserControl
    {
        public EventLogView()
        {
            InitializeComponent();
            LogList.ItemsSource = MockDataProvider.CreateLogs();
        }
    }
}
