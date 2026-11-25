using StoneToolkit.Common.ComponentModel;
using System;

namespace StoneToolkit.WpfUserControl.MessageViewer.ViewModel
{
    public class MessageViewModel : ObservableObject
    {
        private DateTime _time;
        private string _level;
        private string _category;
        private string _content;

        public MessageViewModel(DateTime time, string level, string category, string content)
        {
            _time = time;
            _level = level;
            _category = category;
            _content = content;
        }

        public DateTime Time
        {
            get
            {
                return _time;
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
                return _level;
            }
        }

        public string LevelText
        {
            get
            {
                if (Level == "Info")
                {
                    return "信息";
                }
                else if (Level == "Warn")
                {
                    return "警告";
                }
                else if (Level == "Error")
                {
                    return "错误";
                }

                return Level;
            }
        }

        public string Category
        {
            get
            {
                return _category;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
        }
    }
}
