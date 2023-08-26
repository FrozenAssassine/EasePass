using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class InsertOrOverwriteDatabaseDialog
    {
        public enum Result
        {
            Insert, Overwrite, Cancel
        }
        public async Task<Result> ShowAsync()
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Insert or Overwrite data",
                PrimaryButtonText = "Insert",
                SecondaryButtonText = "Overwrite",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Would you prefer to overwrite the existing data with the imported information, or would you like to insert the new data into the existing records?"
            };

            var res = await dialog.ShowAsync();
            if (res == ContentDialogResult.Primary)
                return Result.Insert;
            else if(res == ContentDialogResult.Secondary)
                return Result.Overwrite;
            return Result.Cancel;
        }
    }
}
