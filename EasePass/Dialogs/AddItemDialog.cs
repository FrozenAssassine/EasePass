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
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class AddItemDialog
    {
        public async Task<PasswordManagerItem> ShowAsync(PasswordsPage.PasswordExists pe)
        {
            var page = new AddItemPage(pe);
            var dialog = new AutoLogoutContentDialog(true)
            {
                Title = "Add Password".Localized("Dialog_AddItem_Headline/Text"),
                PrimaryButtonText = "Add".Localized("Dialog_Button_Add/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            MainWindow.CurrentInstance.inactivityHelper.PreventAutologout = true;
            var dialogRes = await dialog.ShowAsync();
            MainWindow.CurrentInstance.inactivityHelper.PreventAutologout = false;
            if (dialogRes == ContentDialogResult.Primary)
            {
                return page.GetValue();
            }
            return null;
        }
    }
}
