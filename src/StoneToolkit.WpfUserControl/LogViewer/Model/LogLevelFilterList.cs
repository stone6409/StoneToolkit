using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.LogViewer.Model
{
    public class LogLevelFilterList
    {
        private List<LogLevelFilter> _levelFilters;

        private LogLevelFilter _traceLevelFilter;
        private LogLevelFilter _debugLevelFilter;
        private LogLevelFilter _infoLevelFilter;
        private LogLevelFilter _warnLevelFilter;
        private LogLevelFilter _errorLevelFilter;
        private LogLevelFilter _fatalLevelFilter;

        public LogLevelFilterList()
        {
            _traceLevelFilter = new LogLevelFilter("Trace", true);
            _debugLevelFilter = new LogLevelFilter("Debug", true);
            _infoLevelFilter = new LogLevelFilter("Info", true);
            _warnLevelFilter = new LogLevelFilter("Warn", true);
            _errorLevelFilter = new LogLevelFilter("Error", true);
            _fatalLevelFilter = new LogLevelFilter("Fatal", true);

            LevelFilters.Add(_traceLevelFilter);
            LevelFilters.Add(_debugLevelFilter);
            LevelFilters.Add(_infoLevelFilter);
            LevelFilters.Add(_warnLevelFilter);
            LevelFilters.Add(_errorLevelFilter);
            LevelFilters.Add(_fatalLevelFilter);
        }

        public LogLevelFilter TraceLevelFilter
        {
            get { return _traceLevelFilter; }
        }

        public LogLevelFilter DebugLevelFilter
        {
            get { return _debugLevelFilter; }
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

        public LogLevelFilter FatalLevelFilter
        {
            get { return _fatalLevelFilter; }
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
