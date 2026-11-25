using StoneToolkit.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfCommon.TreeViewCustom
{
    public class TreeViewNode : ObservableObject
    {
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
                    if (!_isSelected)
                    {
                        IsInEditMode = false;
                    }

                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }

        private bool _isInEditMode;

        public bool IsInEditMode
        {
            get
            {
                return _isInEditMode;
            }
            set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;
                    RaisePropertyChanged(nameof(IsInEditMode));
                }
            }
        }

        bool _isExpanded = false;

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(nameof(IsExpanded));
            }
        }
    }
}
