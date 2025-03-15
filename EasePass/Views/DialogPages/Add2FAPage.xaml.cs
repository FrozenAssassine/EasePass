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
using EasePass.Helper.Security.Generator;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text;

namespace EasePass.Views
{
    public sealed partial class Add2FAPage : Page
    {
        PasswordManagerItem input = null;

        public Add2FAPage(PasswordManagerItem input)
        {
            this.InitializeComponent();

            if (input == null)
                return;

            this.input = input;
            secretTB.Password = input.Secret;
            digitsTB.Text = input.Digits;
            intervalTB.Text = input.Interval;
            algorithmTB.SelectedItem = input.Algorithm;
        }

        public void UpdateValue()
        {
            if (input == null)
                input = new PasswordManagerItem();

            input.Secret = secretTB.Password;
            input.Digits = digitsTB.Text;
            input.Interval = intervalTB.Text;
            input.Algorithm = (string)algorithmTB.SelectedItem;
        }

        private void DigitsTB_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string digits = digitsTB.Text;
            StringBuilder newDigits = new StringBuilder();
            for (int i = 0; i < digits.Length; i++)
            {
                if (char.IsDigit(digits[i]))
                    newDigits.Append(digits[i]);
            }
            digitsTB.Text = newDigits.ToString();
            digitsTB.SelectionStart = digitsTB.Text.Length;
        }

        private void IntervalTB_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string interval = intervalTB.Text;
            StringBuilder newInterval = new StringBuilder();
            for (int i = 0; i < interval.Length; i++)
            {
                if (char.IsDigit(interval[i]))
                    newInterval.Append(interval[i]);
            }
            intervalTB.Text = newInterval.ToString();
            intervalTB.SelectionStart = intervalTB.Text.Length;
        }

        private void Screen_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ScreenScannerWindow screen = new ScreenScannerWindow();
            screen.Closed += ScannerWindow_Closed;
            screen.AppWindow.Show();
        }

        private void Webcam_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            WebcamScannerWindow webcam = new WebcamScannerWindow();
            webcam.Closed += ScannerWindow_Closed;
            webcam.AppWindow.Show();
        }

        private void ScannerWindow_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
        {
            if (!string.IsNullOrEmpty(((IScannerWindow)sender).Result))
                LoadFromUrl(((IScannerWindow)sender).Result);
        }

        private void LoadFromUrl(string url)
        {
            var res = TOTP.DecodeUrl(url);
            secretTB.Password = res.Secret;
            digitsTB.Text = Convert.ToString(res.Digits);
            intervalTB.Text = Convert.ToString(res.Period);
            algorithmTB.SelectedItem = TOTP.HashModeToString(res.Algorithm);
        }
    }
}
