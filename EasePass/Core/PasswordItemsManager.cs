using EasePass.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Core;

public class PasswordItemsManager
{
    public ObservableCollection<PasswordManagerItem> PasswordItems = null;

    public void Unload()
    {
        //GC will automatically take care of it.
        PasswordItems = null;
    }
    public void Load(ObservableCollection<PasswordManagerItem> items)
    {
        PasswordItems = items;
    }

    public ObservableCollection<PasswordManagerItem> FindItemsByName(string name)
    {
        return new ObservableCollection<PasswordManagerItem>(PasswordItems.Where(x => x.DisplayName.Contains(name, System.StringComparison.OrdinalIgnoreCase)));
    }

    public void DeleteItem(PasswordManagerItem item)
    {
        PasswordItems.Remove(item);
    }

    public void AddItem(PasswordManagerItem item)
    {
        PasswordItems.Add(item);
    }

    public int PasswordAlreadyExists(string password)
    {
        return PasswordItems.Count(x => x.Password == password);
    }
}
