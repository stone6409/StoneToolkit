using System.Windows;
using System.Windows.Media;

namespace CustomShape
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

        private void AddRectangle_Click(object sender, RoutedEventArgs e)
        {
            WpfRectangle rectangle = new WpfRectangle();
            rectangle.RenderTransform = new ScaleTransform(3.0, 1.0);

            canvas1.Children.Add(rectangle);
        }

        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            WpfPath path = new WpfPath();

            path.Stroke = System.Windows.Media.Brushes.Black;
            path.Fill = System.Windows.Media.Brushes.MediumSlateBlue;
            path.StrokeThickness = 4;

            EllipseGeometry ellipseGeometry = new EllipseGeometry();
            ellipseGeometry.Center = new System.Windows.Point(100, 100);
            ellipseGeometry.RadiusX = 40;
            ellipseGeometry.RadiusY = 60;
            path.Data = ellipseGeometry;

            canvas1.Children.Add(path);
        }
    }
}
