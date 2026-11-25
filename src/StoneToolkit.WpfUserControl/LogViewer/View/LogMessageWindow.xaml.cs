using System;
using System.Windows;

namespace StoneToolkit.WpfUserControl.LogViewer.View
{
    /// <summary>
    /// Interaction logic for LogMessageWindow.xaml
    /// </summary>
    public partial class LogMessageWindow : Window
    {
        private static LogMessageWindow _instance;

        public LogMessageWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开（或激活）日志窗口。窗口以无模式方式显示，不阻塞调用方。
        /// 该窗口不设置 Owner，可独立于主窗口显示（主窗口隐藏/最小化时依然可见）。
        /// 单实例复用，重复调用仅激活已有窗口。
        /// </summary>
        public static LogMessageWindow ShowLog()
        {
            if (_instance == null)
            {
                _instance = new LogMessageWindow();
                _instance.Show();
            }
            else
            {
                if (_instance.WindowState == WindowState.Minimized)
                {
                    _instance.WindowState = WindowState.Normal;
                }

                _instance.Activate();
            }

            return _instance;
        }

        protected override void OnClosed(EventArgs e)
        {
            _instance = null;

            base.OnClosed(e);
        }
    }
}
