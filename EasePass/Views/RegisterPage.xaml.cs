using EasePass.Dialogs;
using EasePass.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if(passwordBox.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            if (!passwordBox.Password.Equals(passwordBoxRepeat.Password))
            {
                InfoMessages.PasswordsDoNotMatch();
                return;
            }

            AuthenticationHelper.StorePassword(passwordBox.Password);

            SecureString pw = new SecureString();
            foreach (var character in passwordBox.Password)
            {
                pw.AppendChar(character);
            }
            App.m_frame.Navigate(typeof(PasswordsPage), pw);
        }

        private void Enter_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            Register_Click(null, null);
        }
    }
}
