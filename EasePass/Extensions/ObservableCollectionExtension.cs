using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void SortAlphabetic(this ObservableCollection<PasswordManagerItem> collection)
        {
            collection.Sort((PasswordManagerItem pmi1, PasswordManagerItem pmi2) => pmi1.DisplayName.CompareTo(pmi2.DisplayName));
        }
    }
}
