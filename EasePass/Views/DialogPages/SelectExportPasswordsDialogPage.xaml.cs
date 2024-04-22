using CommunityToolkit.WinUI;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Views.DialogPages;

public sealed partial class SelectExportPasswordsDialogPage : Page
{
    List<PasswordManagerItem> PWItems = new();

    public SelectExportPasswordsDialogPage(ObservableCollection<PasswordManagerItem> items)
    {
        this.InitializeComponent();

        PWItems.Clear();
        PWItems.AddRange(items);

        listView.ItemsSource = PWItems;

        listView.SelectAll();
    }

    public PasswordManagerItem[] GetSelectedPasswords()
    {
        return listView.SelectedItems.Select(x => x as PasswordManagerItem).ToArray();
    }

    private void selectAll_Checkbox_Toggled(object sender, RoutedEventArgs e)
    {
        if (selectAll_Checkbox.IsChecked ?? false)
            listView.SelectAll();
        else
            listView.DeselectAll();
    }

}
