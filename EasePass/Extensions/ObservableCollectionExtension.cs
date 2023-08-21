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
        public static int ByDisplayName(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.DisplayName == null) pmi1.DisplayName = "";
            if (pmi2.DisplayName == null) pmi2.DisplayName = "";
            return pmi1.DisplayName.CompareTo(pmi2.DisplayName);
        }

        public static int ByUsername(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.Username == null) pmi1.Username = "";
            if (pmi2.Username == null) pmi2.Username = "";
            return pmi1.Username.CompareTo(pmi2.Username);
        }

        public static int ByNotes(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.Notes == null) pmi1.Notes = "";
            if (pmi2.Notes == null) pmi2.Notes = "";
            return pmi1.Notes.CompareTo(pmi2.Notes);
        }

        public static int ByWebsite(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.Website == null) pmi1.Website = "";
            if (pmi2.Website == null) pmi2.Website = "";
            return pmi1.Website.CompareTo(pmi2.Website);
        }

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
