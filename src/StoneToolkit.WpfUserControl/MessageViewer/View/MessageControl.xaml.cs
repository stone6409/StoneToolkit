using StoneToolkit.Common.Logging;
using StoneToolkit.WpfUserControl.MessageViewer.ViewModel;
using StoneToolkit.WpfCommon.DataGridCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace StoneToolkit.WpfUserControl.MessageViewer.View
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        private static MessagesViewModel _messagesViewModel;

        private static MessagesViewModel MessagesViewModel
        {
            get
            {
                return _messagesViewModel;
            }
        }

        #region MessageDoubleClick Event

        public static readonly RoutedEvent MessageDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "MessageDoubleClick", RoutingStrategy.Bubble, typeof(MessageEventHandler), typeof(MessageControl));

        public event MessageEventHandler MessageDoubleClick
        {
            add
            {
                AddHandler(MessageDoubleClickEvent, value);
            }
            remove
            {
                RemoveHandler(MessageDoubleClickEvent, value);
            }
        }

        private void OnMessageDoubleClick(DateTime time, string level, string category, string content)
        {
            MessageEventArgs messageEventArgs = new MessageEventArgs(time, level, category, content);
            messageEventArgs.RoutedEvent = MessageDoubleClickEvent;
            messageEventArgs.Source = this;

            RaiseEvent(messageEventArgs);
        }

        #endregion

        public static void InitMessagesViewModel()
        {
            if (_messagesViewModel == null)
            {
                _messagesViewModel = new MessagesViewModel();

                MessageLogger.Logger.Logged += new EventHandler<LoggerEventArgs>((sender, e) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<LoggerEventArgs>((loggerEventArgs) =>
                    {
                        _messagesViewModel.AddMessage(loggerEventArgs.Time, loggerEventArgs.Level, loggerEventArgs.Message);
                    }), e);
                });
            }
        }

        public MessageControl()
        {
            InitializeComponent();
        }

        private bool _hasLoaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_hasLoaded)
            {
                return;
            }
            _hasLoaded = true;

            DataContext = MessagesViewModel;
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
            DataGridCustomStyles.Sorting(dataGrid, e);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageViewModel messageViewModel = dataGrid.SelectedItem as MessageViewModel;
            if (messageViewModel != null)
            {
                OnMessageDoubleClick(messageViewModel.Time, messageViewModel.Level, messageViewModel.Category, messageViewModel.Content);
            }
        }

        public static RoutedUICommand ClearCommand = new RoutedUICommand();

        private void ClearExecute(object sender, ExecutedRoutedEventArgs e)
        {
            MessagesViewModel.Clear();
            dataGrid.Focus();
        }

        private void CanClearExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dataGrid != null && dataGrid.HasItems;
        }

        public static RoutedUICommand CopyCommand = new RoutedUICommand();

        private void CopyExecute(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0,-10}", "Level");
            stringBuilder.Append(" ");

            stringBuilder.AppendFormat("{0,-30}", "Time");
            stringBuilder.Append(" ");

            stringBuilder.AppendFormat("{0,-30}", "Category");
            stringBuilder.Append(" ");

            stringBuilder.AppendFormat("{0,-80}", "Content");
            stringBuilder.AppendLine();

            List<MessageViewModel> messageViewModels = dataGrid.SelectedItems.OfType<MessageViewModel>().ToList();
            foreach (MessageViewModel messageViewModel in messageViewModels)
            {
                stringBuilder.AppendFormat("{0,-10}", messageViewModel.LevelText);
                stringBuilder.Append(" ");

                stringBuilder.AppendFormat("{0,-30:yyyy-MM-dd HH:mm:ss.fff}", messageViewModel.Time);
                stringBuilder.Append(" ");

                stringBuilder.AppendFormat("{0,-30}", messageViewModel.Category);
                stringBuilder.Append(" ");

                stringBuilder.AppendFormat("{0,-80}", messageViewModel.Content);
                stringBuilder.AppendLine();
            }

            string text = stringBuilder.ToString();
            Clipboard.SetText(text);
        }

        private void CanCopyExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelected;
        }

        public bool IsSelected
        {
            get
            {
                return dataGrid != null && dataGrid.SelectedItems != null && dataGrid.SelectedItems.Count > 0;
            }
        }
    }
}
