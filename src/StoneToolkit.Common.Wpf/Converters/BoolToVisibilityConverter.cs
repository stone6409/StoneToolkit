using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace StoneToolkit.Common.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null)
            {
                string? parameterString = parameter.ToString();
                if (parameterString != null)
                {
                    invert = bool.Parse(parameterString);
                }
            }

            bool isVisible = (bool)value;

            if (invert)
            {
                return isVisible ? Visibility.Collapsed : Visibility.Visible;
            }
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            string? parameterString = parameter.ToString();
            if (parameterString != null)
            {
                invert = bool.Parse(parameterString);
            }

            Visibility visiblility = value == null ? Visibility.Collapsed : (Visibility)value;

            if (invert)
            {
                return visiblility != Visibility.Visible;
            }
            return visiblility == Visibility.Visible;
        }
    }
}
