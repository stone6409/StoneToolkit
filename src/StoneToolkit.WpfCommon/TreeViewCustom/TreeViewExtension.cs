using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoneToolkit.WpfCommon.Misc;
using StoneToolkit.WpfCommon.Utilities;

namespace StoneToolkit.WpfCommon.TreeViewCustom
{
    public static class TreeViewExtension
    {
        public static readonly DependencyProperty SelectItemOnRightClickProperty = DependencyProperty.RegisterAttached(
            "SelectItemOnRightClick",
            typeof(bool),
            typeof(TreeViewExtension),
            new UIPropertyMetadata(false, OnSelectItemOnRightClickChanged));

        public static bool GetSelectItemOnRightClick(DependencyObject d)
        {
            return (bool)d.GetValue(SelectItemOnRightClickProperty);
        }

        public static void SetSelectItemOnRightClick(DependencyObject d, bool value)
        {
            d.SetValue(SelectItemOnRightClickProperty, value);
        }

        private static void OnSelectItemOnRightClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool selectItemOnRightClick = (bool)e.NewValue;

            TreeView treeView = d as TreeView;
            if (treeView != null)
            {
                if (selectItemOnRightClick)
                    treeView.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
                else
                    treeView.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
            }
        }

        private static void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            TextBox textBox = UiHelpers.GetVisualParent<TextBox>(e.OriginalSource as DependencyObject);
            if (textBox != null)
                return;

            TreeViewItem treeViewItem = UiHelpers.GetVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
    }
}
