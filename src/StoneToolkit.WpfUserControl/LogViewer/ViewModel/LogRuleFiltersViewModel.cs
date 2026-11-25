using StoneToolkit.Common.ComponentModel;
using StoneToolkit.Common.Naming;
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
    public class LogRuleFiltersViewModel : ObservableObject
    {
        private LogFilterProvider _logFilterProvider;

        private ObservableCollection<LogRuleFilterViewModel> _logRuleFilterViewModels;

        public LogRuleFiltersViewModel(LogFilterProvider logFilterProvider)
        {
            _logFilterProvider = logFilterProvider;
        }

        public LogFilterProvider LogRuleFilterProvider
        {
            get
            {
                return _logFilterProvider;
            }
        }

        LogRuleFilterCollection _logRuleFilters;

        public LogRuleFilterCollection LogRuleFilters
        {
            get
            {
                if (_logRuleFilters == null)
                {
                    _logRuleFilters = (LogRuleFilterCollection)LogRuleFilterProvider.CloneLogRuleFilters();
                }

                return _logRuleFilters;
            }
        }

        public ObservableCollection<LogRuleFilterViewModel> LogRuleFilterViewModels
        {
            get
            {
                if (_logRuleFilterViewModels == null)
                {
                    _logRuleFilterViewModels = new ObservableCollection<LogRuleFilterViewModel>();
                    LoadLogRuleFilters();
                }

                return _logRuleFilterViewModels;
            }
        }

        public void LoadLogRuleFilters()
        {
            LogRuleFilterViewModels.Clear();

            if (LogRuleFilters != null)
            {
                foreach (LogRuleFilter logRuleFilter in LogRuleFilters)
                {
                    LogRuleFilterViewModel logRuleFilterViewModel = new LogRuleFilterViewModel(this, logRuleFilter.Name);
                    logRuleFilterViewModel.Enabled = logRuleFilter.Enabled;
                    logRuleFilterViewModel.Expression = logRuleFilter.Expression;
                    logRuleFilterViewModel.Description = logRuleFilter.Description;

                    LogRuleFilterViewModels.Add(logRuleFilterViewModel);
                }
            }
        }

        public bool IsValidLogRuleFilterName(string name)
        {
            return NamingRulesValidator.IsValidIdentifierName(name) && !LogRuleFilters.ContainsName(name);
        }

        public string GenerateValidLogRuleFilterName()
        {
            return NamingRulesValidator.GenerateValidName(IsValidLogRuleFilterName, "Rule");
        }

        public LogRuleFilterViewModel AddLogRuleFilter()
        {
            LogRuleFilter logRuleFilter = new LogRuleFilter();
            logRuleFilter.Name = GenerateValidLogRuleFilterName();
            LogRuleFilters.Add(logRuleFilter);

            LogRuleFilterViewModel logRuleFilterViewModel = new LogRuleFilterViewModel(this, logRuleFilter.Name);
            logRuleFilterViewModel.Enabled = true;
            LogRuleFilterViewModels.Add(logRuleFilterViewModel);

            return logRuleFilterViewModel;
        }

        public LogRuleFilterViewModel InsertLogRuleFilter(int index)
        {
            LogRuleFilter logRuleFilter = new LogRuleFilter();
            logRuleFilter.Name = GenerateValidLogRuleFilterName();
            LogRuleFilters.Add(logRuleFilter);

            LogRuleFilterViewModel logRuleFilterViewModel = new LogRuleFilterViewModel(this, logRuleFilter.Name);
            logRuleFilterViewModel.Name = logRuleFilter.Name;
            logRuleFilterViewModel.Enabled = true;
            LogRuleFilterViewModels.Insert(index, logRuleFilterViewModel);

            return logRuleFilterViewModel;
        }

        public void RemoveLogRuleFilter(LogRuleFilterViewModel logRuleFilterViewModel)
        {
            LogRuleFilters.RemoveByName(logRuleFilterViewModel.Name);
            LogRuleFilterViewModels.Remove(logRuleFilterViewModel);
        }

        public void UpdateLogRuleFilters()
        {
            foreach (LogRuleFilterViewModel logRuleFilterViewModel in LogRuleFilterViewModels)
            {
                LogRuleFilter logRuleFilter = LogRuleFilters.First(f => f.Name == logRuleFilterViewModel.Name);
                logRuleFilter.Enabled = logRuleFilterViewModel.Enabled;
                logRuleFilter.Expression = logRuleFilterViewModel.Expression;
                logRuleFilter.Description = logRuleFilterViewModel.Description;
            }

            LogRuleFilterProvider.UpdateLogRuleFilters(LogRuleFilters);
        }
    }
}
