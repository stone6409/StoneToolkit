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
using StoneToolkit.Common.Utilities;
using StoneToolkit.Extension.SvgConverter;

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
            string xaml = SvgFileToXaml("icons.drawio.svg");
        }

        private void ImportImageFromSvgFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string objectName;
                object imageObject = ConverterLogic.ConvertSvgToObject("icons.drawio.svg", ResultMode.DrawingImage, null, out objectName, new ResKeyInfo());
            }
            catch (Exception ex)
            {
                // 处理其他未知异常
                Console.WriteLine($"发生未知错误: {ex.Message}");
            }
        }

        public string SvgFileToXaml(string filePath)
        {
            try
            {
                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = false,
                    TextAsGeometry = false,
                    OptimizePath = true,
                };
                //ResKeyInfo resKeyInfo = new ResKeyInfo { Prefix = "Prefix" };
                ResKeyInfo resKeyInfo = new ResKeyInfo { };
                var xaml = ConverterLogic.SvgFileToXaml(filePath, ResultMode.DrawingImage, resKeyInfo, true, settings);

                // 调用移动文件方法
                string targetDirectory = @"D:\images";
                FilePathUtility.MoveFileToDirectory(filePath, targetDirectory);

                return xaml;
            }
            catch (Exception ex)
            {
                // 处理其他未知异常
                Console.WriteLine($"发生未知错误: {ex.Message}");
                return null;
            }
        }
    }
}
