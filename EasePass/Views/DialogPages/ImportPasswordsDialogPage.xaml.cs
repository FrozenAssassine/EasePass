/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using CommunityToolkit.WinUI;
using EasePass.Dialogs;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Views;

public sealed partial class ImportPasswordsDialogPage : Page
{
    ObservableCollection<PasswordManagerItem> PWItems = new();

    public ImportPasswordsDialogPage()
    {
        this.InitializeComponent();
    }

    public void SetMessage(ImportPasswordsDialog.MsgType msg, bool showProgressbar = false)
    {
        switch (msg)
        {
            case ImportPasswordsDialog.MsgType.None:
                if (showProgressbar)
                    progress.Visibility = Visibility.Visible;
                break;
            case ImportPasswordsDialog.MsgType.Error:
                progress.Visibility = Visibility.Collapsed;
                errorMsg.Text = "An error has occurred!";
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
        if (items == null)
            return;

        PWItems.Clear();
        for (int i = 0; i < items.Count; i++)
            PWItems.Add(items[i]);

        listView.UpdateLayout();

        selectAll_Checkbox.Visibility = Visibility.Visible;
        progress.Visibility = Visibility.Collapsed;

        listView.SelectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(0, (uint)listView.Items.Count));
    }
    public void SetPasswords(PasswordManagerItem[] items)
    {
        PWItems.Clear();
        for (int i = 0; i < items.Length; i++)
            PWItems.Add(items[i]);

        listView.UpdateLayout();

        selectAll_Checkbox.Visibility = Visibility.Visible;
        progress.Visibility = Visibility.Collapsed;

        listView.SelectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(0, (uint)listView.Items.Count));
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