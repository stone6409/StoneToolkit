using StoneToolkit.WpfShape.Helpers;
using StoneToolkit.WpfShape.Types;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StoneToolkit.WpfShape.Shapes
{
    public class WpfRoundRuler : Shape
    {
        public WpfRoundRuler()
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
        /// 不内缩描边：圆标尺按 <c>0..Width</c> / <c>0..Height</c> 全幅绘制弧线与刻度，
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

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(WpfRoundRuler),
                new FrameworkPropertyMetadata(135d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double StartAngle
        {
            get
            {
                return (double)GetValue(StartAngleProperty);
            }
            set
            {
                SetValue(StartAngleProperty, value);
            }
        }

        public static readonly DependencyProperty SweepAngleProperty =
            DependencyProperty.Register("SweepAngle", typeof(double), typeof(WpfRoundRuler),
                new FrameworkPropertyMetadata(270d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double SweepAngle
        {
            get
            {
                return (double)GetValue(SweepAngleProperty);
            }
            set
            {
                SetValue(SweepAngleProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("MaxValue", typeof(double), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("MajorTickCount", typeof(int), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("MinorTickCount", typeof(int), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("ShowScaleLabel", typeof(bool), typeof(WpfRoundRuler),
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

        public static readonly DependencyProperty LabelPlacementProperty = DependencyProperty.Register(
            "LabelPlacement",
            typeof(LabelPlacement),
            typeof(WpfRoundRuler),
            new FrameworkPropertyMetadata(
                LabelPlacement.Inside,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public LabelPlacement LabelPlacement
        {
            get
            {
                return (LabelPlacement)GetValue(LabelPlacementProperty);
            }
            set
            {
                SetValue(LabelPlacementProperty, value);
            }
        }

        public static readonly DependencyProperty DecimalDigitsProperty =
            DependencyProperty.Register("DecimalDigits", typeof(int), typeof(WpfRoundRuler),
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

        public static readonly DependencyProperty TickPlacementProperty = DependencyProperty.Register(
            "TickPlacement",
            typeof(TickPlacement),
            typeof(WpfRoundRuler),
            new FrameworkPropertyMetadata(
                TickPlacement.Inside,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public TickPlacement TickPlacement
        {
            get
            {
                return (TickPlacement)GetValue(TickPlacementProperty);
            }
            set
            {
                SetValue(TickPlacementProperty, value);
            }
        }

        public static readonly DependencyProperty MajorTickMarkHeightProperty =
            DependencyProperty.Register("MajorTickMarkHeight", typeof(double), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("MinorTickMarkHeight", typeof(double), typeof(WpfRoundRuler),
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

        public static readonly DependencyProperty ArcColorProperty =
            DependencyProperty.Register("ArcColor", typeof(Color), typeof(WpfRoundRuler),
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color ArcColor
        {
            get
            {
                return (Color)GetValue(ArcColorProperty);
            }
            set
            {
                SetValue(ArcColorProperty, value);
            }
        }

        public static readonly DependencyProperty MajorTickMarkColorProperty =
            DependencyProperty.Register("MajorTickMarkColor", typeof(Color), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("MinorTickMarkColor", typeof(Color), typeof(WpfRoundRuler),
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
            DependencyProperty.Register("Font", typeof(Font), typeof(WpfRoundRuler),
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
            Pen arcPen = new Pen(new SolidColorBrush(ArcColor), 1);
            Typeface typeface = new Typeface(Font.FontFamily, Font.FontStyle, Font.FontWeight, FontStretches.Normal);
            NumberFormatInfo numberInfo = new NumberFormatInfo();
            numberInfo.NumberDecimalDigits = DecimalDigits;

            Rect rect = new Rect(0, 0, Width, Height);
            FormattedText formattedText = new FormattedText(MaxValue.ToString("N", numberInfo), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, Font.FontSize, brush, 1);
            Size maxLabelSize = new Size(formattedText.Width / 2 + 2, formattedText.Height / 2 + 2);

            Rect arcRect = rect;
            if (ShowScaleLabel && LabelPlacement == LabelPlacement.Outside)
            {
                arcRect.Inflate(-maxLabelSize.Width, -maxLabelSize.Height);
            }
            if (TickPlacement == TickPlacement.Outside)
            {
                arcRect.Inflate(-MajorTickMarkHeight, -MajorTickMarkHeight);
            }

            Rect majorTickRect = arcRect;
            Rect minorTickRect = arcRect;
            if (TickPlacement == TickPlacement.Outside)
            {
                majorTickRect.Inflate(MajorTickMarkHeight, MajorTickMarkHeight);
                minorTickRect.Inflate(MinorTickMarkHeight, MinorTickMarkHeight);
            }
            else
            {
                majorTickRect.Inflate(-MajorTickMarkHeight, -MajorTickMarkHeight);
                minorTickRect.Inflate(-MinorTickMarkHeight, -MinorTickMarkHeight);
            }

            Rect labelRect = Rect.Empty;
            if (ShowScaleLabel)
            {
                labelRect = arcRect;

                if (LabelPlacement == LabelPlacement.Outside)
                {
                    labelRect.Inflate(maxLabelSize.Width, maxLabelSize.Height);

                    if (TickPlacement == TickPlacement.Outside)
                    {
                        labelRect.Inflate(MajorTickMarkHeight, MajorTickMarkHeight);
                    }
                }
                else
                {
                    labelRect.Inflate(-maxLabelSize.Width, -maxLabelSize.Height);
                    if (TickPlacement == TickPlacement.Inside)
                    {
                        labelRect.Inflate(-MajorTickMarkHeight, -MajorTickMarkHeight);
                    }
                }
            }

            PathGeometry pathGeometry = new PathGeometry();

            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = false;
            pathFigure.StartPoint = GeometryHelper.GetArcPoint(StartAngle, arcRect);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = GeometryHelper.GetArcPoint(StartAngle + SweepAngle, arcRect);
            arcSegment.Size = new Size(arcRect.Width / 2, arcRect.Height / 2);
            arcSegment.IsLargeArc = SweepAngle > 180.0;
            arcSegment.SweepDirection = SweepDirection.Clockwise;

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            drawingContext.DrawGeometry(Brushes.Transparent, arcPen, pathGeometry);

            double range = MaxValue - MinValue;
            int majorTickIntervalCount = MajorTickCount - 1;
            Pen majorTickPen = new Pen(new SolidColorBrush(MajorTickMarkColor), 1);
            int minorTickIntervalCount = MinorTickCount + 1;
            Pen minorTickPen = new Pen(new SolidColorBrush(MinorTickMarkColor), 1);

            for (int i = 0; i < MajorTickCount; i++)
            {
                double majorAngle = StartAngle + (double)i / majorTickIntervalCount * SweepAngle;

                Point point0 = GeometryHelper.GetArcPoint(majorAngle, arcRect);
                Point point1 = GeometryHelper.GetArcPoint(majorAngle, majorTickRect);
                drawingContext.DrawLine(majorTickPen, point0, point1);

                if (i < MajorTickCount - 1)
                {
                    for (int j = 1; j <= MinorTickCount; j++)
                    {
                        double minorAngle = majorAngle + (double)j / (majorTickIntervalCount * minorTickIntervalCount) * SweepAngle;

                        Point minorPoint0 = GeometryHelper.GetArcPoint(minorAngle, arcRect);
                        Point minorPoint1 = GeometryHelper.GetArcPoint(minorAngle, minorTickRect);
                        drawingContext.DrawLine(minorTickPen, minorPoint0, minorPoint1);
                    }
                }

                if (ShowScaleLabel)
                {
                    double value = (double)i / majorTickIntervalCount * range + MinValue;
                    string text = value.ToString("N", numberInfo);
                    formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, Font.FontSize, brush, 1);
                    Point point = GeometryHelper.GetArcPoint(majorAngle, labelRect);
                    point.Offset(-formattedText.Width / 2, -formattedText.Height / 2);
                    drawingContext.DrawText(formattedText, point);
                }
            }
        }
    }
}
