using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class DeleteExtensionConfirmationDialog
    {
        public async Task<bool> ShowAsync(Extension deleteItem)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Confirm Deletion".Localized("Dialog_ConfirmDelete_Headline/Text"),
                PrimaryButtonText = "Delete".Localized("Dialog_Button_Delete/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Are you sure you want to delete the extension:".Localized("Dialog_ConfirmDelete_Extension/Text") + "" + deleteItem.AboutPlugin.PluginName + "?"
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
