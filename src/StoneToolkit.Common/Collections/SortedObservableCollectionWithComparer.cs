using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Collections
{
    public class SortedObservableCollectionWithComparer<T> : ObservableCollection<T> where T : IComparable<T>
    {
        private readonly IComparer<T> _comparer;

        public SortedObservableCollectionWithComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        protected override void InsertItem(int index, T item)
        {
            int insertIndex = FindInsertIndex(item);
            base.InsertItem(insertIndex, item);
        }

        private int FindInsertIndex(T item)
        {
            var list = new List<T>(this);
            int index = list.BinarySearch(item, _comparer);

            if (index < 0)
            {
                index = ~index;
            }

            return index;
        }
    }
}
