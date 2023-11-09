using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using EasePassExtensibility;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ImportDialog
    {
        public async Task<(PasswordItem[] Items, bool Override)> ShowAsync(IPasswordImporter importer)
        {
            var page = new ImportPage(importer);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Import passwords",
                PrimaryButtonText = "Add",
                SecondaryButtonText = "Override",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            var res = await dialog.ShowAsync();
            PasswordItem[] items = page.GetSelectedPasswords();

            if (res == ContentDialogResult.Primary)
                return (items, false);
            if (res == ContentDialogResult.Secondary)
                return (items, true);
            return (new PasswordItem[0], false);
        }
    }
}
