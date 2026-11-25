using StoneToolkit.Common.ComponentModel;

namespace StoneToolkit.WpfUserControl.ToolBox
{
    public class ToolItem : ObservableObject
    {
        public ToolItem(string name, string displayName, string category)
        {
            Name = name;
            DisplayName = displayName;
            Category = category;
        }

        public ToolItem(string name, string displayName, string category, string imageName)
        {
            Name = name;
            DisplayName = displayName;
            Category = category;
            ImageName = imageName;
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _displayName;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }

        private string _imageName;

        public string ImageName
        {
            get
            {
                // 没有指定名称时，使用名字作为图片
                if (_imageName == null)
                {
                    return Name;
                }
                return _imageName;
            }
            set
            {
                _imageName = value;
                RaisePropertyChanged("ImageName");
            }
        }

        private string _category;

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                RaisePropertyChanged("Category");
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }
    }
}
