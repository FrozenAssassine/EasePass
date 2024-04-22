using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Extensions
{
    internal static class ObservableCollectionExtension
    {


        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static ObservableCollection<T> ReverseSelf<T>(this ObservableCollection<T> collection)
        {
            return new ObservableCollection<T>(collection.Reverse());
        }
    }
}
