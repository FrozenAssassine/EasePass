using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Views.DialogPages;

public sealed partial class SelectExportPasswordsDialogPage : Page
{
    private ObservableCollection<PasswordManagerItem> Items = new ObservableCollection<PasswordManagerItem>();

    public SelectExportPasswordsDialogPage(PasswordManagerItem[] items)
    {
        this.InitializeComponent();
        
        foreach(var item in items)
        {
            Items.Add(item);
        }
    }

    public PasswordManagerItem[] GetSelectedItems()
    {
        return Items.Select(x => x.;
    }
}
