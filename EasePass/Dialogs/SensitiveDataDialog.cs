using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class SensitiveDataDialog
    {
        public async Task<bool> Dialog(Extension extension)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Warning!",
                PrimaryButtonText = "Allow",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "This plugin tries to get access to sensitive information:" + Environment.NewLine + extension.ToString(false),
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
