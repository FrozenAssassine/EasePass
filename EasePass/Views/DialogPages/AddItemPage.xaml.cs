using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace EasePass.Views
{
    public sealed partial class AddItemPage : Page
    {
        PasswordManagerItem input = null;

        PasswordsPage.PasswordExists pe;

        bool isEditMode = false;

        public AddItemPage(PasswordsPage.PasswordExists pe)
        {
            this.InitializeComponent();
            this.pe = pe;
            Hide2FA();
        }

        public AddItemPage(PasswordsPage.PasswordExists pe, PasswordManagerItem input = null)
        {
            this.InitializeComponent();
            this.pe = pe;
            isEditMode = true;

            if (input == null)
                return;

            this.input = input;
            notesTB.Text = input.Notes;
            pwTB.Password = input.Password;
            emailTB.Text = input.Email;
            usernameTB.Text = input.Username;
            nameTB.Text = input.DisplayName;
            websiteTB.Text = input.Website;

            if (!string.IsNullOrEmpty(input.Secret))
            {
                secretTB.Password = input.Secret;
                digitsTB.Text = input.Digits;
                intervalTB.Text = input.Interval;
                algorithmTB.SelectedItem = input.Algorithm;
            }
            else
                Hide2FA();

            if (scroll.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                scroll.Padding = new Microsoft.UI.Xaml.Thickness(0, 0, 13, 0);
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
            input.Website = websiteTB.Text;

            input.Secret = secretTB.Password;
            input.Digits = digitsTB.Text;
            input.Interval = intervalTB.Text;
            input.Algorithm = (string)algorithmTB.SelectedItem;


            if (pwTB.Password.Length > 0 && !(pe(pwTB.Password) == 0 || (isEditMode && pe(pwTB.Password) == 1))) InfoMessages.PasswordAlreadyUsed();

            return input;
        }

        private void DigitsTB_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string digits = digitsTB.Text;
            string newDigits = "";
            for (int i = 0; i < digits.Length; i++)
            {
                if (char.IsDigit(digits[i]))
                    newDigits += digits[i];
            }
            digitsTB.Text = newDigits;
            digitsTB.SelectionStart = digitsTB.Text.Length;
        }

        private void IntervalTB_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string interval = intervalTB.Text;
            string newInterval = "";
            for (int i = 0; i < interval.Length; i++)
            {
                if (char.IsDigit(interval[i]))
                    newInterval += interval[i];
            }
            intervalTB.Text = newInterval;
            intervalTB.SelectionStart = intervalTB.Text.Length;
        }

        private void Hide2FA()
        {
            twoFactorAuthExpander.Visibility = twoFactorAuthTitle.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        private void rem2FA_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            secretTB.Password = "";
            Hide2FA();
        }

        private void exp2FA_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            qrcode.Source = QRCodeScanner.GenerateQRCode(TOTP.EncodeUrl(nameTB.Text, usernameTB.Text, secretTB.Password, TOTP.StringToHashMode((string)algorithmTB.SelectedItem), ConvertHelper.ToInt(digitsTB.Text, 6), ConvertHelper.ToInt(intervalTB.Text, 30)));
            qrcode.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        private void pwTB_PasswordChanged(string password)
        {
            if (!(pe(pwTB.Password) == 0 || (isEditMode && pe(pwTB.Password) == 1)))
            {
                RedBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                RedBorder.BorderThickness = new Microsoft.UI.Xaml.Thickness(2);
            }
            else
            {
                RedBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                RedBorder.BorderThickness = new Microsoft.UI.Xaml.Thickness(0, 2, 0, 2);
            }
        }

        private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            pwTB.Password = await PasswordHelper.GeneratePassword();
        }

        private void AttachFileButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //create an item of the class AttachedPasswordItemFile and pass the filepath and filename.
            //this item can be displayed in the listview.
            //pick and attach the file here, use another class for this maybe
        }

        private void RemoveFile_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is Button btn && btn.Tag is AttachedPasswordItemFile file)
            {
                //the user pressed the delete button for a file in the listview.
                //remove the file from the item.
            }
        }
    }
}
