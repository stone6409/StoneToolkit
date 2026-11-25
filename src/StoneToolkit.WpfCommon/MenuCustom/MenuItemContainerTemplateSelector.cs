using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using StoneToolkit.WpfCommon.MenuCustom;

namespace StoneToolkit.WpfCommon.MenuCustom
{
    //public class MenuItemContainerTemplateSelector : ItemContainerTemplateSelector
    //{
    //    public DataTemplate CommandMenuItemTemplate { get; set; }
    //    public DataTemplate SeparatorItemTemplate { get; set; }

    //    public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
    //    {
    //        if (item is CommandMenuItem)
    //        {
    //            return CommandMenuItemTemplate;
    //        }
    //        else if (item is SeparatorItem)
    //        {
    //            return SeparatorItemTemplate;
    //        }

    //        return null;
    //    }
    //}

    public class MenuItemContainerTemplateSelector : ItemContainerTemplateSelector
    {
        private static ResourceDictionary _dictionary;

        static MenuItemContainerTemplateSelector()
        {
            _dictionary = new ResourceDictionary();
            //_dictionary.Source = new Uri(@"pack://application:,,,/CodeFactory;component/Styles/MenuCustomStyles.xaml");
            _dictionary.Source = new Uri(@"pack://application:,,,/StoneToolkit.WpfCommon;component/MenuCustom/MenuCustomStyles.xaml"); 
        }

        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
            var name = item == null ? null : item.GetType().Name + "Template";
            if (name != null && _dictionary.Contains(name))
            {
                return (DataTemplate)_dictionary[name];
            }

            if (item is CommandMenuItem)
            {
                return (DataTemplate)_dictionary["CommandMenuItemTemplate"];
            }
            else if (item is SeparatorItem)
            {
                return (DataTemplate)_dictionary["SeparatorItemTemplate"];
            }

            return null;
        }
    }
}
