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
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class SensitiveDataDialog
    {
        public async Task<bool> Dialog(Extension extension)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Warning!",
                PrimaryButtonText = "Allow",
                CloseButtonText = "Cancel",
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "This plugin tries to get access to sensitive information:" + Environment.NewLine + extension.ToString(false),
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
