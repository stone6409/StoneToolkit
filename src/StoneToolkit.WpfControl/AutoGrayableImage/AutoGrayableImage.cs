using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using StoneToolkit.WpfControl.Effects;

namespace StoneToolkit.WpfControl
{
    public class AutoGrayableImage : Image
    {
        private double _storedOpacity = 1.0;
        private static readonly Effect _grayscaleEffect = new GrayscaleEffect();

        /// <summary>
        /// Overwritten to handle changes of IsEnabled, Source and OpacityMask properties
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name.Equals(nameof(IsEnabled)))
            {
                if (IsEnabled)
                {
                    Opacity = _storedOpacity;
                    Effect = null;
                }
                else
                {
                    _storedOpacity = Opacity;
                    Opacity = 0.5;
                    Effect = _grayscaleEffect;
                }
            }
        }
    }
}
