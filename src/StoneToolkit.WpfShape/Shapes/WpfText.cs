using StoneToolkit.WpfShape.Types;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace StoneToolkit.WpfShape.Shapes
{
    public class WpfText : FrameworkElement
    {
        public WpfText()
        {
        }

        #region Content

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(WpfText),
                new FrameworkPropertyMetadata("Text", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, OnContentChanged));

        public string Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WpfText = d as WpfText;
            WpfText.Content = (string)e.NewValue;

            WpfText.OnSizeChanged();
        }

        #endregion

        #region Culture

        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(WpfText),
                new FrameworkPropertyMetadata(CultureInfo.GetCultureInfo("en-us"), FrameworkPropertyMetadataOptions.AffectsRender));

        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)GetValue(CultureProperty);
            }
            set
            {
                SetValue(CultureProperty, value);
            }
        }

        #endregion

        #region FlowDirection

        public new static readonly DependencyProperty FlowDirectionProperty =
            DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(WpfText),
                new FrameworkPropertyMetadata(FlowDirection.LeftToRight, FrameworkPropertyMetadataOptions.AffectsRender));

        public new FlowDirection FlowDirection
        {
            get
            {
                return (FlowDirection)GetValue(FlowDirectionProperty);
            }
            set
            {
                SetValue(FlowDirectionProperty, value);
            }
        }

        #endregion

        #region Background

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(WpfText),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        #endregion

        #region Foreground

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(WpfText),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        #endregion

        #region Font

        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register("Font", typeof(Font), typeof(WpfText),
                new FrameworkPropertyMetadata(new Font(), FrameworkPropertyMetadataOptions.AffectsRender, OnFontChanged));

        private static void OnFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WpfText = d as WpfText;
            WpfText.Font = (Font)e.NewValue;

            WpfText.OnSizeChanged();
        }

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

        #region IsStretch

        public static readonly DependencyProperty IsStretchProperty =
            DependencyProperty.Register("IsStretch", typeof(bool), typeof(WpfText),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsStretch
        {
            get
            {
                return (bool)GetValue(IsStretchProperty);
            }
            set
            {
                SetValue(IsStretchProperty, value);
            }
        }

        #endregion

        private void OnSizeChanged()
        {
            FormattedText formattedText = FormattedText();
            Width = formattedText.WidthIncludingTrailingWhitespace;
            Height = FormattedText().Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            FormattedText formattedText = FormattedText();

            var startPoint = new Point(0, 0);
            drawingContext.DrawGeometry(Background, null, new RectangleGeometry(new Rect(0, 0, Width, Height)));
            if (!IsStretch)
            {
                drawingContext.DrawText(formattedText, startPoint);
            }
            else
            {
                double stretchX = 1d;
                double stretchY = 1d;

                if (!double.IsNaN(Width) && !double.IsNaN(Height))
                {
                    stretchX = Width / formattedText.WidthIncludingTrailingWhitespace;
                    stretchY = Height / formattedText.Height;
                }

                drawingContext.PushTransform(new ScaleTransform(stretchX, stretchY));
                drawingContext.DrawText(formattedText, startPoint);
            }
        }

        public FormattedText FormattedText()
        {
            var text = Content;
            if (String.IsNullOrEmpty(Content))
            {
                text = "Text";
            }

            Typeface typeface = new Typeface(Font.FontFamily, Font.FontStyle, Font.FontWeight, FontStretches.Normal);
            FormattedText formattedText = new FormattedText(text, Culture, FlowDirection, typeface, Font.FontSize, Foreground, 1);

            //formattedText.SetFontFamily(FontFamily);
            //formattedText.SetFontStyle(FontStyle);
            //formattedText.SetFontWeight(FontWeight);
            //formattedText.SetFontStretch(FontStretch);
            //formattedText.SetFontSize(FontSize);
            //formattedText.SetForegroundBrush(Foreground);

            return formattedText;
        }
    }
}
