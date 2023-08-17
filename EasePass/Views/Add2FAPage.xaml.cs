using EasePass.Models;
using Microsoft.UI.Xaml.Controls;

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
    }
}
