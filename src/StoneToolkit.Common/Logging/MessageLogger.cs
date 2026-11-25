using StoneToolkit.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Logging
{
    public static class MessageLogger
    {
        private static Logger _logger;

        public static Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    // example: C:\Users\stone\AppData\Roaming\ChatAI\logs\messages
                    string folder = FilePathUtility.GetApplicationDataFolder();
                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = FilePathUtility.GetEntryAssemblyPath();
                    }
                    folder = Path.Combine(folder, "logs", "messages");

                    _logger = LoggerProvider.Current.CreateLogger("MessageLogger", folder, false);
                }

                return _logger;
            }
        }

        private static string FormatMessage(string category, string content)
        {
            return $"#CATEGORY#: {category}, #CONTENT#: {content}";
        }

        public static bool ParseMessage(string message, out string category, out string content)
        {
            string pattern = @"^#CATEGORY#: (.+), #CONTENT#: (.+)$";
            Match match = Regex.Match(message, pattern);

            if (match.Success)
            {
                category = match.Groups[1].Value;
                content = match.Groups[2].Value;
                return true;
            }

            category = null;
            content = null;
            return false;
        }

        public static void Info(string category, string content)
        {
            string message = FormatMessage(category, content);
            Logger.Info(message);
        }

        public static void Warn(string category, string content)
        {
            string message = FormatMessage(category, content);
            Logger.Warn(message);
        }

        public static void Error(string category, string content)
        {
            string message = FormatMessage(category, content);
            Logger.Error(message);
        }
    }
}
