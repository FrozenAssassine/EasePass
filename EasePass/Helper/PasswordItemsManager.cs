using EasePass.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Helper
{
    internal class PasswordItemsManager
    {
        public static ObservableCollection<PasswordManagerItem> FindItemsByName(ObservableCollection<PasswordManagerItem> pwItems, string name)
        {
            return new ObservableCollection<PasswordManagerItem>(pwItems.Where(x => x.DisplayName.Contains(name, System.StringComparison.OrdinalIgnoreCase)));
        }

        public static void DeleteItem(ObservableCollection<PasswordManagerItem> pwItems, PasswordManagerItem item)
        {
            pwItems.Remove(item);
        }

        public static void AddItem(ObservableCollection<PasswordManagerItem> pwItems, PasswordManagerItem item)
        {
            pwItems.Add(item);
        }
    }
}
