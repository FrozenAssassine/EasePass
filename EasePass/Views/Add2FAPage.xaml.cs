using EasePass.Helper;
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
            for(int i = 0; i < digits.Length; i++)
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
            // TODO: Scan qr-code from somewhere on the screen.
            //       The result is an url.
            //       Call the function LoadFromUrl(string url)
        }

        private void Webcam_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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
