using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using StoneToolkit.WpfCommon.TreeViewCustom;

namespace StoneToolkit.WpfCommon.Converters
{
    public class LeftMarginMultiplierConverter : IValueConverter
    {
        public double Length { get; set; }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    Thickness thickness = new Thickness(0, 0, 0, 0);
        //    if (value != null)
        //    {
        //        TreeViewItem treeViewItem = (TreeViewItem)value;
        //        ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(treeViewItem);
        //        if (parent != null)
        //        {
        //            if (parent.GetType() == typeof(TreeView))
        //            {
        //                thickness = new Thickness(System.Convert.ToDouble(Length), 0, 0, 0);
        //            }
        //            else if (parent.GetType() == typeof(TreeViewItem))
        //            {
        //                Grid grid = (Grid)UiHelpers.GetVisualChild<Grid>(parent);
        //                if (grid != null)
        //                    thickness = new Thickness(System.Convert.ToDouble(grid.Margin.Left + Length), 0, 0, 0);
        //            }
        //        }
        //    }
        //    return thickness;
        //}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null)
                return new Thickness(0);
            int extra = 0;
            if (parameter != null)
            {
                extra = int.Parse(parameter.ToString());
            }

            return new Thickness(Length * (item.GetDepth() + extra), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
