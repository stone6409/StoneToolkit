
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace StoneToolkit.WpfCommon.Converters
{
    public class CollectionToBoolConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is ICollection collection)
                {
                    return collection.Count > 0; // 如果集合包含元素，返回 true
                }
    
                return false;
            }
    
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}
