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

using Avalonia.Controls;
using EasePass.AvaloniaUI;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Services;
using EasePass.ViewModels;
using EasePass.ViewModels.Dialog;
using EasePass.Views;
using EasePass.Views.DialogViews;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class AddItemDialog
    {
        public async Task<PasswordManagerItem> ShowAsync(PasswordsPage.PasswordExists pe)
        {
            var vm = new AddItemPageViewModel(pe);
            var view = new AddItemPage { DataContext = vm };
            var dialog = new Helper.Logout.AutoLogoutContentDialog(true)
            {
                Title = "Add Password".Localized("Dialog_AddItem_Headline/Text"),
                PrimaryButtonText = "Add".Localized("Dialog_Button_Add/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                Content = view
            };

            var result = await dialog.ShowAsyncWithPreventAutoLogout();
            if (result == DialogResult.Primary)
                return vm.GetResult();
            return null;
        }
    }
}
