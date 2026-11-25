using StoneToolkit.WpfShape.Helpers;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfCurve : WpfShape
    {
        private Geometry _curveGeometry = Geometry.Empty;

        public WpfCurve()
        {

        }

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
                "Points", typeof(PointCollection), typeof(WpfCurve),
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
            typeof(WpfCurve),
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

                if (points == null || points.Count < 2)
                {
                    _curveGeometry = Geometry.Empty;

                    return _curveGeometry;
                }

                _curveGeometry = PointsToBerizerCurveGeometry(points, false, FillRule);

                return _curveGeometry;
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
        }
    }
}
