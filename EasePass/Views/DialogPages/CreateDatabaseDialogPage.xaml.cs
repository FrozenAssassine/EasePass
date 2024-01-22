using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System.Security;
using System;
using Windows.Storage;
using EasePass.Extensions;

namespace EasePass.Views
{
    public sealed partial class CreateDatabaseDialogPage : Page
    {
        private string DatabaseOutputLocation = "";
        private SecureString MasterPassword;

        public CreateDatabaseDialogPage()
        {
            this.InitializeComponent();
        }

        public (string path, SecureString masterPassword, string databaseName) Evaluate()
        {
            return (DatabaseOutputLocation, passwordBox.Password.ConvertToSecureString(), databaseName.Text.Length == 0 ? "Database" : databaseName.Text);
        }

        public bool PasswordsMatch => passwordBoxRepeat.Password == passwordBox.Password;

        private async void Choose_DatabaseLocation_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var pickerRes = await FilePickerHelper.PickOpenFile(new string[] { ".epdb" });
            if (!pickerRes.success)
                return;

            DatabaseOutputLocation = pickerRes.path;
        }


        private async void Default_DatabaseLocation_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var databaseFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backups", CreationCollisionOption.OpenIfExists);

            DatabaseOutputLocation = databaseFolder.Path;
        }
    }
}
