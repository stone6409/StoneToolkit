using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfRectangle : WpfClosedShape
    {
        public WpfRectangle()
        {
        }

        private Rect GetRenderRect()
        {
            double penThickness = GetPen().Thickness;
            double margin = penThickness / 2;
            return new Rect(margin, margin,
                Math.Max(0, Width - penThickness),
                Math.Max(0, Height - penThickness));
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(GetRenderRect());
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = GetPen();
            var rect = GetRenderRect();

            if (VerticalFillPercent == 100 && HorizontalFillPercent == 100)
            {
                drawingContext.DrawRectangle(Fill, pen, rect);
            }
            else
            {
                drawingContext.DrawRectangle(Brushes.Transparent, pen, rect);
                DrawPercentFill(drawingContext, pen);
            }
        }
    }
}
