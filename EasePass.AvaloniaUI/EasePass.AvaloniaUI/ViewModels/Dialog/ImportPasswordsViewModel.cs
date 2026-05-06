using CommunityToolkit.Mvvm.ComponentModel;
using EasePass.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.ViewModels.Dialog;

public class ImportPasswordItemWrapper : ObservableObject
{
    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public PasswordManagerItem Item { get; }

    public ImportPasswordItemWrapper(PasswordManagerItem item)
    {
        Item = item;
    }
}

public partial class ImportPasswordsViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ImportPasswordItemWrapper> _passwords = new();
    [ObservableProperty] private bool _confirmOverwrite;
    [ObservableProperty] private bool _showOverwriteConfirm;

    public void SetPasswords(ObservableCollection<PasswordManagerItem> items)
    {
        Passwords.Clear();
        foreach (var item in items)
            Passwords.Add(new ImportPasswordItemWrapper(item));
    }

    public void SetPasswords(PasswordManagerItem[] items)
    {
        Passwords.Clear();
        foreach (var item in items)
            Passwords.Add(new ImportPasswordItemWrapper(item));
    }

    public PasswordManagerItem[] GetSelectedPasswords()
    {
        return Passwords.Where(p => p.IsSelected).Select(p => p.Item).ToArray();
    }

    public void SelectAll()
    {
        foreach (var p in Passwords) p.IsSelected = true;
    }

    public void DeselectAll()
    {
        foreach (var p in Passwords) p.IsSelected = false;
    }
}
