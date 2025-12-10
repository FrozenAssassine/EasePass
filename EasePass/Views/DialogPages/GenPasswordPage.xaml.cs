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

using EasePass.Helper.Security.Generator;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public sealed partial class GenPasswordPage : Page
    {
        public GenPasswordPage()
        {
            this.InitializeComponent();
        }

        public async void GeneratePassword()
        {
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Visible;

            await _GeneratePassword();
            safetyChart.EvaluatePassword(passwordTB.Password);
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public async Task _GeneratePassword()
        {
            passwordTB.Password = await PasswordHelper.GeneratePassword();
        }

        private void passwordTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            safetyChart.EvaluatePassword(passwordTB.Password);
        }
    }
}
