using EasePass.Helper;
using EasePass.Views;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class EnterPasswordDialog
    {
        public async Task<string> ShowAsync()
        {
            var page = new EnterPasswordPage();
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Enter password of the database",
                CloseButtonText = "Close",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            var res = await dialog.ShowAsync();

            return res == ContentDialogResult.Primary ? page.GetPassword() : "";
        }
    }
}
