using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class CreateDatabaseDialog
    {
        private CreateDatabaseDialogPage page;
        public async Task<Database> ShowAsync()
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Create Database",
                CloseButtonText = "Close",
                PrimaryButtonText = "Create",
                XamlRoot = App.m_window.Content.XamlRoot,
            };
            page = new CreateDatabaseDialogPage();
            dialog.Content = page;

            dialog.Closing += Dialog_Closing;

            var res = await dialog.ShowAsync();
            if(res == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                var eval = page.Evaluate();
                return Database.CreateEmptyDatabase(Path.Combine(Path.GetDirectoryName(eval.path), eval.databaseName + ".epdb"), eval.masterPassword);
            }
            return null;
        }

        private void Dialog_Closing(Microsoft.UI.Xaml.Controls.ContentDialog sender, Microsoft.UI.Xaml.Controls.ContentDialogClosingEventArgs args)
        {
            if (page == null)
                return;

            //cancel on password mismatch:
            if (!page.PasswordsMatch)
            {
                args.Cancel = true;
            }
        }
    }
}
