using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoneToolkit.WpfShape.Shapes
{
    public class WpfPipe : WpfShape
    {
        private Geometry _ployLineGeometry = Geometry.Empty;

        private RenderTargetBitmap _bitmap;

        private Rect _bitmapBound;

        public WpfPipe()
        {
            StrokeDashArray = new DoubleCollection() { 2, 2 };
        }

        public static readonly DependencyProperty LiquidVisibleProperty =
           DependencyProperty.Register("LiquidVisibility", typeof(bool), typeof(WpfPipe), 
               new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public bool LiquidVisible
        {
            get 
            { 
                return (bool)GetValue(LiquidVisibleProperty); 
            }
            set 
            { 
                SetValue(LiquidVisibleProperty, value); 
            }
        }

        public static readonly DependencyProperty PipeColorProperty =
            DependencyProperty.Register("PipeColor", typeof(Color), typeof(WpfPipe),
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color PipeColor
        {
            get 
            { 
                return (Color)GetValue(PipeColorProperty); 
            }
            set 
            { 
                SetValue(PipeColorProperty, value); 
            }
        }

        public static readonly DependencyProperty PipeWidthProperty =
            DependencyProperty.Register("PipeWidth", typeof(double), typeof(WpfPipe),
                new FrameworkPropertyMetadata(15d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPipeChanged)));

        public double PipeWidth
        {
            get 
            { 
                return (double)GetValue(PipeWidthProperty); 
            }
            set 
            { 
                SetValue(PipeWidthProperty, value); 
            }
        }

        private static void OnPipeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WpfPipe wpfPipe = d as WpfPipe;
            if (wpfPipe != null)
            {
                wpfPipe.ClearBitmap();
            }
        }

        public static readonly DependencyProperty LiquidColorProperty = DependencyProperty.Register("LiquidColor", typeof(Color), typeof(WpfPipe),
                new FrameworkPropertyMetadata(Colors.Green, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLiquidColorChanged)));

        public Color LiquidColor
        {
            get 
            { 
                return (Color)GetValue(LiquidColorProperty);
            }
            set 
            { 
                SetValue(LiquidColorProperty, value); 
            }
        }

        private static void OnLiquidColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WpfPipe wpfPipe = d as WpfPipe;
            if (wpfPipe != null)
            {
                wpfPipe.Stroke = new SolidColorBrush((Color)e.NewValue);
            }
        }

        public static readonly DependencyProperty LiquidWidthProperty = DependencyProperty.Register(
            "LiquidWidth", typeof(double), typeof(WpfPipe),
            new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LiquidWidth
        {
            get
            {
                return (double)GetValue(LiquidWidthProperty);
            }
            set
            {
                SetValue(LiquidWidthProperty, value);
            }
        }

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
                "Points", typeof(PointCollection), typeof(WpfPipe),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPipeChanged)));

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
            typeof(WpfPipe),
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

        private PathGeometry CachedGeometry()
        {
            PathFigure pathFigure = new PathFigure();
            PathGeometry polyLineGeometry = new PathGeometry();

            if (Points == null)
            {
                return null;
            }

            if (Points.Count > 0)
            {
                pathFigure.StartPoint = Points[0];

                PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                for (int i = 1; i < Points.Count; i++)
                {
                    LineSegment lineSegment = new LineSegment();
                    lineSegment.Point = Points[i];
                    LineSegmentCollection.Add(lineSegment);
                }

                pathFigure.Segments = LineSegmentCollection;
            }

            polyLineGeometry.Figures.Add(pathFigure);
            polyLineGeometry.FillRule = FillRule;

            return polyLineGeometry;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return CachedGeometry();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            _ployLineGeometry = DefiningGeometry;
            //开发时，或初始化时更新管道壁缓存图片与其边界
            if (_bitmap == null)
            {
                UpdatePipeBitmapAndBounds();
            }
            //画出管道壁
            drawingContext.DrawImage(_bitmap, _bitmapBound);

            //画液体
            if (LiquidVisible)
            {
                drawingContext.DrawGeometry(null, GetLiquidPen(), _ployLineGeometry);
            }
        }

        public Pen GetLiquidPen()
        {
            Pen pen = new Pen();
            pen.Thickness = Math.Abs(LiquidWidth);
            pen.Brush = new SolidColorBrush(LiquidColor);
            pen.StartLineCap = StrokeStartLineCap;
            pen.EndLineCap = StrokeEndLineCap;
            pen.DashCap = StrokeDashCap;
            pen.LineJoin = StrokeLineJoin;
            pen.MiterLimit = StrokeMiterLimit;
            DoubleCollection dashes = null;
            dashes = StrokeDashArray;
            double strokeDashOffset = StrokeDashOffset;
            if ((dashes != null) || (strokeDashOffset != 0.0))
            {
                pen.DashStyle = new DashStyle(dashes, strokeDashOffset);
            }
            pen.Freeze();

            return pen;
        }

        public void ClearBitmap()
        {
            if (_bitmap != null)
            {
                _bitmap.Clear();
                _bitmap = null;
            }
        }

        private void UpdatePipeBitmapAndBounds()
        {
            if (Points == null || Equals(_ployLineGeometry, Geometry.Empty) || Points.Any(p => double.IsNaN(p.X) || double.IsNaN(p.Y)))
            {
                return;
            }

            byte colorR = PipeColor.R;
            byte colorG = PipeColor.G;
            byte colorB = PipeColor.B;
            byte alpha = (PipeColor.A > (byte)0x10) ? PipeColor.A : (byte)0x10;
            double deltaR = (255 - colorR) * 2 / PipeWidth;
            double deltaG = (255 - colorG) * 2 / PipeWidth;
            double deltaB = (255 - colorB) * 2 / PipeWidth;

            var customThickness = PipeWidth;
            DrawingVisual drawingVisual = new DrawingVisual();
            VisualBitmapScalingMode = BitmapScalingMode.Fant;

            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                while (customThickness > 0)
                {
                    colorR = (byte)Math.Min(colorR + deltaR, 255);
                    colorG = (byte)Math.Min(colorG + deltaG, 255);
                    colorB = (byte)Math.Min(colorB + deltaB, 255);
                    var color = Color.FromArgb(alpha, colorR, colorG, colorB);
                    var brush = new SolidColorBrush(color);

                    Pen Pen = new Pen()
                    {
                        Brush = brush,
                        Thickness = customThickness,
                        StartLineCap = PenLineCap.Flat,
                        EndLineCap = PenLineCap.Flat,
                        LineJoin = PenLineJoin.Round
                    };
                    Pen.Freeze();

                    context.DrawGeometry(null, Pen, _ployLineGeometry);

                    customThickness -= 2;
                }

                context.Close();
            }

            try
            {
                if (drawingVisual.ContentBounds != Rect.Empty && Math.Abs(drawingVisual.ContentBounds.Width * drawingVisual.ContentBounds.Height) > 0)
                {
                    _bitmapBound = drawingVisual.ContentBounds;
                    _bitmap = new RenderTargetBitmap((int)_bitmapBound.Width, (int)_bitmapBound.Height, 96, 96, PixelFormats.Pbgra32);
                    drawingVisual.Transform = new TranslateTransform(-_bitmapBound.Left, -_bitmapBound.Top);

                    _bitmap.Render(drawingVisual);
                    _bitmap.Freeze();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            try
            {
                // Get value of current pixel
                var source = (BitmapSource)_bitmap;
                var x = (int)(hitTestParameters.HitPoint.X) - (int)_bitmapBound.Left; //ActualWidth * source.PixelWidth);
                var y = (int)(hitTestParameters.HitPoint.Y) - (int)_bitmapBound.Top; //ActualHeight * source.PixelHeight);
                var pixels = new byte[4];
                source.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
                // Check alpha channel
                if (pixels[3] < 10)
                {
                    return null;
                }
                else
                {
                    return new PointHitTestResult(this, hitTestParameters.HitPoint);
                }
            }
            catch
            {
                return null;
            }
        }

        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            return base.HitTestCore(hitTestParameters);
        }
    }
}
