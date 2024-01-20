using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using EasePassExtensibility;
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
        Action SavePasswordItems = null;
        string SelectedPrinter = "";
        ObservableCollection<PasswordImporterBase> passwordImporter = null;

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
            passwordItems = navParam.PwItemsManager.PasswordItems;
            passwordsPage = navParam.PasswordPage;
            SavePasswordItems = navParam.SavePwItems;

            printerSelector.Items.Clear();
            foreach(string printer in PrinterSettings.InstalledPrinters)
            {
                printerSelector.Items.Add(printer);
            }

            passwordImporter = new ObservableCollection<PasswordImporterBase>();
            foreach(IPasswordImporter importer in ExtensionHelper.GetAllClassesWithInterface<IPasswordImporter>())
            {
                passwordImporter.Add(new PasswordImporterBase(importer));
            }

            if (passwordImporter.Count == 0)
            {
                noPluginsInfo.Visibility = Visibility.Visible;
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

            PasswordManagerItem[] passwords = await new SelectExportPasswordsDialog().ShowAsync(passwordItems);

            var encodedDB = EncryptDecryptHelper.EncryptStringAES(DatabaseHelper.CreateJsonstring(new ObservableCollection<PasswordManagerItem>(passwords)), encryptDBPassword.Password, salt);

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

            //show dialog to confirm import of selected passwords
            var importPWDialog = new ImportPasswordsDialog();
            importPWDialog.SetPagePasswords(importedItems);
            var dialogResult = await importPWDialog.ShowAsync();

            if (dialogResult.Items == null)
                return;

            if (dialogResult.Override)
            {
                for (int i = 0; i < dialogResult.Items.Length; i++) // I prefer my way with two loops because it will retain the item order.
                {
                    bool found = false;
                    for (int j = 0; j < passwordItems.Count; j++)
                    {
                        if (passwordItems[j].DisplayName.Equals(dialogResult.Items[i].DisplayName))
                        {
                            passwordItems[j] = dialogResult.Items[i];
                            found = true;
                        }
                    }
                    if (!found)
                        passwordItems.Add(dialogResult.Items[i]);
                }
            }
            else
            {
                foreach (var item in dialogResult.Items)
                {
                    passwordItems.Add(item);
                }
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

        private void ExtensionManage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(ExtensionPage), new SettingsNavigationParameters());
        }

        private async void ImportPassword_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.XamlRoot = App.m_window.Content.XamlRoot;

            PasswordImporterBase piBase = (PasswordImporterBase)(sender as Button).Tag;
            var res = await PasswordImportManager.ManageImport(piBase.PasswordImporter);
            
            if (res.Items == null)
                return;

            if (res.Override)
            {
                for (int i = 0; i < res.Items.Length; i++) // I prefer my way with two loops because it will retain the item order.
                {
                    bool found = false;
                    for (int j = 0; j < passwordItems.Count; j++)
                    {
                        if (passwordItems[j].DisplayName.Equals(res.Items[i].DisplayName))
                        {
                            passwordItems[j] = res.Items[i];
                            found = true;
                        }
                    }
                    if (!found)
                        passwordItems.Add(res.Items[i]);
                }
            }
            else
            {
                for (int i = 0; i < res.Items.Length; i++)
                {
                    passwordItems.Add(res.Items[i]);
                }
            }

            SavePasswordItems();
        }

        private void ResetPopularity_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < passwordItems.Count; i++)
            {
                passwordItems[i].Clicks.Clear();
            }
            SavePasswordItems();
        }

        private async void ShowDatabaseBackups_Click(object sender, RoutedEventArgs e)
        {
            await new LoadDatabaseBackupDialog().ShowAsync(MainWindow.databaseBackupHelper);
        }
    }
}