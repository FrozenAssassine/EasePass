using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class DeleteConfirmationDialog
    {
        public async Task<bool> ShowAsync(PasswordManagerItem deleteItem)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirm Deletion",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Are you sure you want to delete the item: " + deleteItem.DisplayName + "?"
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
