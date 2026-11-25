using StoneToolkit.WpfShape.Types;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StoneToolkit.WpfShape.Shapes
{
    public class WpfRuler : Shape
    {
        public WpfRuler()
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

                return new RectangleGeometry(rect);
            }
        }

        /// <summary>
        /// 形状矩形：由当前尺寸实时计算。
        /// <para>
        /// 不内缩描边：标尺按 <c>0..Width</c> / <c>0..Height</c> 全幅绘制刻度线，
        /// 几何仅用于包围盒与命中测试，故与绘制区域保持一致。
        /// </para>
        /// <para>
        /// 不使用缓存字段：WPF 会在 <see cref="OnRender"/> 之外的测量阶段读取
        /// <see cref="DefiningGeometry"/>，缓存字段会导致尺寸变化后仍按旧矩形计算。
        /// </para>
        /// </summary>
        private Rect GetRenderRect()
        {
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

            return new Rect(0d, 0d, System.Math.Max(width, 0d), System.Math.Max(height, 0d));
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WpfRuler),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsRender));


        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }


        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(WpfRuler),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MinValue
        {
            get
            {
                return (double)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(WpfRuler),
                new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaxValue
        {
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly DependencyProperty MajorTickCountProperty =
            DependencyProperty.Register("MajorTickCount", typeof(int), typeof(WpfRuler),
                new FrameworkPropertyMetadata(11, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public int MajorTickCount
        {
            get
            {
                return (int)GetValue(MajorTickCountProperty);
            }
            set
            {
                SetValue(MajorTickCountProperty, value);
            }
        }

        public static readonly DependencyProperty MinorTickCountProperty =
            DependencyProperty.Register("MinorTickCount", typeof(int), typeof(WpfRuler),
                new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public int MinorTickCount
        {
            get
            {
                return (int)GetValue(MinorTickCountProperty);
            }
            set
            {
                SetValue(MinorTickCountProperty, value);
            }
        }

        public static readonly DependencyProperty ShowScaleLabelProperty =
            DependencyProperty.Register("ShowScaleLabel", typeof(bool), typeof(WpfRuler),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowScaleLabel
        {
            get
            {
                return (bool)GetValue(ShowScaleLabelProperty);
            }
            set
            {
                SetValue(ShowScaleLabelProperty, value);
            }
        }

        public static readonly DependencyProperty DecimalDigitsProperty =
            DependencyProperty.Register("DecimalDigits", typeof(int), typeof(WpfRuler),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public int DecimalDigits
        {
            get
            {
                return (int)GetValue(DecimalDigitsProperty);
            }
            set
            {
                SetValue(DecimalDigitsProperty, value);
            }
        }

        public static readonly DependencyProperty MajorTickMarkHeightProperty =
            DependencyProperty.Register("MajorTickMarkHeight", typeof(double), typeof(WpfRuler),
                new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MajorTickMarkHeight
        {
            get
            {
                return (double)GetValue(MajorTickMarkHeightProperty);
            }
            set
            {
                SetValue(MajorTickMarkHeightProperty, value);
            }
        }

        public static readonly DependencyProperty MinorTickMarkHeightProperty =
            DependencyProperty.Register("MinorTickMarkHeight", typeof(double), typeof(WpfRuler),
                new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MinorTickMarkHeight
        {
            get
            {
                return (double)GetValue(MinorTickMarkHeightProperty);
            }
            set
            {
                SetValue(MinorTickMarkHeightProperty, value);
            }
        }

        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Color), typeof(WpfRuler),
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color LineColor
        {
            get
            {
                return (Color)GetValue(LineColorProperty);
            }
            set
            {
                SetValue(LineColorProperty, value);
            }
        }

        public static readonly DependencyProperty MajorTickMarkColorProperty =
            DependencyProperty.Register("MajorTickMarkColor", typeof(Color), typeof(WpfRuler),
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color MajorTickMarkColor
        {
            get
            {
                return (Color)GetValue(MajorTickMarkColorProperty);
            }
            set
            {
                SetValue(MajorTickMarkColorProperty, value);
            }
        }

        public static readonly DependencyProperty MinorTickMarkColorProperty =
            DependencyProperty.Register("MinorTickMarkColor", typeof(Color), typeof(WpfRuler),
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color MinorTickMarkColor
        {
            get
            {
                return (Color)GetValue(MinorTickMarkColorProperty);
            }
            set
            {
                SetValue(MinorTickMarkColorProperty, value);
            }
        }

        #region Font

        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register("Font", typeof(Font), typeof(WpfRuler),
                new FrameworkPropertyMetadata(new Font(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Font Font
        {
            get
            {
                return (Font)GetValue(FontProperty);
            }
            set
            {
                SetValue(FontProperty, value);
            }
        }

        #endregion



        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush brush = new SolidColorBrush(Colors.Black);
            Pen linePen = new Pen(new SolidColorBrush(LineColor), 1);
            Typeface typeface = new Typeface(Font.FontFamily, Font.FontStyle, Font.FontWeight, FontStretches.Normal);
            NumberFormatInfo numberInfo = new NumberFormatInfo();
            numberInfo.NumberDecimalDigits = DecimalDigits;

            double range = MaxValue - MinValue;
            int majorTickIntervalCount = MajorTickCount - 1;
            Pen majorTickPen = new Pen(new SolidColorBrush(MajorTickMarkColor), 1);
            int minorTickIntervalCount = MinorTickCount + 1;
            Pen minorTickPen = new Pen(new SolidColorBrush(MinorTickMarkColor), 1);

            if (Orientation == Orientation.Horizontal)
            {
                Point point0 = new Point(0, Height);
                Point point1 = new Point(Width, Height);
                drawingContext.DrawLine(linePen, point0, point1);

                for (int i = 0; i < MajorTickCount; i++)
                {
                    double x = (double)i / majorTickIntervalCount * Width;
                    double fontHeight = 0;

                    if (ShowScaleLabel)
                    {
                        double value = (double)i / majorTickIntervalCount * range + MinValue;
                        string text = value.ToString("N", numberInfo);
                        FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, Font.FontSize, brush, 1);
                        formattedText.TextAlignment = TextAlignment.Center;
                        Point point = new Point(x, 0);
                        drawingContext.DrawText(formattedText, point);

                        fontHeight = formattedText.Height;
                    }

                    point0 = new Point(x, Height);
                    point1 = new Point(x, Height - MajorTickMarkHeight);
                    drawingContext.DrawLine(majorTickPen, point0, point1);

                    if (i < MajorTickCount - 1)
                    {
                        for (int j = 1; j <= MinorTickCount; j++)
                        {
                            double minorX = x + (double)j / (majorTickIntervalCount * minorTickIntervalCount) * Width;

                            Point minorPoint0 = new Point(minorX, Height);
                            Point minorPoint1 = new Point(minorX, Height - MinorTickMarkHeight);
                            drawingContext.DrawLine(minorTickPen, minorPoint0, minorPoint1);
                        }
                    }
                }
            }
            else
            {
                Point point0 = new Point(0, 0);
                Point point1 = new Point(0, Height);
                drawingContext.DrawLine(linePen, point0, point1);

                FormattedText formattedText = new FormattedText(MaxValue.ToString("N", numberInfo), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, Font.FontSize, brush, 1);
                Size maxLabelSize = new Size(formattedText.Width + 2, formattedText.Height + 2);

                for (int i = 0; i < MajorTickCount; i++)
                {
                    double y = (double)i / majorTickIntervalCount * Height;

                    if (ShowScaleLabel)
                    {
                        double value = (double)i / majorTickIntervalCount * range + MinValue;
                        string text = value.ToString("N", numberInfo);
                        formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, Font.FontSize, brush, 1);
                        formattedText.TextAlignment = TextAlignment.Right;

                        Point point = new Point(Width, y - maxLabelSize.Height / 2);
                        drawingContext.DrawText(formattedText, point);
                    }

                    point0 = new Point(0, y);
                    point1 = new Point(MajorTickMarkHeight, y);
                    drawingContext.DrawLine(majorTickPen, point0, point1);

                    if (i < MajorTickCount - 1)
                    {
                        for (int j = 1; j <= MinorTickCount; j++)
                        {
                            double minorY = y + (double)j / (majorTickIntervalCount * minorTickIntervalCount) * Height;

                            Point minorPoint0 = new Point(0, minorY);
                            Point minorPoint1 = new Point(MinorTickMarkHeight, minorY);
                            drawingContext.DrawLine(minorTickPen, minorPoint0, minorPoint1);
                        }
                    }
                }
            }
        }
    }
}
