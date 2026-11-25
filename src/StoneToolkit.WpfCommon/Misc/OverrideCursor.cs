using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoneToolkit.WpfCommon.Misc
{
    public class OverrideCursor : IDisposable
    {
        static Stack<Cursor> _Stack = new Stack<Cursor>();

        public OverrideCursor(Cursor changeToCursor)
        {
            _Stack.Push(changeToCursor);

            if (Mouse.OverrideCursor != changeToCursor)
                Mouse.OverrideCursor = changeToCursor;
        }

        public void Dispose()
        {
            _Stack.Pop();

            Cursor cursor = _Stack.Count > 0 ? _Stack.Peek() : null;

            if (cursor != Mouse.OverrideCursor)
                Mouse.OverrideCursor = cursor;
        }
    }
}
