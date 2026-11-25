using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Logging
{
    public class Logger
    {
        private NLog.Logger _nLogger;
        public event EventHandler<LoggerEventArgs> Logged;

        public Logger()
        {
        }

        public Logger(NLog.Logger nLogger)
        {
            _nLogger = nLogger;
        }

        public NLog.Logger NLogger
        {
            get
            {
                return _nLogger;
            }
            internal set
            {
                _nLogger = value;
            }
        }

        public string LogsFolder { get; internal set; }

        public void Trace(string message)
        {
            _nLogger.Trace(message);
        }

        public void Trace(string format, params object[] args)
        {
            _nLogger.Trace(format, args);
        }

        public void Debug(string message)
        {
            _nLogger.Debug(message);
        }

        public void Debug(string format, params object[] args)
        {
            _nLogger.Debug(format, args);
        }

        public void Info(string message)
        {
            _nLogger.Info(message);
        }

        public void Info(string format, params object[] args)
        {
            _nLogger.Info(format, args);
        }

        public void Warn(string message)
        {
            _nLogger.Warn(message);
        }

        public void Error(string message)
        {
            _nLogger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _nLogger.Error(exception, message);
        }

        public void Fatal(string message)
        {
            _nLogger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            _nLogger.Fatal(message, exception);
        }

        public void LogMethod(string longdate, string level, string message)
        {
            if (Logged != null)
            {
                DateTime dateTime = DateTime.Parse(longdate);
                Logged(this, new LoggerEventArgs(dateTime, level, message));
            }
        }
    }
}
