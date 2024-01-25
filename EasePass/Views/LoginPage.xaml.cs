using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
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
                return;
            }
            dailyTipTextBlock.Text = tip;
            dailyTipGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
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
                var db = new Database(Database.GetAllDatabasePaths()[0], pw);

                WrongCount = 0;

                App.m_frame.Navigate(typeof(PasswordsPage), db);
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
