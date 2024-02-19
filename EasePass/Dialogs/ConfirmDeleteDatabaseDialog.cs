using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ConfirmDeleteDatabaseDialog
    {
        public async Task<bool> ShowAsync(Database database)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Confirm delete Database",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Close".Localized("Dialog_Button_Close/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Confirm to delete Database:\n" + database.Name + "\n" + database.Path,
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
