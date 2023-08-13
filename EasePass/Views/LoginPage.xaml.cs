using EasePass.Dialogs;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class LoginPage : Page
    {
        int WrongCount = 0;

        public LoginPage()
        {
            this.InitializeComponent();

            passwordBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }

        private void PWLogin_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (WrongCount > 2)
            {
                InfoMessages.TooManyPasswordAttempts();
                return;
            }

            if (AuthenticationHelper.VerifyPassword(passwordBox.Password))
            {
                WrongCount = 0;

                SecureString pw = new SecureString();
                foreach (var character in passwordBox.Password)
                {
                    pw.AppendChar(character);
                }

                App.m_frame.Navigate(typeof(PasswordsPage), pw);
                return;
            }
            
            WrongCount++;
            InfoMessages.EnteredWrongPassword(WrongCount);
        }
        private void Enter_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            PWLogin_Click(null, null);
        }
    }
}
