using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StoneToolkit.WpfCommon.Misc;
using StoneToolkit.WpfCommon.Utilities;

namespace StoneToolkit.WpfCommon.TreeViewCustom
{
    public static class TreeViewItemExtension
    {
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.RegisterAttached(
            "IsInEditMode",
            typeof(bool),
            typeof(TreeViewItemExtension),
            new PropertyMetadata(false, OnIsInEditModeChange));

        public static void SetIsInEditMode(DependencyObject source, bool value)
        {
            source.SetValue(IsInEditModeProperty, value);
        }

        public static bool GetIsInEditMode(DependencyObject source)
        {
            return (bool)source.GetValue(IsInEditModeProperty);
        }

        private static void OnIsInEditModeChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = dependencyObject as TreeViewItem;
            bool value = (bool)e.NewValue;

            if (uiElement != null)
            {
                ContentPresenter contentPresenter = (ContentPresenter)uiElement.Template.FindName("PART_Header", uiElement);

                //DataTemplate myDataTemplate = contentPresenter.ContentTemplate;
                //TextBlock myTextBlock = (TextBlock)myDataTemplate.FindName("textBlock", contentPresenter);
                // TextBlock textBlock = (TextBlock)contentPresenter.ContentTemplate.FindName("TextBlock", contentPresenter);
                // TextBox textBox = (TextBox)contentPresenter.ContentTemplate.FindName("TextBox", contentPresenter);

                TextBlock textBlock = UiHelpers.GetVisualChild<TextBlock>(uiElement, "TextBlock");
                TextBox textBox = UiHelpers.GetVisualChild<TextBox>(uiElement, "TextBox");

                if (value)
                {
                    textBlock.Visibility = Visibility.Hidden;
                    textBox.Visibility = Visibility.Visible;
                    textBox.SelectAll();
                    textBox.Focus();
                }
                else
                {
                    textBlock.Visibility = Visibility.Visible;
                    textBox.Visibility = Visibility.Hidden;
                }
            }
        }

        public static int GetDepth(this TreeViewItem item)
        {
            while (GetSelectedTreeViewItemParent(item) != null)
            {
                var parent = GetSelectedTreeViewItemParent(item);
                if (parent != null)
                    return parent.GetDepth() + 1;

                item = parent;
            }
            return 0;
        }

        private static TreeViewItem GetSelectedTreeViewItemParent(this TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }
    }
}
