using EasePass.Core;
using EasePass.Helper;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class GenPasswordDialog
    {
        public async Task<bool> ShowAsync(PasswordItemsManager pwItemsManager)
        {
            GenPasswordPage page = new GenPasswordPage(pwItemsManager);
            page.GeneratePassword();
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Password generator",
                PrimaryButtonText = "New",
                CloseButtonText = "Done",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
