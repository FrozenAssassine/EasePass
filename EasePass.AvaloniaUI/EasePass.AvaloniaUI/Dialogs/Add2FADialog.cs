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

using EasePass.Models;
using EasePass.Helper.Logout;
using EasePass.Services;
using EasePass.Views.DialogViews;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

internal class Add2FADialog
{
    public async Task<bool> ShowAsync(PasswordManagerItem item)
    {
        var page = new Add2FAPage(item);

        var dialog = new AutoLogoutContentDialog()
        {
            Title = $"Add 2FA to {item.DisplayName}",
            PrimaryButtonText = "Add",
            CloseButtonText = "Cancel",
            Content = page
        };

        var result = await dialog.ShowAsyncWithPreventAutoLogout();
        if (result == DialogResult.Primary)
        {
            page.UpdateItem();
            return true;
        }

        return false;
    }
}