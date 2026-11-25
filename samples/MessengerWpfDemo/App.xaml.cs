using System.Windows;
using MessengerWpfDemo.ViewModels;
using StoneToolkit.Common.Messenger;

namespace MessengerWpfDemo
{
    /// <summary>
    /// 应用入口。在此创建 MessageBus 实例并注入 ViewModel，体现依赖注入。
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var messenger = new WeakReferenceMessenger();
            var mainViewModel = new MainViewModel(messenger);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();
        }
    }
}
