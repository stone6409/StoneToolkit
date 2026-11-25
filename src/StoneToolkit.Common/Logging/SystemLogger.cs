using StoneToolkit.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Logging
{
    public static class SystemLogger
    {
        private static Logger _logger;

        public static Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    // example: C:\Users\stone\AppData\Roaming\ChatAI
                    string folder = FilePathUtility.GetApplicationDataFolder();
                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = FilePathUtility.GetEntryAssemblyPath();
                    }
                    folder = Path.Combine(folder, "logs");

                    _logger = LoggerProvider.Current.CreateLogger("SystemLogger", folder);
                }

                return _logger;
            }
        }

        public static string LogsFolder
        {
            get
            {
                return Logger.LogsFolder;
            }
        }

        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        public static void Trace(string format, params object[] args)
        {
            Logger.Trace(format, args);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Debug(string format, params object[] args)
        {
            Logger.Debug(format, args);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Info(string format, params object[] args)
        {
            Logger.Info(format, args);
        }

        public static void Warn(string message)
        {
            Logger.Warn(message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Error(string message, Exception exception)
        {
            Logger.Error(message, exception);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }
    }
}
