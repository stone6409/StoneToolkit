using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfLine : WpfShape
    {
        private LineGeometry _lineGeometry;

        public WpfLine()
        {
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(WpfLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
                new ValidateValueCallback(WpfShape.IsDoubleFinite));

        public double X1
        {
            get
            {
                return (double)GetValue(X1Property);
            }
            set
            {
                SetValue(X1Property, value);
            }
        }

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(WpfLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
                new ValidateValueCallback(WpfShape.IsDoubleFinite));

        public double Y1
        {
            get
            {
                return (double)GetValue(Y1Property);
            }
            set
            {
                SetValue(Y1Property, value);
            }
        }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(WpfLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
                new ValidateValueCallback(WpfShape.IsDoubleFinite));

        public double X2
        {
            get
            {
                return (double)GetValue(X2Property);
            }
            set
            {
                SetValue(X2Property, value);
            }
        }

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(WpfLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
                new ValidateValueCallback(WpfShape.IsDoubleFinite));

        public double Y2
        {
            get
            {
                return (double)GetValue(Y2Property);
            }
            set
            {
                SetValue(Y2Property, value);
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                Point point1 = new Point(X1, Y1);
                Point point2 = new Point(X2, Y2);

                _lineGeometry = new LineGeometry(point1, point2);

                return _lineGeometry;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = GetPen();
            drawingContext.DrawLine(pen, new Point(X1, Y1), new Point(X2, Y2));
        }
    }
}
