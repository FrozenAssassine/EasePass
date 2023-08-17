using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class GenPasswordDialog
    {
        public async Task<string> ShowAsync()
        {
            var page = new GenPasswordPage();
            var dialog = new ContentDialog
            {
                Title = "Password generator",
                PrimaryButtonText = "Done",
                CloseButtonText = "New",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return null;
            else return "x";
        }
    }
}
