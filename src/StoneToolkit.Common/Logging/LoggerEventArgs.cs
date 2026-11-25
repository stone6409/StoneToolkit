using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Logging
{
    public class LoggerEventArgs : EventArgs
    {
        private DateTime _time;
        private string _level;
        private string _message;

        public LoggerEventArgs(DateTime time, string level, string message)
        {
            _time = time;
            _level = level;
            _message = message;
        }

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public string Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public override string ToString()
        {
            return string.Format("Time: {0}, Level: {1}, Message: {2}", Time, Level, Message);
        }
    }
}
