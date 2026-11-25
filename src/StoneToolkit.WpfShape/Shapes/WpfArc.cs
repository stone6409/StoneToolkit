using StoneToolkit.WpfShape.Helpers;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfArc : WpfShape
    {
        public WpfArc()
        {

        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(WpfArc),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double StartAngle
        {
            get
            {
                return (double)GetValue(StartAngleProperty);
            }
            set
            {
                SetValue(StartAngleProperty, value);
            }
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(WpfArc),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double EndAngle
        {
            get
            {
                return (double)GetValue(EndAngleProperty);
            }
            set
            {
                SetValue(EndAngleProperty, value);
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                Rect rect = GetRenderRect();

                if (rect.Width <= 0d || rect.Height <= 0d)
                {
                    return Geometry.Empty;
                }

                return CachedGeometry(rect);
            }
        }

        /// <summary>
        /// 形状矩形：按描边半宽内缩，由当前尺寸实时计算。
        /// <para>
        /// 不使用缓存字段：WPF 会在 <see cref="OnRender"/> 之外的测量阶段读取
        /// <see cref="DefiningGeometry"/>，缓存字段会导致尺寸变化后仍按旧矩形计算（几何不重算、部分不可见）。
        /// </para>
        /// </summary>
        private Rect GetRenderRect()
        {
            double thickness = System.Math.Abs(StrokeThickness);
            double margin = thickness / 2d;

            double width = Width;
            double height = Height;

            if (double.IsNaN(width) || double.IsInfinity(width))
            {
                width = ActualWidth;
            }

            if (double.IsNaN(height) || double.IsInfinity(height))
            {
                height = ActualHeight;
            }

            return new Rect(
                margin,
                margin,
                System.Math.Max(width - thickness, 0d),
                System.Math.Max(height - thickness, 0d));
        }

        private Geometry CachedGeometry(Rect rect)
        {
            double startNormalizeAngle = GeometryHelper.NormalizeAngle(StartAngle);
            double endNormalizeAngle = GeometryHelper.NormalizeAngle(EndAngle);
            if (endNormalizeAngle < startNormalizeAngle)
            {
                endNormalizeAngle += 360.0;
            }

            bool isZeroAngle = MathHelper.AreClose(startNormalizeAngle, endNormalizeAngle);
            if (isZeroAngle)
            {
                return UpdateZeroAngleGeometry(startNormalizeAngle, rect);
            }

            if (MathHelper.IsVerySmall((endNormalizeAngle - startNormalizeAngle) % 360.0))
            {
                return UpdateClosedArcGeometry(rect, SweepDirection.Clockwise);
            }

            return UpdateOpenArcGeometry(startNormalizeAngle, endNormalizeAngle, SweepDirection.Clockwise, rect);
        }

        private Geometry UpdateZeroAngleGeometry(double angle, Rect rect)
        {
            LineGeometry lineGeometry = new LineGeometry();

            Point arcPoint = GeometryHelper.GetArcPoint(angle, rect);
            lineGeometry.StartPoint = arcPoint;
            lineGeometry.EndPoint = arcPoint;

            return lineGeometry;
        }

        private PathGeometry UpdateClosedArcGeometry(Rect rect, SweepDirection sweepDirection)
        {
            PathGeometry pathGeometry = new PathGeometry();

            double leftCenterY = GeometryHelper.Lerp(rect.Top, rect.Bottom, 0.5);
            Point point = new Point(rect.Left, leftCenterY);
            Point point2 = new Point(rect.Right, leftCenterY);
            Size size = GetArcSize(rect);

            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = false;
            pathFigure.StartPoint = point;
            pathFigure.IsFilled = false;

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = point2;
            arcSegment.SweepDirection = sweepDirection;
            arcSegment.IsLargeArc = false;
            arcSegment.Size = size;

            ArcSegment arcSegment2 = new ArcSegment();
            arcSegment2.Point = point;
            arcSegment2.SweepDirection = sweepDirection;
            arcSegment2.IsLargeArc = false;
            arcSegment2.Size = size;

            pathFigure.Segments.Add(arcSegment);
            pathFigure.Segments.Add(arcSegment2);
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        private PathGeometry UpdateOpenArcGeometry(double startAngle, double endAngle, SweepDirection sweepDirection, Rect rect)
        {
            PathGeometry pathGeometry = new PathGeometry();

            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = false;
            pathFigure.StartPoint = GeometryHelper.GetArcPoint(startAngle, rect);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = GeometryHelper.GetArcPoint(endAngle, rect);
            arcSegment.SweepDirection = sweepDirection;
            arcSegment.IsLargeArc = System.Math.Abs(endAngle - startAngle) > 180.0;
            arcSegment.Size = GetArcSize(rect);

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        private static Size GetArcSize(Rect bound)
        {
            return new Size(bound.Width / 2.0, bound.Height / 2.0);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = GetPen();
            Rect rect = GetRenderRect();

            if (rect.Width <= 0d || rect.Height <= 0d)
            {
                return;
            }

            drawingContext.DrawGeometry(null, pen, CachedGeometry(rect));
        }
    }
}
