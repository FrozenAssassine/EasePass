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
    internal class EnterSecondFactorDialog
    {
        private ContentDialog dialog;
        private EnterSecondFactorPage page;
        public SecureString Token { get; private set; }
        
        public async Task<EnterSecondFactorDialog> ShowAsync()
        {
            page = new EnterSecondFactorPage();
            dialog = new AutoLogoutContentDialog
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
            Token ??= args.Result == ContentDialogResult.Primary ? page.GetPassword().ConvertToSecureString() : null;
        }

        private void Dialog_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Token = page.GetPassword().ConvertToSecureString();
                dialog.Hide();
            }
        }
    }
}
