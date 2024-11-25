﻿using EasePass.Extensions;
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
