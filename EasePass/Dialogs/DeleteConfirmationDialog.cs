using EasePass.Extensions;
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
                Title = "Confirm Deletion".Localized("Dialog_ConfirmDelete_Headline/Text"),
                PrimaryButtonText = "Delete".Localized("Dialog_Button_Delete/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = text,
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }

        public async Task<bool> ShowAsync(PasswordManagerItem deleteItem)
        {
            return await Dialog("Are you sure you want to delete the item:".Localized("Dialog_ConfirmDelete_Item/Text") + " " + deleteItem.DisplayName + "?");
        }
        public async Task<bool> ShowAsync(PasswordManagerItem[] deleteItems)
        {
            return await Dialog("Are you sure you want to delete the items:".Localized("Dialog_ConfirmDelete_Items/Text") + "\n" + string.Join(", ", deleteItems.Select(x => x.DisplayName)) + "?");
        }
    }
}
