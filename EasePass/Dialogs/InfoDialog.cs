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
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class InfoDialog
    {
        public async Task<ContentDialogResult> ShowAsync(string info, string extensionName)
        {
            var page = new TextInfoPage(info);
            var dialog = new Helper.Logout.AutoLogoutContentDialog
            {
                Title = extensionName + " " + "info".Localized("Dialog_Info_Headline/Text"),
                CloseButtonText = "Close".Localized("Dialog_Button_Close/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            return await dialog.ShowAsync();
        }
    }
}
