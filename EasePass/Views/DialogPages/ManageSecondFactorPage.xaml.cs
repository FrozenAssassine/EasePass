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

using EasePass.AppWindows;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text;
using EasePass.Controls;

namespace EasePass.Views
{
    public sealed partial class ManageSecondFactorPage : Page
    {
        DatabaseSettings Settings { get; set; }

        public ManageSecondFactorPage(DatabaseSettings settings)
        {
            this.InitializeComponent();
            enableSecondFactor.IsOn = settings.UseSecondFactor;
            secondFactorTypeTB.SelectedValue = settings.SecondFactorType;
            if (secondFactorTypeTB.SelectedValue == default)
            {
                secondFactorTypeTB.SelectedValue = "Token";
            }

            Settings = (DatabaseSettings)settings.Clone();
            Settings.SecondFactorType = Core.Database.Enums.SecondFactorType.Token;
        }

        private void enableSecondFactor_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggle = (ToggleSwitch)sender;
            secondFactorTypeTB.IsEnabled = toggle.IsOn;
        }
    }
}