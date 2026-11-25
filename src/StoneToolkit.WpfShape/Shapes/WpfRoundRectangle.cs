using System;
using System.Windows;
using System.Windows.Media;
using StoneToolkit.WpfShape.Shapes;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfRoundRectangle : WpfClosedShape
    {
        public WpfRoundRectangle()
        {
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                Rect rect = GetShapeRect();

                if (rect.Width <= 0d || rect.Height <= 0d)
                {
                    return Geometry.Empty;
                }

                return new RectangleGeometry(rect, GetClampedRadiusX(rect), GetClampedRadiusY(rect));
            }
        }

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(WpfRoundRectangle),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double RadiusX
        {
            get
            {
                return (double)GetValue(RadiusXProperty);
            }
            set
            {
                SetValue(RadiusXProperty, value);
            }
        }

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(WpfRoundRectangle),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double RadiusY
        {
            get
            {
                return (double)GetValue(RadiusYProperty);
            }
            set
            {
                SetValue(RadiusYProperty, value);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = GetPen();
            Rect rect = GetShapeRect();

            if (rect.Width <= 0d || rect.Height <= 0d)
            {
                return;
            }

            double radiusX = GetClampedRadiusX(rect);
            double radiusY = GetClampedRadiusY(rect);

            if (VerticalFillPercent == 100 && HorizontalFillPercent == 100)
            {
                drawingContext.DrawRoundedRectangle(Fill, pen, rect, radiusX, radiusY);
            }
            else
            {
                drawingContext.DrawRoundedRectangle(Brushes.Transparent, pen, rect, radiusX, radiusY);
                DrawPercentFill(drawingContext, pen);
            }
        }

        /// <summary>
        /// 形状矩形：按描边半宽内缩（与 GraphWpf 的 <c>RectangleShape</c> 一致）。
        /// <para>由当前尺寸实时计算，不使用缓存字段，避免首帧几何为空。</para>
        /// </summary>
        private Rect GetShapeRect()
        {
            double thickness = Math.Abs(StrokeThickness);
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
                Math.Max(width - thickness, 0d),
                Math.Max(height - thickness, 0d));
        }

        /// <summary>半径钳位到半宽（与 GraphWpf 一致）。</summary>
        private double GetClampedRadiusX(Rect rect)
        {
            double radius = RadiusX;

            return double.IsNaN(radius) || double.IsInfinity(radius)
                ? 0d
                : Math.Min(Math.Max(radius, 0d), rect.Width / 2d);
        }

        /// <summary>半径钳位到半高（与 GraphWpf 一致）。</summary>
        private double GetClampedRadiusY(Rect rect)
        {
            double radius = RadiusY;

            return double.IsNaN(radius) || double.IsInfinity(radius)
                ? 0d
                : Math.Min(Math.Max(radius, 0d), rect.Height / 2d);
        }
    }
}
