using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfRulerExample
{
    public class WpfRuler : Shape
    {
        private Rect _rect = Rect.Empty;

        public WpfRuler()
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
            //double fontHeight1 = Math.Ceiling(20 * fontFamily.LineSpacing);

            Point point0 = new Point(0, Height);
            Point point1 = new Point(Width, Height);
            drawingContext.DrawLine(pen, point0, point1);

            int tickCount = 11;
            int intervalCount = tickCount - 1;
            //double tickSpacing = Width / (tickCount - 1);
            for (int i = 0; i < tickCount; i++)
            {
                double x = (double)i / intervalCount * Width;

                string text = string.Format("{0}", i);
                FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 12, brush, 1);
                formattedText.TextAlignment = TextAlignment.Center;
                Point point = new Point(x, 0);
                drawingContext.DrawText(formattedText, point);

                double fontHeight = formattedText.Height;

                point0 = new Point(x, Height);
                point1 = new Point(x, fontHeight);
                drawingContext.DrawLine(pen, point0, point1);
            }
        }
    }
}
