using EasePass.Dialogs;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EasePass.Views;

public sealed partial class ImportPasswordsDialogPage : Page
{
    ObservableCollection<PasswordManagerItem> Items = new ObservableCollection<PasswordManagerItem>();
    bool[] Checked = null;

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
        // Do not override "Items" with a new ObservableCollection. It would destroy the binding in GUI.
        Checked = new bool[items.Count];
        Items.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            Items.Add(items[i]);
            Checked[i] = true;
        }

        progress.Visibility = Visibility.Collapsed;
    }
    public void SetPasswords(PasswordManagerItem[] items)
    {
        // Do not override "Items" with a new ObservableCollection. It would destroy the binding in GUI.
        Items.Clear();
        Checked = new bool[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            Items.Add(items[i]);
            Checked[i] = true;
        }

        progress.Visibility = Visibility.Collapsed;
    }

    public PasswordManagerItem[] GetSelectedPasswords()
    {
        return Items.Where((x, i) => Checked[i] == true).ToArray();
    }

    private void CheckBox_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.Tag is PasswordManagerItem item && item != null)
            Checked[Items.IndexOf(item)] = cb.IsChecked ?? false;
    }
    public (bool result, CheckBox confirmOverwriteCheckbox) GetConfirmOverwriteState()
    {
        return (confirmOverwritePasswords_Checkbox.IsChecked ?? false, confirmOverwritePasswords_Checkbox);
    }
    public void ShowConfirmOverWriteDatabase()
    {
        confirmOverwritePasswords_Checkbox.Visibility = Visibility.Visible;
    }
}
