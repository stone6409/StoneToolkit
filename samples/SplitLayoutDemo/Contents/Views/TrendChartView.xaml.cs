using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SplitLayoutDemo.Contents.Views
{
    public partial class TrendChartView : UserControl
    {
        private readonly IReadOnlyList<double> _samples = MockDataProvider.CreateTrendSamples();

        public TrendChartView()
        {
            InitializeComponent();
            Loaded += (sender, args) => Redraw();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            double width = Plot.ActualWidth;
            double height = Plot.ActualHeight;
            if (width <= 0 || height <= 0 || _samples.Count < 2)
            {
                return;
            }

            Plot.Children.Clear();

            double min = _samples.Min();
            double max = _samples.Max();
            double range = max - min;
            if (range <= 0)
            {
                range = 1;
            }

            PointCollection points = new PointCollection();
            for (int i = 0; i < _samples.Count; i++)
            {
                double x = i * width / (_samples.Count - 1);
                double y = height - (_samples[i] - min) / range * height;
                points.Add(new Point(x, y));
            }

            Polyline line = new Polyline
            {
                Points = points,
                Stroke = new SolidColorBrush(Color.FromRgb(0x0E, 0xA5, 0xE9)),
                StrokeThickness = 1.5
            };
            Plot.Children.Add(line);
        }
    }
}
