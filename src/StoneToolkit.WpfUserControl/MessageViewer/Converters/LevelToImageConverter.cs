using System;
using System.Windows;
using System.Windows.Data;

namespace StoneToolkit.WpfUserControl.MessageViewer.Converters
{
    public class LevelToImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FrameworkElement frameworkElement = values[0] as FrameworkElement;
            string resouceKey = values[1] as string;

            if (resouceKey == null)
                return null;

            resouceKey += "Image";
            return frameworkElement.TryFindResource(resouceKey);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
