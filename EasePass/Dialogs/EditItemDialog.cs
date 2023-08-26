using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class EditItemDialog
    {
        public async Task<PasswordManagerItem> ShowAsync(PasswordsPage.PasswordExists pe, PasswordManagerItem item)
        {
            var page = new AddItemPage(pe, item);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Edit item",
                PrimaryButtonText = "Done",
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
