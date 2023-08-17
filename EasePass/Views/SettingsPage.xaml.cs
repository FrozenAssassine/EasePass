using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class SettingsPage : Page
    {
        ObservableCollection<PasswordManagerItem> passwordItems = null;
        PasswordsPage passwordsPage = null;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void LoadSettings()
        {
            inactivityLogoutTime.Value = AppSettings.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.InactivityLogoutTime);
            doubleTapToCopySW.IsOn = AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.m_window.ShowBackArrow = false;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadSettings();

            App.m_window.ShowBackArrow = true;
            var navParam = e.Parameter as SettingsNavigationParameters;
            passwordItems = navParam.PwItems;
            passwordsPage = navParam.PasswordPage;
        }

        private async void ExportEncryptedDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (encryptDBPassword.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

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
            if(decryptDBPassword.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            var pickerResult = await FilePickerHelper.PickOpenFile(new string[] {".eped" });
            if (pickerResult.success)
            {
                EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(pickerResult.path));
                if (AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, decryptDBPassword.Password))
                {
                    var str = EncryptDecryptHelper.DecryptStringAES(decryptedJson.Data, decryptDBPassword.Password, decryptedJson.Salt);
                    var importedItems = DatabaseHelper.LoadItems(str);

                    var dialogResult = await new InsertOrOverwriteDatabaseDialog().ShowAsync();
                    if (dialogResult == InsertOrOverwriteDatabaseDialog.Result.Cancel)
                        return;

                    if (dialogResult == InsertOrOverwriteDatabaseDialog.Result.Overwrite)
                    {
                        passwordItems.Clear();
                    }

                    foreach (var item in importedItems)
                    {
                        passwordItems.Add(item);
                    }
                    passwordsPage.SaveData();
                    InfoMessages.ImportDBSuccess();
                    return;
                }
                InfoMessages.ExportDBWrongPassword();
            }
        }
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (changePW_newPw.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            if (AuthenticationHelper.VerifyPassword(changePW_currentPw.Password))
            {
                if (changePW_newPw.Password.Equals(changePW_repeatPw.Password))
                {
                    SecureString newMasterPw = new SecureString();
                    foreach(var character in changePW_newPw.Password)
                    {
                        newMasterPw.AppendChar(character);
                    }

                    passwordsPage.masterPassword = newMasterPw;
                    AuthenticationHelper.StorePassword(changePW_newPw.Password);
                    passwordsPage.SaveData();

                    InfoMessages.SuccessfullyChangedPassword();
                    return;
                }
                InfoMessages.PasswordsDoNotMatch();
                return;
            }
            InfoMessages.ChangePasswordWrong();
        }
        private void InactivityLogoutTime_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            AppSettings.SaveSettings(AppSettingsValues.inactivityLogoutTime, inactivityLogoutTime.Value);
        }

        private void doubleTapToCopySW_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.doubleTapToCopy, doubleTapToCopySW.IsOn);
        }
    }
}