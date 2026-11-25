using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 将布局树递归渲染为嵌套的 Grid + GridSplitter。
    /// </summary>
    internal static class LayoutPanelBuilder
    {
        public static FrameworkElement Build(LayoutNode node, SplitLayoutControl control)
        {
            SplitNode split = node as SplitNode;
            if (split != null)
            {
                return BuildSplit(split, control);
            }

            ContentNode content = node as ContentNode;
            if (content != null)
            {
                return BuildContent(content, control);
            }

            return new Border();
        }

        private static FrameworkElement BuildSplit(SplitNode split, SplitLayoutControl control)
        {
            double thickness = control.SplitterThickness;
            Grid grid = new Grid();
            FrameworkElement first = Build(split.First, control);
            FrameworkElement second = Build(split.Second, control);

            if (split.Orientation == SplitOrientation.Horizontal)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(split.Ratio, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(thickness) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1 - split.Ratio, GridUnitType.Star) });

                Grid.SetColumn(first, 0);
                Grid.SetColumn(second, 2);

                GridSplitter splitter = new GridSplitter
                {
                    Width = thickness,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    ResizeDirection = GridResizeDirection.Columns,
                    ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                    Background = control.SplitterBrush
                };
                Grid.SetColumn(splitter, 1);
                splitter.DragCompleted += (sender, args) => UpdateRatio(split, grid, thickness);

                grid.Children.Add(first);
                grid.Children.Add(splitter);
                grid.Children.Add(second);
            }
            else
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(split.Ratio, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(thickness) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1 - split.Ratio, GridUnitType.Star) });

                Grid.SetRow(first, 0);
                Grid.SetRow(second, 2);

                GridSplitter splitter = new GridSplitter
                {
                    Height = thickness,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    ResizeDirection = GridResizeDirection.Rows,
                    ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                    Background = control.SplitterBrush
                };
                Grid.SetRow(splitter, 1);
                splitter.DragCompleted += (sender, args) => UpdateRatio(split, grid, thickness);

                grid.Children.Add(first);
                grid.Children.Add(splitter);
                grid.Children.Add(second);
            }

            return grid;
        }

        private static void UpdateRatio(SplitNode split, Grid grid, double thickness)
        {
            bool horizontal = split.Orientation == SplitOrientation.Horizontal;

            double total = horizontal
                ? grid.ActualWidth - thickness
                : grid.ActualHeight - thickness;
            if (total <= 0)
            {
                return;
            }

            double first = horizontal
                ? grid.ColumnDefinitions[0].ActualWidth
                : grid.RowDefinitions[0].ActualHeight;

            split.Ratio = first / total;
        }

        private static FrameworkElement BuildContent(ContentNode node, SplitLayoutControl control)
        {
            DockPanel dock = new DockPanel { LastChildFill = true, Background = Brushes.White };

            Border header = new Border
            {
                Background = control.HeaderBackground,
                Padding = new Thickness(8, 3, 8, 3),
                Child = new TextBlock
                {
                    Text = node.Title ?? string.Empty,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = control.HeaderForeground
                }
            };
            DockPanel.SetDock(header, Dock.Top);
            dock.Children.Add(header);

            IContentFactory factory = control.Registry.Resolve(node.ContentKey);
            if (factory != null)
            {
                FrameworkElement view = factory.CreateView();
                if (view != null)
                {
                    dock.Children.Add(view);
                }
            }

            return new Border
            {
                BorderBrush = control.CellBorderBrush,
                BorderThickness = new Thickness(1),
                Child = dock,
                ContextMenu = LayoutMenuBuilder.Create(node, control)
            };
        }
    }
}
