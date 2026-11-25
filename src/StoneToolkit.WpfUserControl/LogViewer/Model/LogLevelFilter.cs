using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.LogViewer.Model
{
    public class LogLevelFilter
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
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (EnabledChanged != null)
                        EnabledChanged(this, null);
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
