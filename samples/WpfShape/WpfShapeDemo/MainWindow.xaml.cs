using StoneToolkit.WpfShape.Shapes;
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

namespace WpfShapeDemo
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

        Point _nextPoint = new Point(0, 0);

        Point GetNextPoint(int heigth)
        {
            Point point = _nextPoint;
            _nextPoint.Offset(0, heigth);

            if (_nextPoint.Y > canvas1.ActualHeight - 100)
            {
                _nextPoint.Y = 0;
                _nextPoint.Offset(120, 0);
            }

            return point;
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfLine wpfLine = new WpfLine();
            wpfLine.X1 = point.X + 20;
            wpfLine.Y1 = point.Y + 60;
            wpfLine.X2 = point.X + 100 + 20;
            wpfLine.Y2 = point.Y + 60;
            wpfLine.Stroke = new SolidColorBrush(Colors.Black);

            canvas1.Children.Add(wpfLine);
        }

        private void AddRectangle_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfRectangle wpfRectangle = new WpfRectangle();
            wpfRectangle.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfRectangle.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfRectangle.Width = 100;
            wpfRectangle.Height = 100;
            wpfRectangle.Stroke = new SolidColorBrush(Colors.Black);
            wpfRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));
            //wpfRoundRectangle.RenderTransform = new ScaleTransform(2.0, 1.0);

            canvas1.Children.Add(wpfRectangle);
        }

        private void AddRoundRectangle_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfRoundRectangle wpfRoundRectangle = new WpfRoundRectangle();
            wpfRoundRectangle.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfRoundRectangle.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfRoundRectangle.Width = 100;
            wpfRoundRectangle.Height = 100;
            wpfRoundRectangle.RadiusX = 10;
            wpfRoundRectangle.RadiusY = 20;
            wpfRoundRectangle.Stroke = new SolidColorBrush(Colors.Black);
            wpfRoundRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));
            //wpfRoundRectangle.RenderTransform = new ScaleTransform(2.0, 1.0);

            canvas1.Children.Add(wpfRoundRectangle);
        }

        private void AddEllipse_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfEllipse wpfEllipse = new WpfEllipse();
            wpfEllipse.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfEllipse.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfEllipse.Width = 100;
            wpfEllipse.Height = 100;
            wpfEllipse.Stroke = new SolidColorBrush(Colors.Black);
            wpfEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));

            canvas1.Children.Add(wpfEllipse);
        }

        private void AddPolyline_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfPolyline wpfPolyline = new WpfPolyline();
            wpfPolyline.Points = new PointCollection();
            wpfPolyline.Points.Add(new Point(point.X + 20, point.Y + 20));
            wpfPolyline.Points.Add(new Point(point.X + 120, point.Y + 20));
            wpfPolyline.Points.Add(new Point(point.X + 120, point.Y + 120));

            wpfPolyline.Stroke = new SolidColorBrush(Colors.Black);

            canvas1.Children.Add(wpfPolyline);
        }

        private void AddPolygon_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfPolygon wpfPolygon = new WpfPolygon();
            wpfPolygon.Points = new PointCollection();
            wpfPolygon.Points.Add(new Point(point.X + 20, point.Y + 20));
            wpfPolygon.Points.Add(new Point(point.X + 120, point.Y + 20));
            wpfPolygon.Points.Add(new Point(point.X + 120, point.Y + 120));

            wpfPolygon.Stroke = new SolidColorBrush(Colors.Black);
            wpfPolygon.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));

            canvas1.Children.Add(wpfPolygon);
        }

        private void AddCurve_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfCurve wpfCurve = new WpfCurve();
            wpfCurve.Points = new PointCollection();
            wpfCurve.Points.Add(new Point(point.X + 20, point.Y + 20));
            wpfCurve.Points.Add(new Point(point.X + 120, point.Y + 20));
            wpfCurve.Points.Add(new Point(point.X + 120, point.Y + 120));

            wpfCurve.Stroke = new SolidColorBrush(Colors.Black);

            canvas1.Children.Add(wpfCurve);
        }

        private void AddClosedCurve_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfClosedCurve wpfClosedCurve = new WpfClosedCurve();
            wpfClosedCurve.Points = new PointCollection();
            wpfClosedCurve.Points.Add(new Point(point.X + 20, point.Y + 20));
            wpfClosedCurve.Points.Add(new Point(point.X + 120, point.Y + 20));
            wpfClosedCurve.Points.Add(new Point(point.X + 120, point.Y + 120));

            wpfClosedCurve.Stroke = new SolidColorBrush(Colors.Black);
            wpfClosedCurve.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));

            canvas1.Children.Add(wpfClosedCurve);
        }

        private void AddArc_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfArc wpfArc = new WpfArc();
            wpfArc.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfArc.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfArc.Width = 100;
            wpfArc.Height = 100;
            wpfArc.StartAngle = 90;
            wpfArc.EndAngle = 330;
            wpfArc.Stroke = new SolidColorBrush(Colors.Black);

            canvas1.Children.Add(wpfArc);
        }

        private void AddArch_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfArch wpfArch = new WpfArch();
            wpfArch.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfArch.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfArch.Width = 100;
            wpfArch.Height = 100;
            wpfArch.StartAngle = 90;
            wpfArch.EndAngle = 330;
            wpfArch.Stroke = new SolidColorBrush(Colors.Black);
            wpfArch.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));

            canvas1.Children.Add(wpfArch);
        }

        private void AddPie_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfPie wpfPie = new WpfPie();
            wpfPie.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfPie.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfPie.Width = 100;
            wpfPie.Height = 100;
            wpfPie.StartAngle = 90;
            wpfPie.EndAngle = 330;
            wpfPie.Stroke = new SolidColorBrush(Colors.Black);
            wpfPie.Fill = new SolidColorBrush(Color.FromArgb(255, 165, 61, 61));

            canvas1.Children.Add(wpfPie);
        }

        private void AddPipe_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfPipe wpfPipe = new WpfPipe();
            wpfPipe.Points = new PointCollection();
            wpfPipe.Points.Add(new Point(point.X + 20, point.Y + 20));
            wpfPipe.Points.Add(new Point(point.X + 120, point.Y + 20));
            wpfPipe.Points.Add(new Point(point.X + 120, point.Y + 120));

            canvas1.Children.Add(wpfPipe);
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            Point point = GetNextPoint(120);

            WpfText wpfText = new WpfText();
            wpfText.SetValue(Canvas.LeftProperty, point.X + 20);
            wpfText.SetValue(Canvas.TopProperty, point.Y + 20);
            wpfText.Font = new StoneToolkit.WpfShape.Types.Font();
            wpfText.Font.FontSize = 20;

            canvas1.Children.Add(wpfText);
        }
    }
}
