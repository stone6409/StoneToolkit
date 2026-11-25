using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace StoneToolkit.WpfCommon.DataGridCustom
{
    /// <summary>
    /// 修复 DataGrid “列头与单元格水平错位” 的附加行为。
    ///
    /// WPF 默认 DataGrid 模板中，左上角“全选”角标按钮的宽度绑定到
    /// DataGrid.CellsPanelHorizontalOffset，而列头 Presenter（DataGridColumnHeadersPresenter）
    /// 位于该按钮右侧一列，因此“列头的水平位置 == CellsPanelHorizontalOffset”。
    /// 该值由 DataGrid 内部通过 GetAnyCellOrColumnHeader() 取一个可见的单元格/列头来推算：
    /// 取到单元格时为行头（RowHeader）宽度（正确），取到列头时为 0（错误）。
    /// 在“Tab 懒加载首次显示、窗口隐藏后恢复、窗口缩放、行头宽度变化”等时机，
    /// 该计算可能先于单元格可见而得到 0，且只在 Dispatcher 上排队计算一次、之后不再重算，
    /// 于是列头整体左偏一个行头宽度，表现为“表头与单元格错位”。
    ///
    /// 本行为在上述时机强制 DataGrid 重新计算 CellsPanelHorizontalOffset
    /// （调用其内部方法 QueueInvalidateCellsPanelHorizontalOffset），并重新测量布局，
    /// 从而使列头与单元格重新对齐。在 DataGridCustomStyles.xaml 的 DataGrid 样式上设置
    /// local:DataGridHeaderAlignment.IsEnabled="True"，可对所有使用该样式的网格统一生效。
    /// </summary>
    public static class DataGridHeaderAlignment
    {
        /// <summary>
        /// 是否启用列头对齐修复。默认 false，可通过 DataGrid 样式统一开启。
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(DataGridHeaderAlignment),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// 记录事件是否已挂接，避免重复挂接。
        /// </summary>
        private static readonly DependencyProperty IsAttachedProperty =
            DependencyProperty.RegisterAttached(
                "IsAttached",
                typeof(bool),
                typeof(DataGridHeaderAlignment),
                new PropertyMetadata(false));

        /// <summary>
        /// 标记是否已有待执行的重新对齐操作，用于合并同一帧内的多次高频触发。
        /// </summary>
        private static readonly DependencyProperty IsResyncPendingProperty =
            DependencyProperty.RegisterAttached(
                "IsResyncPending",
                typeof(bool),
                typeof(DataGridHeaderAlignment),
                new PropertyMetadata(false));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            bool enabled = (bool)e.NewValue;
            bool attached = (bool)dataGrid.GetValue(IsAttachedProperty);

            if (enabled && !attached)
            {
                Attach(dataGrid);
            }
            else if (!enabled && attached)
            {
                Detach(dataGrid);
            }
        }

        private static void Attach(DataGrid dataGrid)
        {
            dataGrid.SetValue(IsAttachedProperty, true);
            dataGrid.Loaded += OnDataGridLoaded;
            dataGrid.IsVisibleChanged += OnDataGridIsVisibleChanged;
            dataGrid.SizeChanged += OnDataGridSizeChanged;
            dataGrid.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnDataGridScrollChanged));
        }

        private static void Detach(DataGrid dataGrid)
        {
            dataGrid.SetValue(IsAttachedProperty, false);
            dataGrid.Loaded -= OnDataGridLoaded;
            dataGrid.IsVisibleChanged -= OnDataGridIsVisibleChanged;
            dataGrid.SizeChanged -= OnDataGridSizeChanged;
            dataGrid.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnDataGridScrollChanged));
        }

        private static void OnDataGridLoaded(object sender, RoutedEventArgs e)
        {
            RequestResync(sender as DataGrid);
        }

        private static void OnDataGridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null && dataGrid.IsVisible)
            {
                RequestResync(dataGrid);
            }
        }

        private static void OnDataGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RequestResync(sender as DataGrid);
        }

        private static void OnDataGridScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 仅当横向偏移变化，或视口宽度变化（竖向滚动条出现/消失）时才需要重新对齐
            if (e.HorizontalChange == 0 && e.ViewportWidthChange == 0)
            {
                return;
            }

            RequestResync(sender as DataGrid);
        }

        private static void RequestResync(DataGrid dataGrid)
        {
            if (dataGrid == null)
            {
                return;
            }

            // 合并同一帧内的多次请求
            if ((bool)dataGrid.GetValue(IsResyncPendingProperty))
            {
                return;
            }

            dataGrid.SetValue(IsResyncPendingProperty, true);
            dataGrid.Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                new Action(() =>
                {
                    dataGrid.SetValue(IsResyncPendingProperty, false);
                    Resync(dataGrid);
                }));
        }

        private static void Resync(DataGrid dataGrid)
        {
            if (dataGrid == null || !dataGrid.IsLoaded)
            {
                return;
            }

            // 1) 强制 DataGrid 重新计算 CellsPanelHorizontalOffset —— 列头水平位置的真实来源。
            //    该计算在 Dispatcher 上排队执行，完成后会触发列头重新排布，从而与单元格对齐。
            TryQueueInvalidateCellsPanelHorizontalOffset(dataGrid);

            // 2) 兜底：让列头区域按当前行头宽度 / 竖向滚动条宽度重新测量（重算 filler 列宽度），
            //    并让 DataGrid 重新测量布局。
            DataGridColumnHeadersPresenter headersPresenter = FindVisualChild<DataGridColumnHeadersPresenter>(dataGrid);
            if (headersPresenter != null)
            {
                headersPresenter.InvalidateMeasure();
            }

            dataGrid.InvalidateMeasure();
            dataGrid.UpdateLayout();
        }

        /// <summary>
        /// 通过反射调用 DataGrid 的内部方法 QueueInvalidateCellsPanelHorizontalOffset()，
        /// 请求重新计算 CellsPanelHorizontalOffset。该方法在 .NET Framework 与 .NET 中均存在。
        /// </summary>
        /// <returns>成功排队返回 true；方法不存在或调用失败返回 false（此时由调用方走兜底逻辑）。</returns>
        private static bool TryQueueInvalidateCellsPanelHorizontalOffset(DataGrid dataGrid)
        {
            if (!_queueInvalidateMethodResolved)
            {
                _queueInvalidateMethod = typeof(DataGrid).GetMethod(
                    "QueueInvalidateCellsPanelHorizontalOffset",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    Type.EmptyTypes,
                    null);
                _queueInvalidateMethodResolved = true;
            }

            if (_queueInvalidateMethod == null)
            {
                return false;
            }

            try
            {
                _queueInvalidateMethod.Invoke(dataGrid, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>缓存的 DataGrid.QueueInvalidateCellsPanelHorizontalOffset 方法（内部 API）。</summary>
        private static MethodInfo _queueInvalidateMethod;
        private static bool _queueInvalidateMethodResolved;

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    return typedChild;
                }

                T result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
