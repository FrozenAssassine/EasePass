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
using EasePass.Views;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class Export2FADialog
    {
        public async Task ShowAsync(string qrcode)
        {
            var page = new Export2FAPage(qrcode);
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Export 2FA".Localized("Dialog_Export2FA_Headline/Text"),
                PrimaryButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            await dialog.ShowAsync();
        }
    }
}
