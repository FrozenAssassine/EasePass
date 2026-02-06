using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper.App;
using EasePass.Helper.Database;
using EasePass.Helper.Security.Generator;
using EasePass.Models;
using EasePass.Settings;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<DatabaseItem> _databases = new();

        [ObservableProperty]
        private DatabaseItem _selectedDatabase;

        [ObservableProperty]
        private bool _isLoginInProgress;

        [ObservableProperty]
        private string _dailyTipText;

        [ObservableProperty]
        private bool _dailyTipVisible;

        private int _wrongCount = 0;

        public LoginViewModel()
        {
            LoadDatabases();
            LoadDailyTip();
        }

        public void LoadDatabases() // Changed to void to avoid async void, no await needed for initial load?
        {
            var dbs = Core.Database.Database.GetAllUnloadedDatabases();
            Databases.Clear();
            foreach (var db in dbs) Databases.Add(db);

            string savedDbSource = AppSettings.LoadedDatabaseSource ?? (dbs.Length > 0 ? dbs[0].DatabaseSource.SourceDescription : "");

            foreach (var item in Databases)
            {
                if (item.DatabaseSource.SourceDescription == savedDbSource)
                {
                    SelectedDatabase = item;
                    break;
                }
            }
            if (SelectedDatabase == null && Databases.Count > 0)
                SelectedDatabase = Databases[0];
        }

        private async void LoadDailyTip()
        {
            string tip = await DailyTipHelper.GetTodaysTip(AppSettings.Language);
            if (!string.IsNullOrEmpty(tip))
            {
                DailyTipText = tip;
                DailyTipVisible = true;
            }
        }

        partial void OnSelectedDatabaseChanged(DatabaseItem value)
        {
            if (value != null)
                AppSettings.LoadedDatabaseSource = value.DatabaseSource.SourceDescription;
        }

        [RelayCommand]
        private async Task Login(string password)
        {
            if (_wrongCount > 2)
            {
                InfoMessages.TooManyPasswordAttempts();
                return;
            }

            if (SelectedDatabase == null || string.IsNullOrEmpty(password))
                return;

            IsLoginInProgress = true;
            await Task.Delay(50); // UI update

            var securePw = password.ConvertToSecureString();
            var res = await SelectedDatabase.CheckPasswordCorrect(securePw);

            if (res.result == PasswordValidationResult.WrongPassword)
            {
                _wrongCount++;
                InfoMessages.EnteredWrongPassword(_wrongCount);
                IsLoginInProgress = false;
                return;
            }
            else if (res.result == PasswordValidationResult.DatabaseNotFound)
            {
                InfoMessages.DatabaseFileNotFoundAt(SelectedDatabase.DatabaseSource.SourceDescription);
                IsLoginInProgress = false;
                return;
            }
            else if (res.result == PasswordValidationResult.LockedByOtherUser)
            {
                InfoMessages.DatabaseLockedByOtherUser();
                IsLoginInProgress = false;
                return;
            }

            SelectedDatabase.Load(securePw, res.database);
            if (SelectedDatabase.Settings == null)
            {
                _wrongCount++;
                InfoMessages.EnteredWrongPassword(_wrongCount);
                IsLoginInProgress = false;
                return;
            }
            _wrongCount = 0;

            if (SelectedDatabase.Settings.UseSecondFactor && SelectedDatabase.Settings.SecondFactorType == Core.Database.Enums.SecondFactorType.OTP)
            {
                string token = TokenHelper.Generate(12);
                await new ShowSecondFactorDialog().ShowAsync(token);
                SelectedDatabase.SecondFactor = token.ConvertToSecureString();
                token = null;
                await SelectedDatabase.SaveAsync();
            }

            NavigationHelper.ToPasswords();
            IsLoginInProgress = false;
        }

        [RelayCommand]
        private async Task CreateDatabase()
        {
            DatabaseItem newDB = await ManageDatabaseHelper.CreateDatabase();
            if (newDB != null)
            {
                Databases.Add(newDB);
                SelectedDatabase = newDB;
            }
        }

        [RelayCommand]
        private async Task ImportDatabase()
        {
            DatabaseItem res = await ManageDatabaseHelper.ImportDatabase();
            if (res != null)
            {
                Databases.Add(res);
                SelectedDatabase = res;
            }
        }
    }
}
