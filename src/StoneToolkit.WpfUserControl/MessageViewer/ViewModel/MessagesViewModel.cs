using StoneToolkit.Common.ComponentModel;
using StoneToolkit.Common.Logging;
using StoneToolkit.WpfUserControl.MessageViewer.Model;
using System;
using System.Collections.ObjectModel;

namespace StoneToolkit.WpfUserControl.MessageViewer.ViewModel
{
    public class MessagesViewModel : ObservableObject
    {
        public MessagesViewModel()
        {
        }

        private ObservableCollection<MessageViewModel> _messages;

        public ObservableCollection<MessageViewModel> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new ObservableCollection<MessageViewModel>();
                }

                return _messages;
            }
        }

        private ObservableCollection<MessageViewModel> _showMessages;

        public ObservableCollection<MessageViewModel> ShowMessages
        {
            get
            {
                if (_showMessages == null)
                {
                    _showMessages = new ObservableCollection<MessageViewModel>();
                }

                return _showMessages;
            }
        }

        public void AddMessage(DateTime time, string level, string message)
        {
            string category;
            string content;
            if (MessageLogger.ParseMessage(message, out category, out content))
            {
                MessageViewModel messageViewModel = new MessageViewModel(time, level, category, content);
                Messages.Add(messageViewModel);

                if (IsMatchFilter(messageViewModel))
                {
                    ShowMessages.Add(messageViewModel);
                }
            }
        }

        public void Clear()
        {
            Messages.Clear();
            ShowMessages.Clear();
        }

        #region Filter

        LogLevelFilterList _logLevelFilterList;

        public LogLevelFilterList LogLevelFilterList
        {
            get
            {
                if (_logLevelFilterList == null)
                {
                    _logLevelFilterList = new LogLevelFilterList();
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

        public bool IsMatchLevelFilter(MessageViewModel messageViewModel)
        {
            string level = messageViewModel.Level;

            return LogLevelFilterList.IsMatch(level);
        }

        public bool IsMatchRuleFilter(MessageViewModel messageViewModel)
        {
            return true;
        }

        public void ReFilterShowLogMessages()
        {
            ShowMessages.Clear();

            foreach (MessageViewModel messageViewModel in Messages)
            {
                if (IsMatchFilter(messageViewModel))
                {
                    ShowMessages.Add(messageViewModel);
                }
            }
        }

        public bool IsMatchFilter(MessageViewModel messageViewModel)
        {
            return IsMatchLevelFilter(messageViewModel) && IsMatchRuleFilter(messageViewModel);
        }

        #endregion
    }
}
