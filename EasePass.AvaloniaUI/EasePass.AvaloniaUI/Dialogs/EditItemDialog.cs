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
using EasePass.Models;
using EasePass.Services;
using EasePass.ViewModels.Dialog;
using EasePass.Views;
using EasePass.Views.DialogViews;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class EditItemDialog
    {
        public async Task<PasswordManagerItem> ShowAsync(PasswordsPage.PasswordExists pe, PasswordManagerItem item)
        {
            var vm = new AddItemPageViewModel(pe, item);
            var view = new AddItemPage { DataContext = vm };
            var dialog = new Helper.Logout.AutoLogoutContentDialog(true)
            {
                Title = "Edit item".Localized("Dialog_EditItem_Headline/Text"),
                PrimaryButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                Content = view
            };
            var dialogResult = await dialog.ShowAsyncWithPreventAutoLogout();
            if (dialogResult == DialogResult.Primary)
                return vm.GetResult();
            return null;
        }
    }
}
