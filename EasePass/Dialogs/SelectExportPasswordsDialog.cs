using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class SelectExportPasswordsDialog
    {
        public async Task<PasswordManagerItem[]> ShowAsync(ObservableCollection<PasswordManagerItem> items)
        {
            var page = new SelectExportPasswordsDialogPage(items);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Select items to export",
                PrimaryButtonText = "Export",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return page.GetSelectedPasswords();
            return null;
        }
    }
}
