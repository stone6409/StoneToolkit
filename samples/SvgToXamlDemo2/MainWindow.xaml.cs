using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpVectors.Renderers.Wpf;
using SvgConverter;

namespace SvgToXamlDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SvgFileToXamlButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ImportImageFromSvgFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string objectName;
                //WpfDrawingSettings 
                object imageObject = ConverterLogic.ConvertSvgToObject("icons.drawio.svg", ResultMode.DrawingImage, null, out objectName, new ResKeyInfo());
            }
            catch (Exception ex)
            {
                // 处理其他未知异常
                Console.WriteLine($"发生未知错误: {ex.Message}");
            }
        }
    }
}
