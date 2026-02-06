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
using EasePass.Helper.Logout;
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ManageSecondFactorDialog
    {
        /// <summary>
        /// Opens the <see cref="ManageSecondFactorPage"/> and returns it's result
        /// </summary>
        /// <param name="databaseName">The Name of the Database, which SecondFactor should be managed</param>
        /// <param name="settings">The current Settins of the Database. The Page does not change the Properties.</param>
        /// <param name="token">The current SecondFactor Token (The Token will be allocated as <see cref="string"/> to show it to the User)</param>
        /// <returns>Returns the <see langword="true"/> in the <see langword="result"/> if Settings should be changed.
        /// If the Settins should be changed it will also return the new <see cref="DatabaseSettings"/> and the old/new Token as <see cref="SecureString"/>.
        /// Otherwise the old Settings and the old Token will be returned.</returns>
        public async Task<(bool Result, DatabaseSettings Settings, SecureString Token)> ShowAsync(string databaseName, DatabaseSettings settings, SecureString token = null)
        {
            ManageSecondFactorPage page = new ManageSecondFactorPage(settings, token);
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

            if (!result)
                return (result, settings, token);

            if (!page.Settings.UseSecondFactor)
            {
                // we need to set the Token to null to avoid errors on the other side
                // For example if the Token will be set always and at some point the Setting will not be checked
                page.Token = null;
            }
            return (result, page.Settings, page.Token);
        }
    }
}
