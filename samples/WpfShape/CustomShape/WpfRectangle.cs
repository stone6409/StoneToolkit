using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomShape
{
    public sealed class WpfRectangle : Shape
    {
        public WpfRectangle()
        {
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(_rect);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = new Pen();
            pen.Thickness = 1;
            pen.Brush = new SolidColorBrush(Colors.Black);

            drawingContext.DrawRectangle(Fill, pen, _rect);
        }

        private Rect _rect = new Rect(100, 100, 100, 100);
    }
}
