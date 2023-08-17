using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class Add2FADialog
    {
        public async Task<PasswordManagerItem> ShowAsync(PasswordManagerItem item)
        {
            var page = new Add2FAPage(item);
            var dialog = new ContentDialog
            {
                Title = "Add 2FA to " + item.DisplayName,
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return page.GetValue();
            return null;
        }
    }
}
