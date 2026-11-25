using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StoneToolkit.WpfUserControl.LogViewer.Converters
{
    public class LevelToImageConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FrameworkElement frameworkElement = values[0] as FrameworkElement;
            string resourceKey = values[1] as string;

            if (frameworkElement == null || resourceKey == null)
            {
                return null;
            }

            resourceKey += "Image";
            return frameworkElement.TryFindResource(resourceKey);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
