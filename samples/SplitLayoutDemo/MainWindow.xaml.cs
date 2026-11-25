using System.Windows;

namespace SplitLayoutDemo
{
    /// <summary>
    /// 主窗口。视图模型在构造时接管布局控件的注册与交互。
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(LayoutControl);
        }
    }
}
