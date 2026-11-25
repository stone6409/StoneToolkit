using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace StoneToolkit.WpfCommon.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null)
            {
                invert = bool.Parse(parameter.ToString());
            }

            bool isVisible = (bool)value;

            if (invert)
            {
                return isVisible ? Visibility.Hidden : Visibility.Visible;
            }
            return isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null)
            {
                invert = bool.Parse(parameter.ToString());
            }

            Visibility visiblility = value == null ? Visibility.Hidden : (Visibility)value;

            if (invert)
            {
                return visiblility != Visibility.Visible;
            }
            return visiblility == Visibility.Visible;
        }
    }
}
