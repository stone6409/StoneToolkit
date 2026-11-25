using StoneToolkit.WpfShape.Helpers;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfClosedCurve : WpfClosedShape
    {
        private Geometry _closedCurveGeometry = Geometry.Empty;

        public WpfClosedCurve()
        {

        }

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
                "Points", typeof(PointCollection), typeof(WpfClosedCurve),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public PointCollection Points
        {
            get
            {
                return (PointCollection)GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, value);
            }
        }

        public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register(
            "FillRule",
            typeof(FillRule),
            typeof(WpfClosedCurve),
            new FrameworkPropertyMetadata(
                FillRule.EvenOdd,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            new ValidateValueCallback(IsFillRuleValid)
            );

        public FillRule FillRule
        {
            get
            {
                return (FillRule)GetValue(FillRuleProperty);
            }
            set
            {
                SetValue(FillRuleProperty, value);
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                PointCollection points = Points;

                if (points == null || points.Count < 3)
                {
                    _closedCurveGeometry = Geometry.Empty;

                    return _closedCurveGeometry;
                }

                _closedCurveGeometry = PointsToBerizerCurveGeometry(points, true, FillRule);

                return _closedCurveGeometry;
            }
        }

        public PathGeometry PointsToBerizerCurveGeometry(PointCollection points, bool isClosed, FillRule fillRule)
        {
            var myPathFigure = new PathFigure { StartPoint = points.FirstOrDefault() };
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            var beizerSegments = InterpolationHelper.InterpolatePointWithBeizerCurves(points.ToList(), isClosed);
            if (beizerSegments == null || beizerSegments.Count == 0)
            {
                //Add a line segment <this is generic for more than one line>
                for (int i = 1; i < points.Count; i++)
                {
                    var myLineSegment = new LineSegment { Point = points[i] };
                    pathSegments.Add(myLineSegment);
                }
            }
            else
            {
                foreach (var beizerCurveSegment in beizerSegments)
                {
                    var segment = new BezierSegment
                    {
                        Point1 = beizerCurveSegment.FirstControlPoint,
                        Point2 = beizerCurveSegment.SecondControlPoint,
                        Point3 = beizerCurveSegment.EndPoint,
                    };

                    pathSegments.Add(segment);
                }
            }
            myPathFigure.Segments = pathSegments;
            myPathFigure.IsClosed = isClosed;
            myPathFigure.IsFilled = isClosed;

            var myPathFigureCollection = new PathFigureCollection { myPathFigure };
            var myPathGeometry = new PathGeometry { Figures = myPathFigureCollection, FillRule = fillRule };

            return myPathGeometry;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = GetPen();
            drawingContext.DrawGeometry(null, pen, DefiningGeometry);

            if (VerticalFillPercent == 100 && HorizontalFillPercent == 100)
            {
                drawingContext.DrawGeometry(Fill, pen, DefiningGeometry);
            }
            else
            {
                drawingContext.DrawGeometry(null, pen, DefiningGeometry);
                DrawPercentFill(drawingContext, pen);
            }
        }
    }
}
