using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace StoneToolkit.WpfCustomControl.Button
{
    public class Button : ButtonBase
    {
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }

        #region Text Property

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Button), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTextChanged)));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button ownerButton = d as Button;
        }

        #endregion

        #region TextDecorations Property

        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(Button), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTextDecorationsChanged)));

        public TextDecorationCollection TextDecorations
        {
            get
            {
                return (TextDecorationCollection)GetValue(TextDecorationsProperty);
            }
            set
            {
                SetValue(TextDecorationsProperty, value);
            }
        }

        private static void OnTextDecorationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button ownerButton = d as Button;
        }

        #endregion


        #region Image Property

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(Button), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnImageChanged)));

        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button ownerButton = d as Button;
        }

        #endregion

        #region TextImageRelation Property

        public static readonly DependencyProperty TextImageRelationProperty =
            DependencyProperty.Register("TextImageRelation", typeof(TextImageRelation), typeof(Button), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTextImageRelationChanged)));

        public TextImageRelation TextImageRelation
        {
            get
            {
                return (TextImageRelation)GetValue(TextImageRelationProperty);
            }
            set
            {
                SetValue(TextImageRelationProperty, value);
            }
        }

        private static void OnTextImageRelationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button ownerButton = d as Button;
        }

        #endregion

        #region ForeColor Property

        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Color), typeof(Button), new FrameworkPropertyMetadata(Colors.Black, new PropertyChangedCallback(OnForeColorChanged)));

        public Color ForeColor
        {
            get
            {
                return (Color)GetValue(ForeColorProperty);
            }
            set
            {
                SetValue(ForeColorProperty, value);
            }
        }

        private static void OnForeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button ownerButton = d as Button;
            ownerButton.OnForeColorChanged((Color)e.NewValue);
        }

        private void OnForeColorChanged(Color color)
        {
            Foreground = new SolidColorBrush(color);
        }

        #endregion

        #region Automation

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ButtonAutomationPeer(this);
        }

        internal void AutomationButtonBaseClick()
        {
            OnClick();
        }

        #endregion
    }
}
