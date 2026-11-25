using StoneToolkit.WpfShape.Types;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public abstract class WpfClosedShape : WpfShape
    {
        protected WpfClosedShape()
        {
        }

        public static readonly DependencyProperty HorizontalFillPercentProperty =
            DependencyProperty.Register("HorizontalFillPercent", typeof(double), typeof(WpfClosedShape),
                new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender), new ValidateValueCallback(IsFillPercentValid));

        public static readonly DependencyProperty VerticalFillPercentProperty =
            DependencyProperty.Register("VerticalFillPercent", typeof(double), typeof(WpfClosedShape),
                new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender), new ValidateValueCallback(IsFillPercentValid));

        private static bool IsFillPercentValid(object value)
        {
            double doubleValue = (double)value;
            return doubleValue >= 0.0 && doubleValue <= 100.0;
        }

        public double HorizontalFillPercent
        {
            get
            {
                return (double)GetValue(HorizontalFillPercentProperty);
            }
            set
            {
                SetValue(HorizontalFillPercentProperty, value);
            }
        }

        public double VerticalFillPercent
        {
            get
            {
                return (double)GetValue(VerticalFillPercentProperty);
            }
            set
            {
                SetValue(VerticalFillPercentProperty, value);
            }
        }

        public static readonly DependencyProperty HorizontalFillDirectionProperty =
            DependencyProperty.Register("HorizontalFillDirection", typeof(HorizontalFillDirection), typeof(WpfClosedShape),
              new FrameworkPropertyMetadata(HorizontalFillDirection.LeftToRight, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty VerticalFillDirectionProperty =
            DependencyProperty.Register("VerticalFillDirection", typeof(VerticalFillDirection), typeof(WpfClosedShape),
              new FrameworkPropertyMetadata(VerticalFillDirection.BottomToTop, FrameworkPropertyMetadataOptions.AffectsRender));

        public HorizontalFillDirection HorizontalFillDirection
        {
            get
            {
                return (HorizontalFillDirection)GetValue(HorizontalFillDirectionProperty);
            }
            set
            {
                SetValue(HorizontalFillDirectionProperty, value);
            }
        }

        public VerticalFillDirection VerticalFillDirection
        {
            get
            {
                return (VerticalFillDirection)GetValue(VerticalFillDirectionProperty);
            }
            set
            {
                SetValue(VerticalFillDirectionProperty, value);
            }
        }

        protected void DrawPercentFill(DrawingContext drawingContext, Pen pen)
        {
            var rectangleGeometry = new RectangleGeometry();
            Rect rect = DefiningGeometry.GetRenderBounds(pen);
            if (rect.Width >= StrokeThickness && rect.Height >= StrokeThickness)
            {
                switch (HorizontalFillDirection)
                {
                    case HorizontalFillDirection.RightToLeft:
                        rect.X += rect.Width * (100 - HorizontalFillPercent) / 100;
                        rect.Width = rect.Width * HorizontalFillPercent;
                        break;
                    case HorizontalFillDirection.CenterToSides:
                        rect.X += rect.Width * (100 - HorizontalFillPercent) / (100 * 2);
                        rect.Width = rect.Width * HorizontalFillPercent;
                        break;
                    default:
                        rect.Width = rect.Width * HorizontalFillPercent / 100;
                        break;
                }

                switch (VerticalFillDirection)
                {
                    case VerticalFillDirection.BottomToTop:
                        rect.Y += rect.Height * (100 - VerticalFillPercent) / 100;
                        rect.Height = rect.Height * VerticalFillPercent;
                        break;
                    case VerticalFillDirection.CenterToSides:
                        rect.Y += rect.Height * (100 - VerticalFillPercent) / (100 * 2);
                        rect.Height = rect.Height * VerticalFillPercent;
                        break;
                    default:
                        rect.Height = rect.Height * VerticalFillPercent / 100;
                        break;
                }

                rectangleGeometry.Rect = rect;
            }

            Geometry geometry = DefiningGeometry.GetWidenedPathGeometry(pen);
            Geometry percentFillGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, DefiningGeometry, geometry);

            var combinedGeometry = new CombinedGeometry(GeometryCombineMode.Intersect, rectangleGeometry, percentFillGeometry);
            drawingContext.DrawGeometry(Fill, null, combinedGeometry);
        }
    }
}
