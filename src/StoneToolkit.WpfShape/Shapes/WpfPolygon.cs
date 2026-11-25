using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public sealed class WpfPolygon : WpfClosedShape
    {
        private Geometry _polygonGeometry;

        public WpfPolygon()
        {
        }

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
                "Points", typeof(PointCollection), typeof(WpfPolygon),
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
            typeof(WpfPolygon),
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
                CacheDefiningGeometry();
                return _polygonGeometry;
            }
        }

        protected override void CacheDefiningGeometry()
        {
            PointCollection pointCollection = Points;
            PathFigure pathFigure = new PathFigure();

            // Are we degenerate?
            // Yes, if we don't have data
            if (pointCollection == null)
            {
                _polygonGeometry = Geometry.Empty;
                return;
            }

            // Create the polygon PathGeometry
            // ISSUE-Microsoft-07/11/2003 - Bug 859068
            // The constructor for PathFigure that takes a PointCollection is internal in the Core
            // so the below causes an A/V. Consider making it public.
            if (pointCollection.Count > 0)
            {
                pathFigure.StartPoint = pointCollection[0];

                if (pointCollection.Count > 1)
                {
                    Point[] array = new Point[pointCollection.Count - 1];

                    for (int i = 1; i < pointCollection.Count; i++)
                    {
                        array[i - 1] = pointCollection[i];
                    }

                    pathFigure.Segments.Add(new PolyLineSegment(array, true));
                }

                pathFigure.IsClosed = true;
            }

            PathGeometry polygonGeometry = new PathGeometry();
            polygonGeometry.Figures.Add(pathFigure);

            // Set FillRule
            polygonGeometry.FillRule = FillRule;

            _polygonGeometry = polygonGeometry;
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
