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

using EasePass.Core.Database.Format.Serialization;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ManageSecondFactorDialog
    {
        public async Task<bool> ShowAsync(string databaseName, DatabaseSettings settings)
        {
            ManageSecondFactorPage page = new ManageSecondFactorPage(settings);
            AutoLogoutContentDialog dialog = new AutoLogoutContentDialog
            {
                Title = "Manage SecondFactor for".Localized("Dialog_ManageSecondFactor_Headline/Text") + " " + databaseName,
                PrimaryButtonText = "Change".Localized("Dialog_Button_Change/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = page
            };

            MainWindow.CurrentInstance.inactivityHelper.PreventAutologout = true;
            bool result = await dialog.ShowAsync() == ContentDialogResult.Primary;
            MainWindow.CurrentInstance.inactivityHelper.PreventAutologout = false;
            return result;
        }
    }
}
