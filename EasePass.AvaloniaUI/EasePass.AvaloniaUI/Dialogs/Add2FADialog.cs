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

using EasePass.AvaloniaUI;
using EasePass.AvaloniaUI.Views;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class Add2FADialog
    {
        public async Task<bool> ShowAsync(PasswordManagerItem item)
        {
            var page = new Add2FAPage(item);

            var dialog = new BaseDialog
            {
                Title = $"Add 2FA to {item.DisplayName}",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                Content = page
            };

            MainWindow.current.inactivityHelper.PreventAutologout = true;

            var result = await dialog.ShowDialogAsync(MainWindow.current);

            MainWindow.current.inactivityHelper.PreventAutologout = false;

            if (result == DialogResult.Primary)
            {
                page.UpdateValue();
                return true;
            }

            return false;
        }
    }