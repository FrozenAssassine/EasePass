using EasePass.Dialogs;
using EasePass.Models;
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
            if (passwordBox.Password.Length < 4)
            {
                InfoMessages.PasswordTooShort();
                return;
            }

            if (!passwordBox.Password.Equals(passwordBoxRepeat.Password))
            {
                InfoMessages.PasswordsDoNotMatch();
                return;
            }

            SecureString pw = new SecureString();
            foreach (var character in passwordBox.Password)
            {
                pw.AppendChar(character);
            }

            Database.CreateEmptyDatabase(Database.GetAllDatabasePaths()[0], pw);
            App.m_frame.Navigate(typeof(PasswordsPage), pw);
        }

        private void passwordBoxRepeat_EnterInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            Register_Click(null, null);
        }

        private void passwordBox_EnterInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            passwordBoxRepeat.Focus(FocusState.Programmatic);
        }
    }
}
