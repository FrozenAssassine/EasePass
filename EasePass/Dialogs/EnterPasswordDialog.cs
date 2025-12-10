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

using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class EnterPasswordDialog
    {
        private ContentDialog dialog;
        public SecureString Password { get; private set; }
        private EnterPasswordPage page;

        public async Task<EnterPasswordDialog> ShowAsync()
        {
            page = new EnterPasswordPage();
            dialog = new Helper.Logout.AutoLogoutContentDialog
            {
                Title = "Enter password of the database".Localized("Dialogs_EnterPW_Title/Text"),
                PrimaryButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };
            dialog.KeyDown += Dialog_KeyDown;
            dialog.Closing += Dialog_Closing;

            await dialog.ShowAsync();
            return this;
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if(Password == null)
                Password = args.Result == ContentDialogResult.Primary ? page.GetPassword().ConvertToSecureString() : null;
        }

        private void Dialog_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Password = page.GetPassword().ConvertToSecureString();
                dialog.Hide();
            }
        }
    }
}
