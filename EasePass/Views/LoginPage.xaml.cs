using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class LoginPage : Page
    {
        int WrongCount = 0;
        List<Database> databases = null;

        public LoginPage()
        {
            this.InitializeComponent();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            databasebox.Items.Clear();
            databases = new List<Database>();
            databases.AddRange(Database.GetAllUnloadedDatabases());
            foreach (Database db in databases)
                databasebox.Items.Add(db.Name);

            databasebox.SelectedIndex = 0;

            string tip = await DailyTipHelper.GetTodaysTip();
            if (string.IsNullOrEmpty(tip))
                return;

            dailyTipTextBlock.Text = tip;
            dailyTipGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;

            passwordBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
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
                Database.LoadedInstance = new Database(databases[databasebox.SelectedIndex].Path, pw);

                WrongCount = 0;

                App.m_frame.Navigate(typeof(PasswordsPage));
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

        private async void CreateDatabase_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Database newDB = await new CreateDatabaseDialog().ShowAsync();
            if (newDB == null)
                return;
            Database.AddDatabasePath(newDB.Path);
            databases.Add(newDB);
            databasebox.Items.Add(newDB.Name);
        }
    }
}
