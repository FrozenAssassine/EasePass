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
        private ContentDialog dialog;
        public SecureString Password { get; private set; }
        private EnterPasswordPage page;

        public async Task<EnterPasswordDialog> ShowAsync()
        {
            page = new EnterPasswordPage();
            dialog = new AutoLogoutContentDialog
            {
                Title = "Enter password of the database",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Done",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };
            dialog.KeyDown += Dialog_KeyDown;
            dialog.Closing += Dialog_Closing;

            await dialog.ShowAsync();
            return this;
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if(Password == null)
                Password = args.Result == ContentDialogResult.Primary ? page.GetPassword().ConvertToSecureString() : null;
        }

        private void Dialog_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Password = page.GetPassword().ConvertToSecureString();
                dialog.Hide();
            }
        }
    }
}
