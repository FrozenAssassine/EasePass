using EasePass.Models;
using EasyCodeClass;
using Microsoft.UI.Xaml.Controls;
using System;

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

        public PasswordManagerItem GetValue()
        {
            if (input == null)
                input = new PasswordManagerItem();

            input.Secret = secretTB.Password;
            input.Digits = digitsTB.Text;
            input.Interval = intervalTB.Text;
            input.Algorithm = (string)algorithmTB.SelectedItem;
            
            return input;
        }

        private void digitsTB_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string digits = digitsTB.Text;
            string newDigits = "";
            for(int i = 0; i < digits.Length; i++)
            {
                if (char.IsDigit(digits[i])) newDigits += digits[i];
            }
            digitsTB.Text = newDigits;
            digitsTB.SelectionStart = digitsTB.Text.Length;
        }

        private void intervalTB_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string interval = intervalTB.Text;
            string newInterval = "";
            for (int i = 0; i < interval.Length; i++)
            {
                if (char.IsDigit(interval[i])) newInterval += interval[i];
            }
            intervalTB.Text = newInterval;
            intervalTB.SelectionStart = intervalTB.Text.Length;
        }

        private void screen_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // TODO: Scan qr-code from somewhere on the screen.
            //       The result is an url.
            //       Call the function LoadFromUrl(string url)
        }

        private void webcam_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // TODO: Scan qr-code using webcam.
            //       The result is an url.
            //       Call the function LoadFromUrl(string url)
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
