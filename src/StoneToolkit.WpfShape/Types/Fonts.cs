using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfShape.Types
{
    public static class Fonts
    {
        static Font _fefault;

        public static Font Default 
        { 
            get
            {
                if (_fefault == null)
                {
                    _fefault = new Font();
                }

                return _fefault;
            }
        }
    }
}
