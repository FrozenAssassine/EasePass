using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Security;
using System.Text;

namespace EasePass.Views
{
    public sealed partial class SettingsPage : Page
    {
        ObservableCollection<PasswordManagerItem> passwordItems = null;
        PasswordsPage passwordsPage = null;
        string SelectedPrinter = "";

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void LoadSettings()
        {
            inactivityLogoutTime.Value = AppSettings.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.inactivityLogoutTime);
            doubleTapToCopySW.IsOn = AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy);
            autoBackupDB.IsOn = AppSettings.GetSettingsAsBool(AppSettingsValues.autoBackupDB, DefaultSettingsValues.autoBackupDatabase);
            autoBackupDBPath.Text = AppSettings.GetSettings(AppSettingsValues.autoBackupDBPath, "");
            autoBackupDBTime.Value = AppSettings.GetSettingsAsInt(AppSettingsValues.autoBackupDBTime, DefaultSettingsValues.autoBackupDBTime);
            showIcons.IsOn = AppSettings.GetSettingsAsBool(AppSettingsValues.showIcons, DefaultSettingsValues.showIcons);
            pswd_chars.Text = AppSettings.GetSettings(AppSettingsValues.passwordChars, DefaultSettingsValues.PasswordChars);
            pswd_length.Text = Convert.ToString(AppSettings.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength));
            disableLeakedPasswords.IsOn = !AppSettings.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords);
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

            printerSelector.Items.Clear();
            foreach(string printer in PrinterSettings.InstalledPrinters)
            {
                printerSelector.Items.Add(printer);
            }
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

            var pickerResult = await FilePickerHelper.PickSaveFile(("Ease Pass Exported Database", new List<string> { ".eped" }));
            if (pickerResult.success)
            {
                File.WriteAllText(pickerResult.path, fileData);
                InfoMessages.ExportDBSuccess();
            }
        }
        private async void ImportEncryptedDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (decryptDBPassword.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".eped" });
            if (!pickerResult.success)
                return;

            EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(pickerResult.path));
            if (!AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, decryptDBPassword.Password))
            {
                InfoMessages.ImportDBWrongPassword();
                return;
            }
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
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (changePW_newPw.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            if (!AuthenticationHelper.VerifyPassword(changePW_currentPw.Password))
            {
                InfoMessages.ChangePasswordWrong();
                return;
            }

            if (!changePW_newPw.Password.Equals(changePW_repeatPw.Password))
            {
                InfoMessages.PasswordsDoNotMatch();
                return;
            }

            SecureString newMasterPw = new SecureString();
            foreach (var character in changePW_newPw.Password)
            {
                newMasterPw.AppendChar(character);
            }

            passwordsPage.masterPassword = newMasterPw;
            AuthenticationHelper.StorePassword(changePW_newPw.Password);
            passwordsPage.SaveData();

            InfoMessages.SuccessfullyChangedPassword();
        }
        private void InactivityLogoutTime_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            AppSettings.SaveSettings(AppSettingsValues.inactivityLogoutTime, inactivityLogoutTime.Value);
        }

        private void DoubleTapToCopySW_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.doubleTapToCopy, doubleTapToCopySW.IsOn);
        }

        private async void PickAutoBackupDBPath_Click(object sender, RoutedEventArgs e)
        {
            var res = await FilePickerHelper.PickSaveFile(("Ease Pass Database", new List<string> { ".epdb" }));
            if (!res.success)
                return;

            autoBackupDBPath.Text = res.path;
        }

        private void AutoBackupDB_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.autoBackupDB, autoBackupDB.IsOn);
        }
        private void AutoBackupDBTime_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            AppSettings.SaveSettings(AppSettingsValues.autoBackupDBTime, autoBackupDBTime.Value);
        }
        private void AutoBackupDBPathTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.autoBackupDBPath, autoBackupDBPath.Text);
        }

        private void showIcons_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.showIcons, showIcons.IsOn);
            if (passwordsPage != null) 
                passwordsPage.Reload();
        }

        private void pswd_length_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string length = pswd_length.Text;
            StringBuilder newLength = new StringBuilder();
            for (int i = 0; i < length.Length; i++)
            {
                if (char.IsDigit(length[i]))
                    newLength.Append(length[i]);
            }
            pswd_length.Text = newLength.ToString();
            pswd_length.SelectionStart = pswd_length.Text.Length;
        }

        private void pswd_length_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pswd_length.Text.Length == 0 || pswd_length.Text == "0")
            {
                AppSettings.SaveSettings(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength);
                return;
            }
            AppSettings.SaveSettings(AppSettingsValues.passwordLength, Math.Max(ConvertHelper.ToInt(pswd_length.Text, DefaultSettingsValues.PasswordLength), 8));
        }

        private void pswd_chars_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.passwordChars, 
                pswd_chars.Text.Length == 0 ? DefaultSettingsValues.PasswordChars : pswd_chars.Text
                );
        }

        private void disableLeakedPasswords_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings(AppSettingsValues.disableLeakedPasswords, !disableLeakedPasswords.IsOn);
        }

        private void printerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPrinter = (string)e.AddedItems[0];
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedPrinter))
            {
                InfoMessages.PrinterNotSelected();
                return;
            }
            PrinterHelper.Print(passwordItems, SelectedPrinter);
        }
    }
}