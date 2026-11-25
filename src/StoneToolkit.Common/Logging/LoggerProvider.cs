using NLog;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Logging
{
    public sealed class LoggerProvider
    {
        private readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>(StringComparer.OrdinalIgnoreCase);

        public LoggerProvider() 
        { 
        }

        private static LoggerProvider _current;

        public static LoggerProvider Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LoggerProvider();
                }

                return _current;
            }
        }


        public Logger CreateLogger(string name)
        {
            Logger logger;

            if (!_loggers.TryGetValue(name, out logger))
            {
                logger = CreateLoggerInternal(name);
                _loggers.TryAdd(name, logger);
            }

            return logger;
        }

        private Logger CreateLoggerInternal(string name)
        {
            NLog.Logger nlogger = NLog.LogManager.GetLogger(name);

            Logger logger = new Logger(nlogger);
            return logger;
        }

        public Logger CreateLogger(string name, string logsFolder, bool onlyThisMethodCallTarget = false)
        {
            Logger logger;

            if (!_loggers.TryGetValue(name, out logger))
            {
                logger = CreateLoggerInternal(name, logsFolder, onlyThisMethodCallTarget);
                _loggers.TryAdd(name, logger);
            }

            return logger;
        }
  
        private Logger CreateLoggerInternal(string name, string logsFolder, bool onlyThisMethodCallTarget = false)
        {
            Logger logger = new Logger();
            logger.LogsFolder = logsFolder;

            LogFactory logFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                if (!onlyThisMethodCallTarget)
                {
                    ColoredConsoleTarget coloredConsoleTarget = new ColoredConsoleTarget();
                    coloredConsoleTarget.Layout = @"${longdate} ${uppercase:${level}} ${message}";
                    builder.Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, coloredConsoleTarget);

                    FileTarget fileTarget = new FileTarget();
                    fileTarget.FileName = Path.Combine(logsFolder, "${shortdate}.log");
                    fileTarget.Layout = "${longdate} ${uppercase:${level}} ${message}";
                    builder.Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);

                    FileTarget exceptionFileTarget = new FileTarget();
                    exceptionFileTarget.FileName = Path.Combine(logsFolder, "exception.${shortdate}.log");
                    exceptionFileTarget.Layout = @"${longdate} ${uppercase:${level}} ${message}
logger：${logger}
stack：${stacktrace:format=DetailedFlat}
exception：${exception:format=ToString}
===============================";

                    builder.Configuration.AddRule(LogLevel.Error, LogLevel.Fatal, exceptionFileTarget);
                }

                ThisMethodCallTarget thisMethodCallTarget = new ThisMethodCallTarget();
                thisMethodCallTarget.ObjectInstance = logger;
                thisMethodCallTarget.MethodName = "LogMethod";
                thisMethodCallTarget.Parameters.Add(new MethodCallParameter("${longdate}"));
                thisMethodCallTarget.Parameters.Add(new MethodCallParameter("${level}"));
                thisMethodCallTarget.Parameters.Add(new MethodCallParameter("${message}"));
                builder.Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, thisMethodCallTarget);
            }).LogFactory;

            NLog.Logger nlogger = logFactory.GetLogger(name);
            logger.NLogger = nlogger;

            return logger;
        }
    }
}
