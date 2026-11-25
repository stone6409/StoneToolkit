using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoneToolkit.WpfCommon.TreeViewCustom;
using StoneToolkit.WpfCommon.Utilities;

namespace StoneToolkit.WpfCommon.TreeViewCustom
{
    partial class TreeViewCustomStyles
    {
        private void TreeViewItem_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = (TreeViewItem)sender;
            treeViewItem = UiHelpers.GetVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);

            TreeViewNode treeViewNode = treeViewItem.DataContext as TreeViewNode;
            if (treeViewNode.IsInEditMode)
            {
                e.Handled = false;
                return;
            }

            ContentPresenter contentPresenter = UiHelpers.GetVisualChild<ContentPresenter>(treeViewItem, "PART_Header");
            StackPanel stackPanel = UiHelpers.GetVisualChild<StackPanel>(contentPresenter, "StackPanel");
            if (stackPanel != null)
            {
                ContextMenu contextMenu = stackPanel.ContextMenu;
                if (contextMenu != null)
                {
                    //e.Handled = true;

                    contextMenu.IsEnabled = true;
                    contextMenu.PlacementTarget = treeViewItem;
                    contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    contextMenu.IsOpen = true;
                }
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            TextBlock textBlock = UiHelpers.GetVisualChild<TextBlock>(textBox.Parent, "TextBlock");
            TreeViewItem treeViewItem = UiHelpers.GetVisualParent<TreeViewItem>(textBox.Parent);

            TreeViewNode treeViewNode = treeViewItem.DataContext as TreeViewNode;
            switch (e.Key)
            {
                case Key.Enter:
                    // 先强制更新绑定源（确保 LostFocus 之前 Name 已被写入），再退出编辑模式
                    // textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    treeViewNode.IsInEditMode = false;
                    break;
                case Key.Escape:
                    textBox.Text = textBlock.Text;
                    treeViewNode.IsInEditMode = false;
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            TreeViewNode treeViewNode = textBox.DataContext as TreeViewNode;
            if (treeViewNode != null)
            {
                treeViewNode.IsInEditMode = false;
            }
        }
    }
}
