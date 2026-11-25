using StoneToolkit.Common.ComponentModel;

namespace StoneToolkit.WpfUserControl.LogViewer.ViewModel
{
    public class LogRuleFilterViewModel : ObservableObject
    {
        LogRuleFiltersViewModel LogRuleFiltersViewModel { get; set; }

        public LogRuleFilterViewModel(LogRuleFiltersViewModel logRuleFiltersViewModel, string name)
        {
            LogRuleFiltersViewModel = logRuleFiltersViewModel;
            _name = name;
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value && LogRuleFiltersViewModel.IsValidLogRuleFilterName(value))
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private bool _enabled;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                RaisePropertyChanged("Enabled");
            }
        }

        private string _expression;

        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
                RaisePropertyChanged("Expression");
            }
        }

        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
    }
}