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

using EasePass.Core.Database.Enums;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Extensions;
using EasePass.Helper.Security.Generator;
using EasePassExtensibility;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class ManageSecondFactorPage : Page
    {
        #region Properties
        /// <summary>
        /// The Token which should be used
        /// </summary>
        public SecureString Token { get; set; }
        /// <summary>
        /// A Copy of the Settings, which will be set with new values if the User configure it different
        /// </summary>
        public DatabaseSettings Settings { get; set; }
        #endregion

        #region Constructor
        public ManageSecondFactorPage(DatabaseSettings settings, SecureString token)
        {
            this.InitializeComponent();
            Settings = (DatabaseSettings)settings.Clone();
            Settings.SecondFactorType = SecondFactorType.Token;

            Token = token;
            Token ??= TokenHelper.Generate(12).ConvertToSecureString();

            enableSecondFactor.IsOn = settings.UseSecondFactor;
            
            secondFactorTypeTB.SelectedValue = settings.SecondFactorType;
            if (secondFactorTypeTB.SelectedValue == default)
            {
                secondFactorTypeTB.SelectedValue = "Token";
            }
        }
        #endregion

        private void enableSecondFactor_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggle = (ToggleSwitch)sender;
            bool isOn = toggle.IsOn;
            secondFactorTypeTB.IsEnabled = isOn;
            Settings.UseSecondFactor = isOn;

            if (isOn)
            {
                Settings.SecondFactorType = (SecondFactorType)Enum.Parse(typeof(SecondFactorType), (string)secondFactorTypeTB.SelectedValue);
                secondFactorToken.Token = Token.ConvertToString();
            }
            else
            {
                Settings.SecondFactorType = SecondFactorType.None;
                secondFactorToken.Token = "";
            }
        }

        private void SecondFactorTypeTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (enableSecondFactor.IsOn)
            {
                ComboBox combo = (ComboBox)sender;
                if (combo.SelectedValue == null)
                    return;

                try
                {
                    Settings.SecondFactorType = (SecondFactorType)Enum.Parse(typeof(SecondFactorType), (string)secondFactorTypeTB.SelectedValue);
                }
                catch { }
            }
        }
    }
}