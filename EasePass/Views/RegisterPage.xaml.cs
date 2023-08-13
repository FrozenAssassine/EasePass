using EasePass.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
            AuthenticationHelper.StorePassword(passwordBox.Password);
            App.m_frame.Navigate(typeof(PasswordsPage));
        }
    }
}
