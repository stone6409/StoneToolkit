using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Data
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        private T _oldValue;
        private T _newValue;

        public ValueChangedEventArgs(T oldValue, T newValue)
            : base()
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public T OldValue
        {
            get
            {
                return _oldValue;
            }
        }

        public T NewValue
        {
            get
            {
                return _newValue;
            }
        }
    }
}
