using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfTextExample
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

        #region FontFamily

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(WpfText),
                new FrameworkPropertyMetadata(new FontFamily("Times New Roman"), FrameworkPropertyMetadataOptions.AffectsRender, OnFontFamilyChanged));

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WpfText = d as WpfText;
            WpfText.FontFamily = (FontFamily)e.NewValue;

            WpfText.OnSizeChanged();
        }

        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }

        #endregion

        #region FontSize

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(WpfText),
                new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsRender, OnFontSizeChanged));

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WpfText = d as WpfText;
            WpfText.FontSize = (double)e.NewValue;

            WpfText.OnSizeChanged();
        }

        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        #endregion

        #region FontWeight

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(WpfText),
                new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.AffectsRender, OnFontWeightChanged));

        private static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WpfText = d as WpfText;
            WpfText.FontWeight = (FontWeight)e.NewValue;

            WpfText.OnSizeChanged();
        }

        public FontWeight FontWeight
        {
            get 
            { 
                return (FontWeight)GetValue(FontWeightProperty); 
            }
            set 
            { 
                SetValue(FontWeightProperty, value); 
            }
        }

        #endregion

        #region FontStyle

        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(WpfText),
                new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.AffectsRender, OnFontStyleChanged));

        private static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WpfText = d as WpfText;
            WpfText.FontStyle = (FontStyle)e.NewValue;

            WpfText.OnSizeChanged();
        }

        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)GetValue(FontStyleProperty);
            }
            set
            {
                SetValue(FontStyleProperty, value);
            }
        }

        #endregion

        #region FontStretch

        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(WpfText),
                new FrameworkPropertyMetadata(FontStretches.Normal, FrameworkPropertyMetadataOptions.AffectsRender));

        public FontStretch FontStretch
        {
            get 
            { 
                return (FontStretch)GetValue(FontStretchProperty); 
            }
            set 
            { 
                SetValue(FontStretchProperty, value); 
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
            
            Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            FormattedText formattedText = new FormattedText(text, Culture, FlowDirection, typeface, FontSize, Foreground, 1);

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
