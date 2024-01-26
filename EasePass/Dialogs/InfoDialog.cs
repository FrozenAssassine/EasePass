using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class InfoDialog
    {
        public async Task<ContentDialogResult> ShowAsync(string info, string extensionName)
        {
            var page = new TextInfoPage(info);
            var dialog = new AutoLogoutContentDialog
            {
                Title = extensionName + " " + "info".Localized("Dialog_Info_Headline/Text"),
                CloseButtonText = "Close".Localized("Dialog_Button_Close/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            return await dialog.ShowAsync();
        }
    }
}
