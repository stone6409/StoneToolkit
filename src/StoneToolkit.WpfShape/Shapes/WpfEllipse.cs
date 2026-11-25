using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfEllipse : WpfClosedShape
    {
        public WpfEllipse()
        {
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

                return new EllipseGeometry(rect);
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = GetPen();
            Rect rect = GetRenderRect();

            if (rect.Width <= 0d || rect.Height <= 0d)
            {
                return;
            }

            Point point = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            if (VerticalFillPercent == 100 && HorizontalFillPercent == 100)
            {
                drawingContext.DrawEllipse(Fill, pen, point, rect.Width / 2, rect.Height / 2);
            }
            else
            {
                drawingContext.DrawEllipse(Brushes.Transparent, pen, point, rect.Width / 2, rect.Height / 2);
                DrawPercentFill(drawingContext, pen);
            }
        }
    }
}
