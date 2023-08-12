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
    internal class AddItemDialog
    {
        public async Task<(PasswordManagerItem pwItem, PasswordManagerCategory category)> ShowAsync(List<PasswordManagerCategory> categories)
        {
            var page = new AddItemPage(categories);
            var dialog = new ContentDialog
            {
                Title = "Add item",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if(await dialog.ShowAsync() == ContentDialogResult.Primary)
                return page.GetValue();
            return (null, null);
        }
    }
}
