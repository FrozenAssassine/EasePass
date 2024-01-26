using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class Add2FADialog
    {
        public async Task<bool> ShowAsync(PasswordManagerItem item)
        {
            var page = new Add2FAPage(item);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Add 2FA to".Localized("Dialog_Add2FA_Headline/Text") + " " + item.DisplayName,
                PrimaryButtonText = "Add".Localized("Dialog_Button_Add/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            bool res = await dialog.ShowAsync() == ContentDialogResult.Primary;
            if (res)
                page.UpdateValue();
            return res;
        }
    }
}
