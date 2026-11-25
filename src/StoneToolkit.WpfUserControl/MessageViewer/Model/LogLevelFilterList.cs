using System.Collections.Generic;

namespace StoneToolkit.WpfUserControl.MessageViewer.Model
{
    public class LogLevelFilterList
    {
        private List<LogLevelFilter> _levelFilters;

        private LogLevelFilter _infoLevelFilter;
        private LogLevelFilter _warnLevelFilter;
        private LogLevelFilter _errorLevelFilter;

        public LogLevelFilterList()
        {
            _infoLevelFilter = new LogLevelFilter("Info", true);
            _warnLevelFilter = new LogLevelFilter("Warn", true);
            _errorLevelFilter = new LogLevelFilter("Error", true);

            LevelFilters.Add(_infoLevelFilter);
            LevelFilters.Add(_warnLevelFilter);
            LevelFilters.Add(_errorLevelFilter);
        }

        public LogLevelFilter InfoLevelFilter
        {
            get { return _infoLevelFilter; }
        }

        public LogLevelFilter WarnLevelFilter
        {
            get { return _warnLevelFilter; }
        }

        public LogLevelFilter ErrorLevelFilter
        {
            get { return _errorLevelFilter; }
        }

        public List<LogLevelFilter> LevelFilters
        {
            get
            {
                if (_levelFilters == null)
                    _levelFilters = new List<LogLevelFilter>();

                return _levelFilters;
            }
        }

        public bool IsMatch(string level)
        {
            foreach (LogLevelFilter levelFilter in LevelFilters)
            {
                if (levelFilter.IsMatch(level))
                    return true;
            }

            return false;
        }
    }
}
