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

using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class LoginPage : Page
    {
        int WrongCount = 0;

        public LoginPage()
        {
            this.InitializeComponent();

            passwordBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            App.m_window.ShowBackArrow = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ManageDatabaseHelper.LoadDatabasesToCombobox(databasebox);
            TemporaryDatabaseHelper.HandleImportTempDatabase(e, databasebox);

            await DailyTipHelper.ShowDailyTip(dailyTipTextBlock, dailyTipGrid);
        }

        private async void PWLogin_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (WrongCount > 2)
            {
                InfoMessages.TooManyPasswordAttempts();
                return;
            }

            SecureString pw = passwordBox.Password.ConvertToSecureString();
            
            if (databasebox.SelectedItem == null)
                return;

            DatabaseItem selectedDB = (databasebox.SelectedItem as DatabaseItem);
            var res = await selectedDB.CheckPasswordCorrect(pw);
            if (res.result == PasswordValidationResult.WrongPassword)
            {
                WrongCount++;
                InfoMessages.EnteredWrongPassword(WrongCount);
                return;
            }
            else if(res.result == PasswordValidationResult.DatabaseNotFound)
            {
                InfoMessages.DatabaseFileNotFoundAt(selectedDB.Path);
                return;
            }
            WrongCount = 0;

            await selectedDB.Load(pw);
            NavigationHelper.ToPasswords();
            return;
        }
        private void Enter_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            PWLogin_Click(null, null);
        }

        private async void CreateDatabase_Click(SplitButton sender, SplitButtonClickEventArgs args)
        {
            DatabaseItem newDB = await ManageDatabaseHelper.CreateDatabase();
            if (newDB == null)
                return;

            databasebox.Items.Add(newDB);
            databasebox.SelectedItem = newDB;
        }

        private void databasebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (databasebox.SelectedItem == null)
                return;

            AppSettings.LoadedDatabaseName = (databasebox.SelectedItem as DatabaseItem).Name;
        }

        private async void ImportDatabase_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DatabaseItem res = await ManageDatabaseHelper.ImportDatabase();
            if (res == null)
                return;

            databasebox.Items.Add(res);
            databasebox.SelectedItem = res;
        }

        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            passwordBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }
    }
}
