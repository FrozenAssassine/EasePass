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
        Database database = null;
        PasswordsPage passwordsPage = null;
        ObservableCollection<PasswordImporterBase> passwordImporter = null;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void LoadSettings()
        {
            inactivityLogoutTime.Value = AppSettings.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.inactivityLogoutTime);
            doubleTapToCopySW.IsOn = AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy);
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
            database = navParam.Database;
            passwordsPage = navParam.PasswordPage;

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


        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (changePW_newPw.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            SecureString pw = new SecureString();
            foreach (var character in changePW_currentPw.Password)
            {
                pw.AppendChar(character);
            }

            try
            {
                new Database(database.Path, pw);
            }
            catch
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

            database.MasterPassword = newMasterPw;
            database.Save();

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
                    for (int j = 0; j < database.Items.Count; j++)
                    {
                        if (database.Items[j].DisplayName.Equals(res.Items[i].DisplayName))
                        {
                            database.Items[j] = res.Items[i];
                            found = true;
                        }
                    }
                    if (!found)
                        database.Items.Add(res.Items[i]);
                }
            }
            else
            {
                for (int i = 0; i < res.Items.Length; i++)
                {
                    database.Items.Add(res.Items[i]);
                }
            }

            database.Save();
        }

        private void ExtensionManage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(ExtensionPage), new SettingsNavigationParameters());
        }

        private void ResetPopularity_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < database.Items.Count; i++)
            {
                database.Items[i].Clicks.Clear();
            }
            database.Save();
        }

        private void ManageDatabases_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(ManageDatabasePage));
        }
    }
}