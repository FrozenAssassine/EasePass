using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using EasePassExtensibility;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    public class ImportDialog
    {
        private ImportPage page;

        public async Task<(PasswordManagerItem[] Items, bool Override)> ShowAsync(MsgType msg = MsgType.None)
        {
            page = new ImportPage();
            SetPageMessage(msg);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Import passwords",
                PrimaryButtonText = "Add",
                SecondaryButtonText = "Override",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            var res = await dialog.ShowAsync();
            PasswordManagerItem[] items = page.GetSelectedPasswords();

            if (res == ContentDialogResult.Primary)
                return (items, false);
            if (res == ContentDialogResult.Secondary)
                return (items, true);
            return (new PasswordManagerItem[0], false);
        }

        public void SetPageMessage(MsgType msg)
        {
            page.SetMessage(msg);
        }

        public void SetPagePasswords(PasswordManagerItem[] items)
        {
            page.SetPasswords(items);
        }

        public enum MsgType
        {
            None,
            Error,
            NoPasswords
        }
    }
}
