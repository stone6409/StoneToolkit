using StoneToolkit.WpfShape.Helpers;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfArch : WpfClosedShape
    {
        public WpfArch()
        {

        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(WpfArch),
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
            DependencyProperty.Register("EndAngle", typeof(double), typeof(WpfArch),
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
                return UpdateEllipseGeometry(rect, SweepDirection.Clockwise);
            }

            return UpdateArchGeometry(startNormalizeAngle, endNormalizeAngle, SweepDirection.Clockwise, rect);
        }

        private Geometry UpdateZeroAngleGeometry(double angle, Rect rect)
        {
            var geometry = new LineGeometry();
            var arcPoint = GeometryHelper.GetArcPoint(angle, rect);
            var point2 = rect.Center();
            geometry.SetIfDifferent(LineGeometry.StartPointProperty, arcPoint);
            geometry.SetIfDifferent(LineGeometry.EndPointProperty, point2);

            return geometry;
        }

        private PathGeometry UpdateEllipseGeometry(Rect rect, SweepDirection sweepDirection)
        {
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            var segment = new ArcSegment();
            var segment2 = new ArcSegment();
            double y = GeometryHelper.Lerp(rect.Top, rect.Bottom, 0.5);
            var point = new Point(rect.Left, y);
            var point2 = new Point(rect.Right, y);
            figure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            figure.SetIfDifferent(PathFigure.IsFilledProperty, true);
            figure.SetIfDifferent(PathFigure.StartPointProperty, point);
            var size = new Size(rect.Width / 2.0, rect.Height / 2.0);
            segment.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            segment.SetIfDifferent(ArcSegment.SizeProperty, size);
            segment.SetIfDifferent(ArcSegment.SweepDirectionProperty, sweepDirection);
            segment.SetIfDifferent(ArcSegment.PointProperty, point2);
            segment2.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            segment2.SetIfDifferent(ArcSegment.SizeProperty, size);
            segment2.SetIfDifferent(ArcSegment.SweepDirectionProperty, sweepDirection);
            segment2.SetIfDifferent(ArcSegment.PointProperty, point);
            geometry.Figures.Add(figure);
            figure.Segments.Add(segment);
            figure.Segments.Add(segment2);

            return geometry;
        }

        private PathGeometry UpdateArchGeometry(double start, double end, SweepDirection sweepDirection, Rect rect)
        {
            PathFigure figure;
            ArcSegment segment;
            LineSegment segment2;

            var cachedGeometry = new PathGeometry();
            cachedGeometry.Figures.Add(figure = new PathFigure());
            figure.Segments.Add(segment = new ArcSegment());
            figure.Segments.Add(segment2 = new LineSegment());
            figure.IsClosed = true;
            segment.SweepDirection = sweepDirection;
            figure.SetIfDifferent(PathFigure.StartPointProperty, GeometryHelper.GetArcPoint(start, rect));
            segment.SetIfDifferent(ArcSegment.PointProperty, GeometryHelper.GetArcPoint(end, rect));
            segment2.SetIfDifferent(LineSegment.PointProperty, GeometryHelper.GetArcPoint(end, rect));
            segment.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(rect));
            segment.SetIfDifferent(ArcSegment.IsLargeArcProperty, (System.Math.Abs(end - start)) > 180.0);

            return cachedGeometry;
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

            Geometry geometry = CachedGeometry(rect);

            if (VerticalFillPercent == 100 && HorizontalFillPercent == 100)
            {
                drawingContext.DrawGeometry(Fill, pen, geometry);
            }
            else
            {
                drawingContext.DrawGeometry(null, pen, geometry);
                DrawPercentFill(drawingContext, pen);
            }
        }
    }
}
