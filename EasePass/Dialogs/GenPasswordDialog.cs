using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class GenPasswordDialog
    {
        public async Task<bool> ShowAsync()
        {
            var dialog = new ContentDialog
            {
                Title = "Password generator",
                PrimaryButtonText = "New",
                CloseButtonText = "Done",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = new GenPasswordPage()
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
