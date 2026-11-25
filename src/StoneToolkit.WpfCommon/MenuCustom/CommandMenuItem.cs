using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoneToolkit.WpfCommon.MenuCustom
{
    public class CommandMenuItem : MenuItem
    {
        public string Header { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public string InputGestureText { get; set; }

        public virtual List<MenuItem> SubMenuItems
        {
            get
            {
                return null;
            }
        }
    }
}
