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

using EasePass.Extensions;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

internal class RenameDatabaseDialog
{
    public async Task<bool> ShowAsync(/*Database databaseItem*/)
    {
        TextBox dbName = new TextBox
        {
            PlaceholderText = "Database name".Localized("Dialog_RenameDB_Name/Text"),
            Text = "", //databaseItem.Name
        };

        var dialog = new AutoLogoutContentDialog
        {
            Title = "Rename Database".Localized("Dialog_RenameDB_Headline/Text"),
            PrimaryButtonText = "Rename".Localized("Dialog_Button_Rename/Text"),
            CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
            XamlRoot = App.m_window.Content.XamlRoot,
            Content = dbName
        };

        return (await dialog.ShowAsync() == ContentDialogResult.Primary && dbName.Text.Length > 0);
    }
}
