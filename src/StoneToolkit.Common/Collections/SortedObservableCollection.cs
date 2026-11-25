using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFactoryLibrary.SolutionNodes
{
    public class SortedObservableCollection<T> : ObservableCollection<T> where T : IComparable<T>
    {
        protected override void InsertItem(int index, T item)
        {
            int insertIndex = FindInsertIndex(item);
            base.InsertItem(insertIndex, item);
        }

        private int FindInsertIndex(T item)
        {
            List<T> list = new List<T>(this);
            int index = list.BinarySearch(item);

            if (index < 0)
            {
                index = ~index;
            }

            return index;
        }
    }
}
