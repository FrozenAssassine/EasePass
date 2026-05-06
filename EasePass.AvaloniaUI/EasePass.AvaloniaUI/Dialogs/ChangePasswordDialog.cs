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
using EasePass.Services;
using EasePass.ViewModels.Dialog;
using EasePass.Views;
using EasePass.Views.DialogViews;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ChangePasswordDialog
    {
        private BaseDialog dialog;
        private ChangePasswordViewModel _vm;

        public async Task ShowAsync(DatabaseItem db)
        {
            _vm = new ChangePasswordViewModel();
            var page = new ChangePasswordPage { DataContext = _vm };

            dialog = new Helper.Logout.AutoLogoutContentDialog
            {
                Title = "Change Password for".Localized("Dialog_ChangePassword_Title/Text") + " " + db.Name,
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                PrimaryButtonText = "Change".Localized("Dialog_Button_Change/Text"),
                Content = page
            };
            dialog.Closing += Dialog_Closing;
            await dialog.ShowOnMainView();
        }

        private async void Dialog_Closing(object? sender, BaseDialogClosingArgs args)
        {
            if (args.Result != DialogResult.Primary)
                return;

            args.Cancel = true; // prevent close until validated

            ChangePasswordPageResult result = await _vm.ChangePassword();
            switch (result)
            {
                case ChangePasswordPageResult.Success:
                    InfoMessages.SuccessfullyChangedPassword();
                    dialog.Close(DialogResult.Cancel); // close after success
                    break;
                case ChangePasswordPageResult.IncorrectPassword:
                    InfoMessages.ChangePasswordWrong();
                    break;
                case ChangePasswordPageResult.PWNotMatching:
                    InfoMessages.PasswordsDoNotMatch();
                    break;
                case ChangePasswordPageResult.PWTooShort:
                    InfoMessages.PasswordTooShort();
                    break;
            }
        }
    }
}