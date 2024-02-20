using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class LoginPage : Page
    {
        int WrongCount = 0;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var databases = Database.GetAllUnloadedDatabases();
            var comboboxIndexDBName = AppSettings.GetSettings(AppSettingsValues.loadedDatabaseName, databases.Length > 0 ? databases[0].Name : "");
            foreach (var item in databases)
            {
                databasebox.Items.Add(item);
                if(comboboxIndexDBName.Equals(item.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    databasebox.SelectedItem = item;
                }
            }

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

            SecureString pw = passwordBox.Password.ConvertToSecureString();
            
            if (databasebox.SelectedItem == null)
                return;

            var dbToLoad = new Database((databasebox.SelectedItem as Database).Path);
            var validationResult = dbToLoad.ValidatePwAndLoadDB(pw, false);
            if (validationResult == PasswordValidationResult.WrongPassword)
            {
                WrongCount++;
                InfoMessages.EnteredWrongPassword(WrongCount);
                return;
            }
            else if(validationResult == PasswordValidationResult.DatabaseNotFound)
            {
                InfoMessages.DatabaseFileNotFoundAt(dbToLoad.Path);
                return;
            }

            WrongCount = 0;

            App.m_frame.Navigate(typeof(PasswordsPage));
            return;
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
            databasebox.Items.Add(newDB);
        }

        private void databasebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (databasebox.SelectedItem == null)
                return;

            AppSettings.SaveSettings(AppSettingsValues.loadedDatabaseName, (databasebox.SelectedItem as Database).Name);
        }
    }
}
