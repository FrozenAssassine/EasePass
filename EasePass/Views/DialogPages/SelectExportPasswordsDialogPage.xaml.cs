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
        
        if(items != null)
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
