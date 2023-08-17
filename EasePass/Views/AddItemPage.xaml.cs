using EasePass.Models;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Views
{
    public sealed partial class AddItemPage : Page
    {
        PasswordManagerItem input = null;

        public AddItemPage(PasswordManagerItem input = null)
        {
            this.InitializeComponent();

            if (input == null)
                return;

            this.input = input;
            notesTB.Text = input.Notes;
            pwTB.Password = input.Password;
            emailTB.Text = input.Email;
            usernameTB.Text = input.Username;
            nameTB.Text = input.DisplayName;

            if (!string.IsNullOrEmpty(input.Secret))
            {
                secretTB.Password = input.Secret;
                digitsTB.Text = input.Digits;
                intervalTB.Text = input.Interval;
                algorithmTB.SelectedItem = input.Algorithm;
            }
            else
            {
                secretLB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                secretTB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                digitsLB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                digitsTB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                intervalLB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                intervalTB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                algorithmLB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                algorithmTB.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }

        public PasswordManagerItem GetValue()
        {
            if (input == null)
                input = new PasswordManagerItem();

            input.Notes = notesTB.Text;
            input.Password = pwTB.Password;
            input.Email = emailTB.Text;
            input.Username = usernameTB.Text;
            input.DisplayName = nameTB.Text;

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
            for (int i = 0; i < digits.Length; i++)
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
