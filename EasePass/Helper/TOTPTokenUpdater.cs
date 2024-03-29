﻿using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

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
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            if (selectedItem == null)
                return;

            if (string.IsNullOrEmpty(selectedItem.Secret))
                return;

            totpTB.Text = generateCurrent(selectedItem);
        }

        public static string generateCurrent(PasswordManagerItem item)
        {
            string token = TOTP.GenerateTOTPToken(
                NTPClient.GetTime(),
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
            Timer_Tick(this, null);
        }
    }
}
