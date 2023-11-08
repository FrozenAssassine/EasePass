using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class DeleteExtensionConfirmationDialog
    {
        public async Task<bool> ShowAsync(Extension deleteItem)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Confirm Deletion",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Are you sure you want to delete the extension: " + deleteItem.AboutPlugin.PluginName + "?"
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
