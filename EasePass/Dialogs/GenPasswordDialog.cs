using EasePass.Models;
using EasePass.Core;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class GenPasswordDialog
    {
        public async Task<bool> ShowAsync()
        {
            GenPasswordPage page = new GenPasswordPage();
            page.GeneratePassword();
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Password generator".Localized("Dialog_PWGenerator_New"),
                PrimaryButtonText = "New".Localized("Dialog_Button_New/Text"),
                CloseButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
