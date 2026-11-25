using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;

namespace StoneToolkit.Extension.Tray
{
    /// <summary>
    /// Interaction logic for CustomBalloon.xaml
    /// </summary>
    public partial class CustomBalloon : UserControl
    {
        public CustomBalloon()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 关闭气球
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.CloseBalloon();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomBalloon), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // 定义绑定属性
        public static readonly DependencyProperty BalloonTitleProperty =
            DependencyProperty.Register("BalloonTitle", typeof(string), typeof(CustomBalloon), new PropertyMetadata(string.Empty));

        public string BalloonTitle
        {
            get { return (string)GetValue(BalloonTitleProperty); }
            set { SetValue(BalloonTitleProperty, value); }
        }

        public static readonly DependencyProperty BalloonTextProperty =
            DependencyProperty.Register("BalloonText", typeof(string), typeof(CustomBalloon), new PropertyMetadata(string.Empty));

        public string BalloonText
        {
            get { return (string)GetValue(BalloonTextProperty); }
            set { SetValue(BalloonTextProperty, value); }
        }
    }
}
