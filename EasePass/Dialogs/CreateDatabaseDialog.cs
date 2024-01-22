using EasePass.Helper;
using EasePass.Views;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class CreateDatabaseDialog
    {
        private CreateDatabaseDialogPage page;
        public async Task<bool> ShowAsync()
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

                //Create the database here with the given parameter from page.Evaluate();:
                return true;
            }
            return false;
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
