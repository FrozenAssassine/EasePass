using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void WindowsHello_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(await AuthenticationHelper.AuthenticateAsync())
            {
                App.m_frame.Navigate(typeof(PasswordsPage));
            }
        }
    }
}
