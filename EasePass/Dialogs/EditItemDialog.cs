﻿using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class EditItemDialog
    {
        public async Task<PasswordManagerItem> ShowAsync(PasswordsPage.PasswordExists pe, PasswordManagerItem item)
        {
            var page = new AddItemPage(pe, item);
            var dialog = new AutoLogoutContentDialog(true)
            {
                Title = "Edit item".Localized("Dialog_EditItem_Headline/Text"),
                PrimaryButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };
            MainWindow.CurrentInstance.inactivityHelper.PreventAutologout = true;
            var dialogResult = await dialog.ShowAsync();
            MainWindow.CurrentInstance.inactivityHelper.PreventAutologout = false;
            
            if (dialogResult == ContentDialogResult.Primary)
                return page.GetValue();
            return null;
        }
    }
}
