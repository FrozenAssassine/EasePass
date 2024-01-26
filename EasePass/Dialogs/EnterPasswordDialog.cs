using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class EnterPasswordDialog
    {
        public async Task<SecureString> ShowAsync()
        {
            var page = new EnterPasswordPage();
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Enter password of the database",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Done",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            var res = await dialog.ShowAsync();

            return res == ContentDialogResult.Primary ? page.GetPassword().ConvertToSecureString() : null;
        }
    }
}
