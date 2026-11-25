using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.ToolBox
{
    public class ToolItemEventArgs : EventArgs
    {
        public ToolItemEventArgs(ToolItem toolItem)
        {
            ToolItem = toolItem;
        }

        public ToolItem ToolItem { get; set; }
    }
}
