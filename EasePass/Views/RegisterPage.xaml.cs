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
using EasePass.Helper.App;
using EasePass.Helper.Database;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
            App.m_window.ShowBackArrow = false;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            if (!passwordBox.Password.Equals(passwordBoxRepeat.Password))
            {
                InfoMessages.PasswordsDoNotMatch();
                return;
            }

            SecureString pw = new SecureString();
            foreach (var character in passwordBox.Password)
            {
                pw.AppendChar(character);
            }

            var dbPath = DefaultSettingsValues.databasePaths;
            Database.LoadedInstance = Database.CreateNewDatabase(dbPath, pw);
            Database.AddDatabasePath(dbPath);

            NavigationHelper.ToPasswords(pw);
        }

        private void passwordBoxRepeat_EnterInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            Register_Click(null, null);
        }

        private void passwordBox_EnterInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            passwordBoxRepeat.Focus(FocusState.Programmatic);
        }

        private async void LoadExistingDatabase_Click(object sender, RoutedEventArgs e)
        {
            var res = await ManageDatabaseHelper.ImportDatabase();
            if (res == null)
                return;

            SettingsManager.SaveSettings(AppSettingsValues.loadedDatabaseName, res.Name);

            NavigationHelper.ToLoginPage();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            passwordBox.Focus(FocusState.Programmatic);
        }

        private async void CreateAdvancedDatabase_Click(object sender, RoutedEventArgs e)
        {
            var db = await new CreateDatabaseDialog().ShowAsync();
            if (db == null)
                return;

            if (db.DatabaseSource is NativeDatabaseSource nds)
                Database.AddDatabasePath(nds.Path);
            Database.LoadedInstance = db;

            //maybe show login page first here?
            NavigationHelper.ToPasswords(db.MasterPassword);
        }
    }
}
