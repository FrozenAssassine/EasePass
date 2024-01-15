using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Title = extensionName + " info",
                CloseButtonText = "Close",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            return await dialog.ShowAsync();
        }
    }
}
