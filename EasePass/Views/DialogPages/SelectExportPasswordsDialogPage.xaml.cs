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
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Views.DialogPages;

public sealed partial class SelectExportPasswordsDialogPage : Page
{
    private ObservableCollection<PasswordManagerItem> PWItems;

    public SelectExportPasswordsDialogPage(ObservableCollection<PasswordManagerItem> items)
    {
        this.InitializeComponent();
        this.PWItems = items;
    }

    public PasswordManagerItem[] GetSelectedPasswords()
    {
        return listView.SelectedItems.Select(x => x as PasswordManagerItem).ToArray();
    }

    private void selectAll_Checkbox_Toggled(object sender, RoutedEventArgs e)
    {
        if (selectAll_Checkbox.IsChecked == true)
        {
            listView.SelectAll();
        }
        else
        {
            listView.DeselectAll();
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        listView.SelectAll();
    }

    private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int count = listView.SelectedItems.Count;
        if (sender is ListView item)
        {
            amount.Text = count + " / " + PWItems.Count;
        }

        if (count <= 0)
        {
            selectAll_Checkbox.IsChecked = false;
        }
    }
}
