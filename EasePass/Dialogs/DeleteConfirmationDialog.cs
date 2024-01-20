using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class DeleteConfirmationDialog
    {
        public async Task<bool> Dialog(string text)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Confirm Deletion",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = text,
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }

        public async Task<bool> ShowAsync(PasswordManagerItem deleteItem)
        {
            return await Dialog("Are you sure you want to delete the item: " + deleteItem.DisplayName + "?");
        }
        public async Task<bool> ShowAsync(PasswordManagerItem[] deleteItems)
        {
            return await Dialog("Are you sure you want to delete the items: \n" + string.Join(", ", deleteItems.Select(x => x.DisplayName)) + "?");
        }
    }
}
