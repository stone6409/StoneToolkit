using StoneToolkit.Common.Utilities;
using StoneToolkit.WpfUserControl.LogViewer.ConfigureFile;
using StoneToolkit.WpfUserControl.LogViewer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.LogViewer.Provider
{
    public class LogFilterProvider
    {
        public LogFilterProvider(string fileName)
        {
            LogRuleFilterConfigureFile = new LogFilterConfigureFile(fileName, true);
        }

        private static LogFilterProvider _default;

        public static LogFilterProvider Default
        {
            get
            {
                if (_default == null)
                {
                    string applictionProjectFolder = FilePathUtility.CurrentApplictionDataFolder;
                    string fileName = Path.Combine(applictionProjectFolder, "LogFilterData.xml");
                    _default = new LogFilterProvider(fileName);
                }

                return _default;
            }
        }

        public LogFilterConfigureFile LogRuleFilterConfigureFile { get; private set; }

        public LogLevelFilterList LoadLogLevelFilterList()
        {
            return LogRuleFilterConfigureFile.LoadLogLevelFilterList();
        }

        private LogRuleFilterCollection _logRuleFilters;

        public LogRuleFilterCollection LogRuleFilters
        {
            get
            {
                if (_logRuleFilters == null)
                {
                    _logRuleFilters = LogRuleFilterConfigureFile.LoadLogRuleFilters();
                }

                return _logRuleFilters;
            }
            private set
            {
                _logRuleFilters = null;
            }
        }

        public LogRuleFilterCollection CloneLogRuleFilters()
        {
            LogRuleFilterCollection logRuleFilters = new LogRuleFilterCollection();
            foreach (LogRuleFilter logRuleFilter in LogRuleFilters)
            {
                logRuleFilters.Add(logRuleFilter.Clone());
            }

            return logRuleFilters;
        }

        public LogRuleFilter GetLogRuleFilter(string name)
        {
            return LogRuleFilters.First(f => f.Name == name);
        }

        public void UpdateLogRuleFilters(LogRuleFilterCollection logRuleFilters)
        {
            LogRuleFilterConfigureFile.UpdateLogRuleFilters(logRuleFilters);

            LogRuleFilters = null;
        }
    }
}
