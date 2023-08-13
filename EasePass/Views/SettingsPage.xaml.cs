using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EasePass.Views
{
    public sealed partial class SettingsPage : Page
    {
        ObservableCollection<PasswordManagerItem> passwordItems = null;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.m_window.ShowBackArrow = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.m_window.ShowBackArrow = true;
            passwordItems = e.Parameter as ObservableCollection<PasswordManagerItem>;
        }


        private async void ExportEncryptedDatabase_Click(object sender, RoutedEventArgs e)
        {
            var (hash, salt) = AuthenticationHelper.HashPassword(encryptDBPassword.Password);

            var encodedDB = EncryptDecryptHelper.EncryptStringAES(DatabaseHelper.CreateJsonstring(passwordItems), encryptDBPassword.Password, salt);

            var encryptedItem = new EncryptedDatabaseItem(hash, salt, encodedDB);
            string fileData = JsonConvert.SerializeObject(encryptedItem, Formatting.Indented);

            var pickerResult = await FilePickerHelper.PickSaveFile(("Ease Pass Encrypted Database", new List<string> { ".eped" }));
            if(pickerResult.success)
            {
                File.WriteAllText(pickerResult.path, fileData);
                InfoMessages.ExportDBSuccess();
            }
        }
        private async void ImportEncryptedDatabase_Click(object sender, RoutedEventArgs e)
        {
            var pickerResult = await FilePickerHelper.PickOpenFile(new string[] {".eped" });
            if (pickerResult.success)
            {
                EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(pickerResult.path));
                if (AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, decryptDBPassword.Password))
                {
                    var str = EncryptDecryptHelper.DecryptStringAES(decryptedJson.Data, decryptDBPassword.Password, decryptedJson.Salt);
                    var importedItems = DatabaseHelper.LoadItems(str);
                    
                    var dialogResult = await new InsertOrOverwriteDatabaseDialog().ShowAsync();
                    if(dialogResult == InsertOrOverwriteDatabaseDialog.InsertOverwriteDialogResult.Insert)
                    {
                        foreach(var item in importedItems)
                        {
                            passwordItems.Add(item);
                        }
                        InfoMessages.ImportDBSuccess();
                    }
                    else if (dialogResult == InsertOrOverwriteDatabaseDialog.InsertOverwriteDialogResult.Overwrite)
                    {
                        passwordItems = importedItems;
                        InfoMessages.ImportDBSuccess();
                    }
                    return;
                }
                InfoMessages.ExportDBWrongPassword();
            }
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.GoBack();
        }
    }
}
