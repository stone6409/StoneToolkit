using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.WpfShape.Helpers
{
    public static class MathHelper
    {
        public static bool AreClose(double value1, double value2)
        {
            return ((value1 == value2) || IsVerySmall(value1 - value2));
        }

        public static bool IsVerySmall(double value)
        {
            return (Math.Abs(value) < 1E-06);
        }
    }
}
