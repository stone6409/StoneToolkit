using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoneToolkit.WpfCommon.DataGridCustom;
using StoneToolkit.WpfUserControl.LogViewer.Provider;
using StoneToolkit.WpfUserControl.LogViewer.ViewModel;

namespace StoneToolkit.WpfUserControl.LogViewer.View
{
    /// <summary>
    /// Interaction logic for LogRuleFiltersControl.xaml
    /// </summary>
    public partial class LogRuleFiltersControl : UserControl
    {
        public LogRuleFiltersControl()
        {
            InitializeComponent();
        }

        public LogFilterProvider LogRuleFilterProvider { get; private set; }

        public LogRuleFiltersViewModel LogRuleFiltersViewModel { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LogRuleFiltersViewModel = new LogRuleFiltersViewModel(LogFilterProvider.Default);
            DataContext = LogRuleFiltersViewModel;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            var toolBar = sender as ToolBar;
            if (toolBar == null)
            {
                return;
            }

            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGridCustomStyles.Sorting(DataGrid, e);
        }

        public void SaveCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LogRuleFiltersViewModel.UpdateLogRuleFilters();
        }

        public void CanSaveCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LogRuleFiltersViewModel != null;
        }

        public void AddCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LogRuleFilterViewModel logRuleFilterViewModel = LogRuleFiltersViewModel.AddLogRuleFilter();

            DataGrid.SelectedItem = logRuleFilterViewModel;
            FocusSelectedItem();
        }

        public void CanAddCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LogRuleFiltersViewModel != null;
        }

        public void InsertCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            int selectedIndex = DataGrid.SelectedIndex;

            LogRuleFilterViewModel sampleObjectViewModel = LogRuleFiltersViewModel.InsertLogRuleFilter(selectedIndex);

            DataGridCustomStyles.RefreshDataGridRowNumbers(DataGrid);

            DataGrid.SelectedItem = sampleObjectViewModel;
            FocusSelectedItem();
        }

        public void CanInsertCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelected;
        }

        public void DeleteCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            int selectedIndex = DataGrid.SelectedIndex;

            List<LogRuleFilterViewModel> selecteditems = DataGrid.SelectedItems.OfType<LogRuleFilterViewModel>().ToList();
            foreach (LogRuleFilterViewModel item in selecteditems)
            {
                LogRuleFiltersViewModel.RemoveLogRuleFilter(item);
            }

            int count = LogRuleFiltersViewModel.LogRuleFilterViewModels.Count;
            if (selectedIndex >= count)
            {
                selectedIndex = count - 1;
            }
            DataGrid.SelectedIndex = selectedIndex;
            FocusSelectedItem();
        }

        public void CanDeleteCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelected;
        }

        public bool IsSelected
        {
            get
            {
                return DataGrid != null && DataGrid.SelectedItems != null && DataGrid.SelectedItems.Count > 0;
            }
        }

        private void FocusSelectedItem()
        {
            DataGrid.UpdateLayout();
            DataGridCustomStyles.SelectRowByItem(DataGrid, DataGrid.SelectedItem);
        }

    }
}
