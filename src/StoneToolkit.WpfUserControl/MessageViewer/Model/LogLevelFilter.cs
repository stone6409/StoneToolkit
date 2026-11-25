using StoneToolkit.Common.ComponentModel;
using System;

namespace StoneToolkit.WpfUserControl.MessageViewer.Model
{
    public class LogLevelFilter : ObservableObject
    {
        private bool _enabled;

        public event EventHandler<EventArgs> EnabledChanged;

        public LogLevelFilter()
        {
        }

        public LogLevelFilter(string level, bool enabled)
        {
            Level = level;
            Enabled = enabled;
        }

        public string Level { get; set; }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (EnabledChanged != null)
                    {
                        EnabledChanged(this, null);
                    }

                    RaisePropertyChanged(nameof(Enabled));
                }
            }
        }

        public bool IsMatch(string level)
        {
            if (Enabled)
            {
                if (Level == "*")
                    return true;

                if (string.Compare(level, Level, true) == 0)
                    return true;
            }

            return false;
        }
    }
}
