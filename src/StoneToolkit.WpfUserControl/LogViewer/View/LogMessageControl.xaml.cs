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
using System.Windows.Threading;
using StoneToolkit.WpfUserControl.LogViewer.ViewModel;
using StoneToolkit.Common.Logging;
using StoneToolkit.WpfUserControl.LogViewer.Provider;
using StoneToolkit.WpfCommon.DataGridCustom;

namespace StoneToolkit.WpfUserControl.LogViewer.View
{
    /// <summary>
    /// Interaction logic for LogMessageControl.xaml
    /// </summary>
    public partial class LogMessageControl : UserControl
    {
        public static RoutedUICommand ClearCommand = new RoutedUICommand();

        public static RoutedUICommand FiltersCommand = new RoutedUICommand();

        public static RoutedUICommand CopyCommand = new RoutedUICommand();

        public LogFilterProvider LogRuleFilterProvider { get; private set; }

        LogMessagesViewModel _logMessagesViewModel;

        private LogMessagesViewModel LogMessagesViewModel
        {
            get
            {
                if (_logMessagesViewModel == null)
                {
                    _logMessagesViewModel = new LogMessagesViewModel(LogFilterProvider.Default);
                }

                return _logMessagesViewModel;
            }
        }

        public LogMessageControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRuleFilters();

            SystemLogger.Logger.Logged += Logger_Logged;
            DataContext = LogMessagesViewModel;

            //SystemLogger.Trace("This is a Trace log.");
            //SystemLogger.Debug("This is a Debug log.");
            //SystemLogger.Info("This is a Info log.");
            //SystemLogger.Warn("This is a Warn log.");
            //SystemLogger.Error("This is a Error log.");
            //SystemLogger.Fatal("This is a Fatal log.");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SystemLogger.Logger.Logged -= Logger_Logged;
        }

        private void Logger_Logged(object sender, LoggerEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<LoggerEventArgs>((x) =>
            {
                LogMessagesViewModel.AddLogMessage(x.Time, x.Level, x.Message);
            }), e);
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

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGridCustomStyles.Sorting(LogDataGrid, e);
        }

        private void ClearExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LogMessagesViewModel.Clear();
        }

        private void CanClearExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CopyExecute(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0,-10}", "Level");
            stringBuilder.Append(" ");

            stringBuilder.AppendFormat("{0,-30}", "Time");
            stringBuilder.Append(" ");

            stringBuilder.AppendFormat("{0,-80}", "Message");
            stringBuilder.AppendLine();

            List<LogMessageViewModel> logMessageViewModels = LogDataGrid.SelectedItems.OfType<LogMessageViewModel>().ToList();
            foreach (LogMessageViewModel logMessageViewModel in logMessageViewModels)
            {
                stringBuilder.AppendFormat("{0,-10}", logMessageViewModel.Level);
                stringBuilder.Append(" ");

                stringBuilder.AppendFormat("{0,-30:yyyy-MM-dd HH:mm:ss.fff}", logMessageViewModel.Time);
                stringBuilder.Append(" ");

                stringBuilder.AppendFormat("{0,-80}", logMessageViewModel.Message);
                stringBuilder.AppendLine();
            }

            string text = stringBuilder.ToString();
            Clipboard.SetText(text);
        }

        private void CanCopyExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LogDataGrid != null && LogDataGrid.SelectedItems != null && LogDataGrid.SelectedItems.Count > 0;
        }

        private void FiltersExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LogRuleFiltersWindow logRuleFiltersWindow = new LogRuleFiltersWindow();
            logRuleFiltersWindow.Owner = Application.Current.MainWindow;
            if (logRuleFiltersWindow.ShowDialog() == true)
            {
                LoadRuleFilters();
            }
        }

        private void CanFiltersExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadRuleFilters()
        {
            LogMessagesViewModel.LoadLogRuleFilters();

            FilterList.DataContext = null;
            FilterList.DataContext = LogMessagesViewModel.LogRuleFilters;
            FilterList.Visibility = LogMessagesViewModel.LogRuleFilters.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
