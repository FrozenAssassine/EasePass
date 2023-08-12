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
    internal class EditItemDialog
    {
        public async Task<(PasswordManagerItem pwItem, PasswordManagerCategory category)> ShowAsync(List<PasswordManagerCategory> categories, PasswordManagerItem item)
        {
            var page = new AddItemPage(categories, item);
            var dialog = new ContentDialog
            {
                Title = "Edit item",
                PrimaryButtonText = "Done",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return page.GetValue();
            return (null, null);
        }
    }
}
