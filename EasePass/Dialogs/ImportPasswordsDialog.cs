using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    public class ImportPasswordsDialog
    {
        private readonly ImportPasswordsDialogPage importPage;

        public ImportPasswordsDialog()
        {
            importPage = new ImportPasswordsDialogPage();
        }

        public async Task<(PasswordManagerItem[] Items, bool Override)> ShowAsync(bool showProgressbar, MsgType msg = MsgType.None)
        {
            SetPageMessage(msg, showProgressbar);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Import passwords",
                PrimaryButtonText = "Add",
                SecondaryButtonText = "Override",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = importPage
            };

            //to add confirmation on overwrite passwords:
            dialog.Closing += Dialog_Closing;

            var res = await dialog.ShowAsync();
            PasswordManagerItem[] items = importPage.GetSelectedPasswords();

            if (res == ContentDialogResult.Primary)
                return (items, false);
            if (res == ContentDialogResult.Secondary)
                return (items, true);
            return (null, false);
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            //ensure overwrite button was pressed:
            if (args.Result != ContentDialogResult.Secondary)
                return;

            importPage.ShowConfirmOverWriteDatabase();

            var overwriteState = importPage.GetConfirmOverwriteState();
            //allow overwrite
            if (overwriteState.result)
                return;

            overwriteState.confirmOverwriteCheckbox.BorderBrush = new SolidColorBrush(Colors.Red);
            args.Cancel = true;
        }

        public void SetPageMessage(MsgType msg, bool showProgressbar = false)
        {
            importPage.SetMessage(msg, showProgressbar);
        }

        public void SetPagePasswords(PasswordManagerItem[] items)
        {
            importPage.SetPasswords(items);
        }
        public void SetPagePasswords(ObservableCollection<PasswordManagerItem> items)
        {
            importPage.SetPasswords(items);
        }

        public enum MsgType
        {
            None,
            Error,
            NoPasswords
        }
    }
}
