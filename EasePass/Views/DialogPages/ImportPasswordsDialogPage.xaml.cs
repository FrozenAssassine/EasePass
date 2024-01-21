using CommunityToolkit.WinUI;
using EasePass.Dialogs;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Views;

public sealed partial class ImportPasswordsDialogPage : Page
{
    List<PasswordManagerItem> PWItems = new();

    public ImportPasswordsDialogPage()
    {
        this.InitializeComponent();
    }

    public void SetMessage(ImportPasswordsDialog.MsgType msg)
    {
        switch (msg)
        {
            case ImportPasswordsDialog.MsgType.None:
                progress.Visibility = Visibility.Visible;
                break;
            case ImportPasswordsDialog.MsgType.Error:
                progress.Visibility = Visibility.Collapsed;
                errorMsg.Text = "An error has occured!";
                errorMsg.Visibility = Visibility.Visible;
                break;
            case ImportPasswordsDialog.MsgType.NoPasswords:
                progress.Visibility = Visibility.Collapsed;
                errorMsg.Text = "No passwords available!";
                errorMsg.Visibility = Visibility.Visible;
                break;
        }
    }

    public void SetPasswords(ObservableCollection<PasswordManagerItem> items)
    {
        PWItems.Clear();      
        PWItems.AddRange(items);

        selectAll_Checkbox.Visibility = Visibility.Visible;
        progress.Visibility = Visibility.Collapsed;

        listView.SelectAll();
    }
    public void SetPasswords(PasswordManagerItem[] items)
    {
        PWItems.Clear();
        PWItems.AddRange(items);

        selectAll_Checkbox.Visibility = Visibility.Visible;
        progress.Visibility = Visibility.Collapsed;

        listView.SelectAll();
    }

    public PasswordManagerItem[] GetSelectedPasswords()
    {
        return listView.SelectedItems.Select(x => x as PasswordManagerItem).ToArray();
    }

    private void CheckBox_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.Tag is PasswordManagerItemCheck item && item != null)
            item.Checked = cb.IsChecked ?? false;
    }
    public (bool result, CheckBox confirmOverwriteCheckbox) GetConfirmOverwriteState()
    {
        return (confirmOverwritePasswords_Checkbox.IsChecked ?? false, confirmOverwritePasswords_Checkbox);
    }
    public void ShowConfirmOverWriteDatabase()
    {
        confirmOverwritePasswords_Checkbox.Visibility = Visibility.Visible;
    }

    private void selectAll_Checkbox_Toggled(object sender, RoutedEventArgs e)
    {
        if (selectAll_Checkbox.IsChecked ?? false)
            listView.SelectAll();
        else
            listView.DeselectAll();
    }
}