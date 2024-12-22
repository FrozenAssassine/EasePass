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

using EasePass.Core.Database;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ChangePasswordDialog
    {
        private ContentDialog dialog;
        private ChangePasswordPage page;

        public async Task ShowAsync(DatabaseItem db)
        {
            page = new ChangePasswordPage();
            dialog = new AutoLogoutContentDialog
            {
                Title = "Change Password for".Localized("Dialog_ChangePassword_Title/Text") + " " + db.Name,
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                PrimaryButtonText = "Change".Localized("Dialog_Button_Change/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };
            dialog.Closing += Dialog_Closing;
            await dialog.ShowAsync();
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result != ContentDialogResult.Primary)
                return;

            var changePWResult = page.ChangePassword();
            if (changePWResult == ChangePasswordPageResult.Success)
            {
                InfoMessages.SuccessfullyChangedPassword();
                dialog.Hide();
                return;
            }
            else if (changePWResult == ChangePasswordPageResult.IncorrectPassword)
            {
                InfoMessages.ChangePasswordWrong(page.InfoMessageParent);
            }
            else if (changePWResult == ChangePasswordPageResult.PWNotMatching)
            {
                InfoMessages.PasswordsDoNotMatch(page.InfoMessageParent);
            }
            else if (changePWResult == ChangePasswordPageResult.PWTooShort)
            {
                InfoMessages.PasswordTooShort(page.InfoMessageParent);
            }
            args.Cancel = true;
        }

    }
}
