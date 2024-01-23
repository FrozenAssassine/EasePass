using EasePass.Extensions;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

internal class RenameDatabaseDialog
{
    public async Task<bool> ShowAsync(/*Database databaseItem*/)
    {
        TextBox dbName = new TextBox
        {
            PlaceholderText = "Database name".Localized("Dialog_RenameDB_Name/Text"),
            Text = "", //databaseItem.Name
        };

        var dialog = new AutoLogoutContentDialog
        {
            Title = "Rename Database".Localized("Dialog_RenameDB_Headline/Text"),
            PrimaryButtonText = "Rename".Localized("Dialog_Button_Rename/Text"),
            CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
            XamlRoot = App.m_window.Content.XamlRoot,
            Content = dbName
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary && dbName.Text.Length > 0)
        {
            //databaseItem.Name = dbName.Text;
            return true;
        }
        return false;
    }
}
