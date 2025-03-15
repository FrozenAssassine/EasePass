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
using EasePass.Extensions;
using EasePass.Helper.FileSystem;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class CreateDatabaseDialogPage : Page
    {
        private string databaseOutputLocation = "";
        public Panel InfoMessageParent => infoMessageParent;

        public CreateDatabaseDialogPage()
        {
            this.InitializeComponent();
        }
        private string CreateDatabaseName()
        {
            return databaseName.Text.Length == 0 ? "Database" : databaseName.Text;
        }

        private string CreateDatabasePath()
        {
            var path = Path.Combine(databaseOutputLocation, CreateDatabaseName() + ".epdb");
            
            return path;
        }

        public (string path, SecureString masterPassword) Evaluate()
        {
            return (CreateDatabasePath(), passwordBox.Password.ConvertToSecureString());
        }

        public bool PasswordsMatch => passwordBoxRepeat.Password == passwordBox.Password;
        public bool PathValid => databaseOutputLocation.Length > 0;
        public string DatabasePath => databaseOutputLocation;
        public bool PasswordLengthCorrect => passwordBox.Password.Length > 3;
        public bool PathAlreadyExists => File.Exists(CreateDatabasePath());
        
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
            databaseOutputLocation = pickerRes.path;

            if(PathValid && PathAlreadyExists)
            {
                InfoMessages.DatabaseWithThatNameAlreadyExists(infoMessageParent);
            }
        }
    }
}
