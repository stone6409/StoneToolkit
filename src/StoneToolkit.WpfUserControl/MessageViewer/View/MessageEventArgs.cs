using System;
using System.Windows;

namespace StoneToolkit.WpfUserControl.MessageViewer.View
{
    public class MessageEventArgs : RoutedEventArgs
    {
        private DateTime Time { get; set; }

        private string Level { get; set; }

        public string Category { get; set; }

        public string Content { get; set; }

        public MessageEventArgs(DateTime time, string level, string category, string content)
        {
            Time = time;
            Level = level;
            Category = category;
            Content = content;
        }
    }
}
