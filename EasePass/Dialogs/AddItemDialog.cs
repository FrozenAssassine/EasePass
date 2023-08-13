using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class AddItemDialog
    {
        public async Task<PasswordManagerItem> ShowAsync(ObservableCollection<PasswordManagerItem> pwItems)
        {
            var page = new AddItemPage(pwItems);
            var dialog = new ContentDialog
            {
                Title = "Add Password",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            if(await dialog.ShowAsync() == ContentDialogResult.Primary)
                return page.GetValue();
            return null;
        }
    }
}
