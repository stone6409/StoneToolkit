using Hardcodet.Wpf.TaskbarNotification;
using StoneToolkit.Common.Utilities;
using System;
using System.Windows.Controls.Primitives;

namespace StoneToolkit.Extension.Tray
{
    public class NotifyIcon
    {
        private TaskbarIcon _taskbarIcon;
        private bool _disposed;

        public NotifyIcon(TaskbarIcon notifyIcon)
        {
            _taskbarIcon = notifyIcon;
        }

        public static string Title { get; set; } = ApplicationUtility.ApplicationName;

        private TaskbarIcon TaskbarIcon
        {
            get
            {
                return _taskbarIcon;
            }
        }

        private static NotifyIcon _intance;

        public static NotifyIcon Intance
        {
            get
            {
                if (_intance == null)
                {
                    InitIntance();
                }

                return _intance;
            }
        }

        public static void InitIntance()
        {
            if (_intance == null)
            {
                _intance = new NotifyIcon((TaskbarIcon)System.Windows.Application.Current.FindResource("NotifyIcon"));
            }
        }

        public static void UnInitIntance()
        {
            if (_intance != null)
            {
                _intance.Dispose();
            }
        }

        public static void ShowNotifyBalloon(string title, string text)
        {
            Intance.TaskbarIcon.ShowBalloonTip(title, text, BalloonIcon.None);
        }

        public static void ShowCustomBalloon(string title, string text, int? timeout)
        {
            // 创建自定义气球控件
            var balloon = new CustomBalloon
            {
                Title = Title,
                BalloonTitle = title,
                BalloonText = text
            };

            // 显示自定义气球
            Intance.TaskbarIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, timeout); 
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                }

                _taskbarIcon.Dispose();

                _disposed = true;
            }
        }
    }
}
