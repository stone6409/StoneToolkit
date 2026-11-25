using System;
using System.Collections.Generic;
using System.Text;

namespace StoneToolkit.Common.Utilities
{
    public static class EnumHelper
    {
        public static bool TryGetEnumValue<T>(string value, out T result) where T : IConvertible
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                result = (T)Enum.Parse(typeof(T), value);
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
