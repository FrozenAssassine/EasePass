/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using EasePassExtensibility;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;

namespace EasePass.Views
{
    public sealed partial class SettingsPage : Page
    {
        PasswordsPage passwordsPage = null;
        ObservableCollection<PasswordImporterBase> passwordImporter = null;
        bool blockEventsOnloadSettings = false;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void LoadSettings()
        {
            blockEventsOnloadSettings = true;

            inactivityLogoutTime.Value = AppSettings.InactivityLogoutTime;
            doubleTapToCopySW.IsOn = AppSettings.DoubleTapToCopy;
            showIcons.IsOn = AppSettings.ShowIcons;
            pswd_chars.Text = AppSettings.PasswordChars;
            pswd_length.Text = AppSettings.PasswordLength.ToString();
            disableLeakedPasswords.IsOn = !AppSettings.DisableLeakedPasswords;
            clipboardClearTimeout.Value = AppSettings.ClipboardClearTimeoutSec;

            selectLanguageBox.ItemsSource = MainWindow.localizationHelper.languages;

            var languageTag = AppSettings.Language;
            selectLanguageBox.SelectedIndex = MainWindow.localizationHelper.languages.FindIndex(x => x.Tag == languageTag);
            blockEventsOnloadSettings = false;
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
            passwordsPage = navParam.PasswordPage;

            passwordImporter = new ObservableCollection<PasswordImporterBase>();
            foreach (IPasswordImporter importer in ExtensionHelper.GetAllClassesWithInterface<IPasswordImporter>())
            {
                passwordImporter.Add(new PasswordImporterBase(importer));
            }

            if (passwordImporter.Count == 0)
            {
                noPluginsInfo.Visibility = Visibility.Visible;
            }
        }

        private void InactivityLogoutTime_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (blockEventsOnloadSettings)
                return;

            AppSettings.InactivityLogoutTime = (int)inactivityLogoutTime.Value;
        }

        private void DoubleTapToCopySW_Toggled(object sender, RoutedEventArgs e)
        {
            if (blockEventsOnloadSettings)
                return;

            AppSettings.DoubleTapToCopy = doubleTapToCopySW.IsOn;
        }

        private void showIcons_Toggled(object sender, RoutedEventArgs e)
        {
            if (blockEventsOnloadSettings)
                return;
            
            AppSettings.ShowIcons = showIcons.IsOn;
            if (passwordsPage != null)
                passwordsPage.Reload();
        }

        private void pswd_length_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (blockEventsOnloadSettings)
                return;

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
            if (blockEventsOnloadSettings)
                return;

            if (pswd_length.Text.Length == 0 || pswd_length.Text == "0")
            {
                AppSettings.PasswordLength = DefaultSettingsValues.passwordLength;
                return;
            }
            AppSettings.PasswordLength = Math.Max(ConvertHelper.ToInt(pswd_length.Text, DefaultSettingsValues.passwordLength), 8);
        }

        private void pswd_chars_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (blockEventsOnloadSettings)
                return;

            AppSettings.PasswordChars =
                pswd_chars.Text.Length == 0 ? DefaultSettingsValues.passwordChars : pswd_chars.Text;
        }

        private void disableLeakedPasswords_Toggled(object sender, RoutedEventArgs e)
        {
            if (blockEventsOnloadSettings)
                return;

            AppSettings.DisableLeakedPasswords = !disableLeakedPasswords.IsOn;
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
                    for (int j = 0; j < Database.LoadedInstance.Items.Count; j++)
                    {
                        if (Database.LoadedInstance.Items[j].DisplayName.Equals(res.Items[i].DisplayName))
                        {
                            Database.LoadedInstance.Items[j] = res.Items[i];
                            found = true;
                        }
                    }
                    if (!found)
                        Database.LoadedInstance.Items.Add(res.Items[i]);
                }
            }
            else
            {
                for (int i = 0; i < res.Items.Length; i++)
                {
                    Database.LoadedInstance.Items.Add(res.Items[i]);
                }
            }

            Database.LoadedInstance.Save();
        }

        private void ExtensionManage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(ExtensionPage), new SettingsNavigationParameters());
        }

        private void ResetPopularity_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Database.LoadedInstance.Items.Count; i++)
            {
                Database.LoadedInstance.Items[i].Clicks.Clear();
            }
            Database.LoadedInstance.Save();
        }

        private void ManageDatabases_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(ManageDatabasePage));
        }

        private void selectLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (blockEventsOnloadSettings || selectLanguageBox.SelectedItem == null)
                return;

            AppSettings.Language = (selectLanguageBox.SelectedItem as LanguageItem).Tag;

            MainWindow.localizationHelper.SetLanguage(selectLanguageBox.SelectedItem as LanguageItem);
        }

        private void clipboardClearTimeout_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            AppSettings.ClipboardClearTimeoutSec = (int)clipboardClearTimeout.Value;
        }
    }
}