using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StoneToolkit.WpfShape.Shapes
{
    public abstract class WpfShape : Shape
    {
        protected WpfShape()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public Rect Bounds
        {
            get
            {
                return DefiningGeometry.Bounds;
            }
        }

        protected virtual void CacheDefiningGeometry()
        {

        }

        public Pen GetPen()
        {
            Pen pen = new Pen();
            pen.Thickness = Math.Abs(StrokeThickness);
            pen.Brush = Stroke;
            pen.StartLineCap = StrokeStartLineCap;
            pen.EndLineCap = StrokeEndLineCap;
            pen.DashCap = StrokeDashCap;
            pen.LineJoin = StrokeLineJoin;
            pen.MiterLimit = StrokeMiterLimit;

            DoubleCollection dashes = StrokeDashArray;
            double strokeDashOffset = StrokeDashOffset;
            if ((dashes != null) || (strokeDashOffset != 0.0))
            {
                pen.DashStyle = new DashStyle(dashes, strokeDashOffset);
            }

            return pen;
        }

        internal static bool IsDoubleFinite(object o)
        {
            double d = (double)o;
            return !(Double.IsInfinity(d) || Double.IsNaN(d));
        }

        internal static bool IsFillRuleValid(object valueObject)
        {
            FillRule value = (FillRule)valueObject;

            return (value == FillRule.EvenOdd) ||
                   (value == FillRule.Nonzero);
        }
    }
}
