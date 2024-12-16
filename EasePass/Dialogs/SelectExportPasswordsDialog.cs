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
using EasePass.Models;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class SelectExportPasswordsDialog
    {
        public async Task<PasswordManagerItem[]> ShowAsync(ObservableCollection<PasswordManagerItem> items)
        {
            var page = new SelectExportPasswordsDialogPage(items);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Select items to export".Localized("Dialog_Export_Headline/Text"),
                PrimaryButtonText = "Export".Localized("Dialog_Button_Export/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return page.GetSelectedPasswords();
            return null;
        }
    }
}
