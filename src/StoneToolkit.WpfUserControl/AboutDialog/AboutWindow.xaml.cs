using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using StoneToolkit.Common.Logging;

namespace StoneToolkit.WpfUserControl.AboutDialog
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            DataContext = new AboutViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // 打开更新检查页面或执行更新检查逻辑
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://yourcompany.com/update",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开更新页面: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 打开日志文件夹按钮点击事件
        private void OpenLogsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AboutViewModel viewModel)
            {
                OpenLogsFolder(viewModel.LogsFolderPath);
            }
        }

        // 打开日志文件夹
        public void OpenLogsFolder(string logsFolderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(logsFolderPath))
                {
                    SystemLogger.Warn("日志文件夹路径为空，无法打开");
                    return;
                }

                if (!Directory.Exists(logsFolderPath))
                {
                    SystemLogger.Warn($"日志文件夹不存在: {logsFolderPath}");
                    MessageBox.Show($"日志文件夹不存在:\n{logsFolderPath}", "提示",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 使用资源管理器打开日志文件夹
                Process.Start(new ProcessStartInfo
                {
                    FileName = logsFolderPath,
                    UseShellExecute = true
                });

                SystemLogger.Info($"成功打开日志文件夹: {logsFolderPath}");
            }
            catch (Exception ex)
            {
                SystemLogger.Error($"打开日志文件夹失败: {ex.Message}", ex);
                MessageBox.Show($"无法打开日志文件夹:\n{ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 处理超链接点击
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开链接: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}