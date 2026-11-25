using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using StoneToolkit.WpfCommon.Utilities;

namespace StoneToolkit.WpfCommon.DataGridCustom
{
    public partial class DataGridCustomStyles
    {
        private object _sender;

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _sender = sender;
        }

        private void DataGridCell_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_sender == null || _sender != sender)
            {
                _sender = null;
                return;
            }

            var cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                var dataGrid = FindAncestor(cell, typeof(DataGrid)) as DataGrid;
                if (dataGrid != null)
                {
                    dataGrid.CommitEdit();
                    if (dataGrid.SelectionUnit == DataGridSelectionUnit.FullRow)
                    {
                        if (dataGrid.SelectedItems.Count == 1)
                        {
                            if (dataGrid.SelectionMode != DataGridSelectionMode.Single)
                            {
                                dataGrid.SelectedItems.Clear();
                            }
                            dataGrid.SelectedItem = null;
                        }
                        var row = FindAncestor(cell, typeof(DataGridRow)) as DataGridRow;
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                    else
                    {
                        dataGrid.UnselectAllCells();
                    }
                    if (cell.Column is DataGridTemplateColumn &&
                        (cell.Column as DataGridTemplateColumn).CellEditingTemplate == null &&
                        (cell.Column as DataGridTemplateColumn).CellEditingTemplateSelector == null)
                    {
                        return;
                    }
                    if (!cell.IsFocused)
                    {
                        cell.Focus();
                    }
                    if (!cell.IsSelected)
                    {
                        cell.IsSelected = true;
                    }

                    //解决DoNet4.0平台CellEditingTemplateSelector无效的兼容性问题，需手动代码切换
                    var currentRow = FindAncestor(cell, typeof(DataGridRow)) as DataGridRow;
                    if (currentRow != null && cell.Column is DataGridTemplateColumn
                        && (cell.Column as DataGridTemplateColumn).CellEditingTemplateSelector != null)
                    {
                        (cell.Column as DataGridTemplateColumn).CellEditingTemplate = (cell.Column as DataGridTemplateColumn).CellEditingTemplateSelector.SelectTemplate(currentRow.Item, dataGrid);
                    }
                    cell.IsEditing = true;
                }
            }
        }

        private void DataGridSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject source = sender as DependencyObject;
            if (source is Button)
            {
                var grid = FindAncestor(source, typeof(DataGrid));
                if (grid is DataGrid)
                {
                    var datagrid = (grid as DataGrid);
                    datagrid.CommitEdit();
                    datagrid.Focus();
                }
            }
        }

        public static DependencyObject FindAncestor(DependencyObject source, Type typeAncestor)
        {
            var parent = VisualTreeHelper.GetParent(source);
            if (parent != null)
            {
                if (parent.GetType() == typeAncestor)
                    return parent;

                return FindAncestor(parent, typeAncestor);
            }

            if (source is FrameworkElement)
            {
                parent = (source as FrameworkElement).Parent;
                if (parent != null)
                    return parent.GetType() == typeAncestor ? parent : FindAncestor(parent, typeAncestor);
            }

            return null;
        }

        private void RowHeader_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject source  = sender as DependencyObject;
            if (source != null)
            {
                var datagrid = FindAncestor(source, typeof(DataGrid)) as DataGrid;
                if (datagrid != null)
                    datagrid.Focus();
            }
        }

        public static void Sorting(DataGrid dataGrid, DataGridSortingEventArgs e)
        {
            if (e.Column.SortDirection == ListSortDirection.Descending)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                if (view != null && view.SortDescriptions != null)
                {
                    view.SortDescriptions.Clear();
                    e.Column.SortDirection = null;
                }
                e.Handled = true;
            }
            dataGrid.Focus();
        }

        public static void RefreshDataGridRowNumbers(DataGrid dataGrid)
        {
            if (dataGrid == null)
                return;
            foreach (var item in dataGrid.Items)
            {
                var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                    row.Header = row.GetIndex() + 1;
            }
            dataGrid.Focus();
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = UiHelpers.GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method 
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = UiHelpers.GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(rowIndex) as DataGridRow;

            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, 0);
                if (cell != null)
                    cell.Focus();
            }
        }

        public static void SelectRowByItem(DataGrid dataGrid, object item, int column = 0)
        {
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, column);
                if (cell != null)
                    cell.Focus();
            }
            else
            {
                dataGrid.Focus();
            }
        }
    }
}
