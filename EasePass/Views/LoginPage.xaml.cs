using EasePass.Dialogs;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Security;
using System.Threading.Tasks;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string tip = await DailyTipHelper.GetTodaysTip();
            if (string.IsNullOrEmpty(tip))
            {
                dailyTipGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                return;
            }
            dailyTipTextBlock.Text = tip;
        }

        private void PWLogin_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (WrongCount > 2)
            {
                InfoMessages.TooManyPasswordAttempts();
                return;
            }

            SecureString pw = new SecureString();
            foreach (var character in passwordBox.Password)
            {
                pw.AppendChar(character);
            }

            try
            {
                var dbData = DatabaseHelper.LoadDatabase(pw);

                WrongCount = 0;

                App.m_frame.Navigate(typeof(PasswordsPage), pw);
                return;
            }
            catch
            {
                WrongCount++;
                InfoMessages.EnteredWrongPassword(WrongCount);
            }
        }
        private void Enter_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            PWLogin_Click(null, null);
        }
    }
}
