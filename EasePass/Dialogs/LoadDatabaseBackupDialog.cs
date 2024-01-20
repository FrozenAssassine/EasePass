using EasePass.Helper;
using EasePass.Views;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class LoadDatabaseBackupDialog
    {
        public async Task<bool> ShowAsync(DatabaseBackupHelper backupHelper)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Load Backup Database",
                CloseButtonText = "Close",
                XamlRoot = App.m_window.Content.XamlRoot,
            };
            var page = new LoadBackupDatabasePage(dialog);
            dialog.Content = page;

            var res = await dialog.ShowAsync();
            if(res == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                dialog.Hide();
                return await backupHelper.LoadBackupFromFile(page.selectedFile.FilePath);
            }
            return false;
        }
    }
}
