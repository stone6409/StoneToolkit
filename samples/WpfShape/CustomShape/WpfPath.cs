using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomShape
{
    public sealed class WpfPath : Shape
    {
        public WpfPath()
        {
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(Geometry),
            typeof(Path),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            null);

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                Geometry data = Data;

                if (data == null)
                {
                    data = Geometry.Empty;
                }

                return data;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = new SolidColorBrush(System.Windows.Media.Colors.Black);

            DoubleCollection strokeDashArray = new DoubleCollection();
            strokeDashArray.Add(5d);
            pen.DashStyle = new DashStyle(strokeDashArray, 0);

            Rect bounds = DefiningGeometry.GetRenderBounds(pen);
            drawingContext.DrawRectangle(null, pen, bounds);

            RectangleGeometry rectangleGeometry = new RectangleGeometry();

            Rect fillBounds = bounds;
            fillBounds.Y += 0;
            fillBounds.Height -= 100;
            rectangleGeometry.Rect = fillBounds;

            CombinedGeometry combinedGeometry = new CombinedGeometry(GeometryCombineMode.Intersect, DefiningGeometry, rectangleGeometry);
            drawingContext.DrawGeometry(Fill, null, combinedGeometry);

            //drawingContext.DrawGeometry(Fill, null, DefiningGeometry);

            pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = new SolidColorBrush(System.Windows.Media.Colors.Black);
            drawingContext.DrawGeometry(null, pen, DefiningGeometry);
        }
    }
}
