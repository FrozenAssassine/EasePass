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

using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class TOTPTokenUpdater
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private PasswordManagerItem selectedItem;
        private TextBox totpTB;

        public TOTPTokenUpdater(PasswordManagerItem selectedItem, TextBox totpTB)
        {
            this.selectedItem = selectedItem;
            this.totpTB = totpTB;

            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_TickAsync;
        }

        private async void Timer_TickAsync(object sender, object e)
        {
            if (selectedItem == null)
                return;

            if (string.IsNullOrEmpty(selectedItem.Secret))
                return;

            totpTB.Text = await generateCurrent(selectedItem);
        }

        public static async Task<string> generateCurrent(PasswordManagerItem item)
        {
            string token = TOTP.GenerateTOTPToken(
                await NTPClient.GetTime(),
                item.Secret,
                ConvertHelper.ToInt(item.Digits, 6),
                ConvertHelper.ToInt(item.Interval, 30),
                TOTP.StringToHashMode(item.Algorithm)
                );

            string final = "";
            for (int i = 0; i < token.Length; i++)
            {
                final += token[i];
                if ((i + 1) % PasswordsPage.TOTP_SPACING == 0)
                    final += ' ';
            }
            final = final.Trim(' ');
            return final;
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void SimulateTickEvent()
        {
            Timer_TickAsync(this, null);
        }
    }
}
