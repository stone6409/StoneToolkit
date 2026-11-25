using StoneToolkit.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.LogViewer.ViewModel
{
    public class LogMessageViewModel : ObservableObject
    {
        private DateTime _time;
        private string _level;
        private string _message;

        public LogMessageViewModel(DateTime time, string level, string message)
        {
            _time = time;
            _level = level;
            _message = message;
        }

        public DateTime Time
        {
            get
            {
                return this._time;
            }
        }

        public string TimeText
        {
            get
            {
                return _time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
        }

        public string Level
        {
            get
            {
                return this._level;
            }
        }

        public string Message
        {
            get
            {
                return this._message;
            }
        }
    }
}
