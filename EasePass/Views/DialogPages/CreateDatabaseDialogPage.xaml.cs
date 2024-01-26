using EasePass.Extensions;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class CreateDatabaseDialogPage : Page
    {
        private string DatabaseOutputLocation = "";

        public CreateDatabaseDialogPage()
        {
            this.InitializeComponent();
        }

        public (string path, SecureString masterPassword, string databaseName) Evaluate()
        {
            return (DatabaseOutputLocation, passwordBox.Password.ConvertToSecureString(), databaseName.Text.Length == 0 ? "Database" : databaseName.Text);
        }

        public bool PasswordsMatch => passwordBoxRepeat.Password == passwordBox.Password;

        private void databaseName_TextChanged(object sender, TextChangedEventArgs e)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string text = databaseName.Text;
            int selectionStart = databaseName.SelectionStart;
            for (int i = 0; i < invalidChars.Length; i++)
            {
                text = text.Replace("" + invalidChars[i], "");
            }
            databaseName.Text = text;
            databaseName.SelectionStart = Math.Min(text.Length, selectionStart);
        }

        private async void selectPath_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var pickerRes = await FilePickerHelper.PickFolder();
            if (!pickerRes.success)
                return;

            databasePath.Text = pickerRes.path;
            DatabaseOutputLocation = pickerRes.path;
        }
    }
}
