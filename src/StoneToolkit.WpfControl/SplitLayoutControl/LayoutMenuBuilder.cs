using System.Collections.Generic;
using System.Windows.Controls;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 生成格子的右键菜单。
    /// </summary>
    internal static class LayoutMenuBuilder
    {
        public static ContextMenu Create(ContentNode node, SplitLayoutControl control)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem splitHorizontal = new MenuItem { Header = "左右分栏" };
            splitHorizontal.Click += (sender, args) => control.Split(node, SplitOrientation.Horizontal);
            menu.Items.Add(splitHorizontal);

            MenuItem splitVertical = new MenuItem { Header = "上下分栏" };
            splitVertical.Click += (sender, args) => control.Split(node, SplitOrientation.Vertical);
            menu.Items.Add(splitVertical);

            MenuItem close = new MenuItem { Header = "关闭", IsEnabled = node.Parent != null };
            close.Click += (sender, args) => control.Close(node);
            menu.Items.Add(close);

            menu.Items.Add(new Separator());

            foreach (IContentFactory factory in control.Registry.All)
            {
                string key = factory.Key;
                MenuItem item = new MenuItem
                {
                    Header = factory.DisplayName,
                    IsCheckable = true,
                    IsChecked = key == node.ContentKey
                };
                item.Click += (sender, args) => control.ChangeContent(node, key);
                menu.Items.Add(item);
            }

            AppendGlobalItems(menu, control);

            return menu;
        }

        /// <summary>把宿主提供的全局菜单项追加到菜单末尾。</summary>
        private static void AppendGlobalItems(ContextMenu menu, SplitLayoutControl control)
        {
            System.Func<IEnumerable<MenuItem>> factory = control.MenuItemsFactory;
            if (factory == null)
            {
                return;
            }

            IEnumerable<MenuItem> items = factory();
            if (items == null)
            {
                return;
            }

            List<MenuItem> validItems = new List<MenuItem>();
            foreach (MenuItem item in items)
            {
                if (item != null)
                {
                    validItems.Add(item);
                }
            }

            if (validItems.Count == 0)
            {
                return;
            }

            menu.Items.Add(new Separator());

            foreach (MenuItem item in validItems)
            {
                menu.Items.Add(item);
            }
        }
    }
}
