using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;

namespace EasePass.Dialogs
{
    internal class ChangePasswordDialog
    {
        private ContentDialog dialog;
        private ChangePasswordPage page;

        public async Task ShowAsync(Database db)
        {
            page = new ChangePasswordPage();
            dialog = new AutoLogoutContentDialog
            {
                Title = "Change Password for " + db.Name,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Change",
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
                InfoMessages.ChangePasswordWrong();
            }
            else if (changePWResult == ChangePasswordPageResult.PWNotMatching)
            {
                InfoMessages.PasswordsDoNotMatch();
            }
            else if (changePWResult == ChangePasswordPageResult.PWTooShort)
            {
                InfoMessages.PasswordTooShort();
            }
            args.Cancel = true;
        }

    }
}
