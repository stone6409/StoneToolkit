using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfRulerExample
{
    public class WpfRoundRuler : Shape
    {
        private Rect _rect = Rect.Empty;

        public WpfRoundRuler()
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
            Brush brush = new SolidColorBrush(Colors.Black);
            Pen pen = new Pen(brush, 1);

            FontFamily fontFamily = new FontFamily("Century");
            Typeface typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            double startAngle = 135d;
            double scanAngle = 270d;
            Rect rect = new Rect(0, 0, Width, Height);

            PathGeometry pathGeometry = new PathGeometry();

            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = false;
            pathFigure.StartPoint = GeometryHelper.GetArcPoint(startAngle, rect);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = GeometryHelper.GetArcPoint(startAngle + scanAngle, rect);
            arcSegment.Size = new Size(Width / 2, Height / 2);
            arcSegment.IsLargeArc = scanAngle > 180.0;
            arcSegment.SweepDirection = SweepDirection.Clockwise;

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            drawingContext.DrawGeometry(Brushes.Transparent, pen, pathGeometry);

            int tickCount = 11;
            int intervalCount = tickCount - 1;
            Rect tickRect = rect;
            tickRect.Inflate(-10d, -10d);
            Rect tickTextRect = tickRect;
            tickTextRect.Inflate(-10d, -10d);

            for (int i = 0; i < tickCount; i++)
            {
                double angle = startAngle + (double)i / intervalCount * scanAngle;

                Point point0 = GeometryHelper.GetArcPoint(angle, rect);
                Point point1 = GeometryHelper.GetArcPoint(angle, tickRect);
                drawingContext.DrawLine(pen, point0, point1);

                string text = string.Format("{0}", i);
                FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 12, brush, 1);
                //formattedText.TextAlignment = TextAlignment.Center;
                Point point = GeometryHelper.GetArcPoint(angle, tickTextRect);
                point.Offset(-formattedText.Width / 2, -formattedText.Height / 2);
                drawingContext.DrawText(formattedText, point);

                //double fontHeight = formattedText.Height;
            }
        }
    }
}
