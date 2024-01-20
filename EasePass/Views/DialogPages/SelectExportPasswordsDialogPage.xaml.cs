using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Views.DialogPages;

public sealed partial class SelectExportPasswordsDialogPage : Page
{
    private ObservableCollection<PasswordManagerItemCheck> Items = new ObservableCollection<PasswordManagerItemCheck>();

    public SelectExportPasswordsDialogPage(ObservableCollection<PasswordManagerItem> items)
    {
        this.InitializeComponent();
        
        foreach(var item in items)
        {
            Items.Add(new PasswordManagerItemCheck(item));
        }
    }

    public PasswordManagerItem[] GetSelectedPasswords()
    {
        return Items.Where((x, i) => x.Checked == true).Select(x => x.Item).ToArray();
    }

    private void CheckBox_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.Tag is PasswordManagerItemCheck item && item != null)
            item.Checked = cb.IsChecked ?? false;
    }

    private void selectAll_Checkbox_Toggled(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Checked = (sender as CheckBox).IsChecked ?? false;
        }
    }

}
