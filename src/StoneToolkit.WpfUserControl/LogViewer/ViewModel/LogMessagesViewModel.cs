using StoneToolkit.Common.ComponentModel;
using StoneToolkit.WpfUserControl.LogViewer.Model;
using StoneToolkit.WpfUserControl.LogViewer.Provider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.LogViewer.ViewModel
{
    public class LogMessagesViewModel : ObservableObject
    {
        private LogFilterProvider _logFilterProvider;

        public LogMessagesViewModel(LogFilterProvider logFilterProvider)
        {
            _logFilterProvider = logFilterProvider;
        }

        public LogFilterProvider LogFilterProvider
        {
            get
            {
                return _logFilterProvider;
            }
        }

        private ObservableCollection<LogMessageViewModel> _logMessages;

        public ObservableCollection<LogMessageViewModel> LogMessages
        {
            get
            {
                if (_logMessages == null)
                {
                    _logMessages = new ObservableCollection<LogMessageViewModel>();
                }

                return _logMessages;
            }
        }

        private ObservableCollection<LogMessageViewModel> _showLogMessages;

        public ObservableCollection<LogMessageViewModel> ShowLogMessages
        {
            get
            {
                if (_showLogMessages == null)
                {
                    _showLogMessages = new ObservableCollection<LogMessageViewModel>();
                }

                return _showLogMessages;
            }
        }

        public void AddLogMessage(DateTime time, string level, string message)
        {
            LogMessageViewModel logMessageViewModel = new LogMessageViewModel(time, level, message);
            LogMessages.Add(logMessageViewModel);

            if (IsMatchFilter(logMessageViewModel))
            {
                ShowLogMessages.Add(logMessageViewModel);
            }
        }

        public void Clear()
        {
            LogMessages.Clear();
            ShowLogMessages.Clear();
        }

        #region Filter

        LogLevelFilterList _logLevelFilterList;

        public LogLevelFilterList LogLevelFilterList
        {
            get
            {
                if (_logLevelFilterList == null)
                {
                    _logLevelFilterList = LogFilterProvider.LoadLogLevelFilterList();
                    foreach (var levelFilter in _logLevelFilterList.LevelFilters)
                    {
                        levelFilter.EnabledChanged += FilterEnabledChanged;
                    }
                }

                return _logLevelFilterList;
            }
        }

        private void FilterEnabledChanged(object sender, EventArgs eventArgs)
        {
            ReFilterShowLogMessages();
        }

        public bool IsMatchLevelFilter(LogMessageViewModel logMessageViewModel)
        {
            string level = logMessageViewModel.Level;

            return LogLevelFilterList.IsMatch(level);
        }

        LogRuleFilterCollection _logRuleFilters;

        public LogRuleFilterCollection LogRuleFilters
        {
            get
            {
                if (_logRuleFilters == null)
                {
                    LoadLogRuleFilters();
                }

                return _logRuleFilters;
            }
        }

        public void LoadLogRuleFilters()
        {
            _logRuleFilters = LogFilterProvider.LogRuleFilters;

            foreach (LogRuleFilter logRuleFilter in _logRuleFilters)
            {
                logRuleFilter.EnabledChanged += FilterEnabledChanged;
            }

            ReFilterShowLogMessages();
        }

        public bool IsMatchRuleFilter(LogMessageViewModel logMessageViewModel)
        {
            string message = logMessageViewModel.Message;
            int nullResultCount = 0;

            foreach (LogRuleFilter logRuleFilter in LogRuleFilters)
            {
                bool? result = logRuleFilter.IsMatch(message);
                if (result == true)
                {
                    return true;
                }
                else if (result == null)
                {
                    ++nullResultCount;
                }
            }

            if (nullResultCount == LogRuleFilters.Count)
            {
                return true;
            }

            return false;
        }

        public void ReFilterShowLogMessages()
        {
            ShowLogMessages.Clear();

            foreach (var logMessageViewModel in LogMessages)
            {
                if (IsMatchFilter(logMessageViewModel))
                {
                    ShowLogMessages.Add(logMessageViewModel);
                }
            }
        }

        public bool IsMatchFilter(LogMessageViewModel logMessageViewModel)
        {
            return IsMatchLevelFilter(logMessageViewModel) && IsMatchRuleFilter(logMessageViewModel);
        }

        #endregion
    }
}
