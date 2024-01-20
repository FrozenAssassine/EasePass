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
    ObservableCollection<PasswordManagerItemCheck> Items = new ObservableCollection<PasswordManagerItemCheck>();
    //bool[] Checked = null;

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
        //Checked = new bool[items.Count];
        Items.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            Items.Add(new PasswordManagerItemCheck(items[i]));
            //Checked[i] = true;
        }

        selectAll_Checkbox.Visibility = Visibility.Visible;
        progress.Visibility = Visibility.Collapsed;
    }
    public void SetPasswords(PasswordManagerItem[] items)
    {
        // Do not override "Items" with a new ObservableCollection. It would destroy the binding in GUI.
        Items.Clear();
        //Checked = new bool[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            Items.Add(new PasswordManagerItemCheck(items[i]));
            //Checked[i] = true;
        }

        selectAll_Checkbox.Visibility = Visibility.Visible;
        progress.Visibility = Visibility.Collapsed;
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
    public (bool result, CheckBox confirmOverwriteCheckbox) GetConfirmOverwriteState()
    {
        return (confirmOverwritePasswords_Checkbox.IsChecked ?? false, confirmOverwritePasswords_Checkbox);
    }
    public void ShowConfirmOverWriteDatabase()
    {
        confirmOverwritePasswords_Checkbox.Visibility = Visibility.Visible;
    }

    private void selectAll_Checkbox_Checked(object sender, RoutedEventArgs e)
    {
        for(int i = 0; i < Items.Count; i++)
        {
            Items[i].Checked = true;
        }
    }

    private void selectAll_Checkbox_Unchecked(object sender, RoutedEventArgs e)
    {
        for(int i = 0; i < Items.Count; i++)
        {
            Items[i].Checked = false;
        }
    }
}

internal class PasswordManagerItemCheck : INotifyPropertyChanged
{
    private bool check { get; set; } = true;
    public bool Checked
    {
        get => check;
        set
        {
            check = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Checked"));
        }
    }
    public PasswordManagerItem Item = null;

    public event PropertyChangedEventHandler PropertyChanged;

    public PasswordManagerItemCheck(PasswordManagerItem item)
    {
        this.Item = item;
    }
}