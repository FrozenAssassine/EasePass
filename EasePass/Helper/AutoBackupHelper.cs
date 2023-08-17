using EasePass.Models;
using EasePass.Settings;
using EasePass.Views;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;

namespace EasePass.Helper
{
    class AutoBackupHelper
    {
        private string DatabaseBackupPath => AppSettings.GetSettings(AppSettingsValues.autoBackupDBPath, "");
        private DispatcherTimer timer = new DispatcherTimer();
        private PasswordsPage passwordsPage;
        ObservableCollection<PasswordManagerItem> passwordItems = null;

        public void UpdateSettings()
        {
            bool doAutoBackups = AppSettings.GetSettingsAsBool(AppSettingsValues.autoBackupDB, DefaultSettingsValues.autoBackupDatabase);

            int autoBackupMinutes = AppSettings.GetSettingsAsInt(AppSettingsValues.autoBackupDBTime, DefaultSettingsValues.autoBackupDBTime);
            timer.Interval = new TimeSpan(0, autoBackupMinutes, 0);

            if (!doAutoBackups)
            {
                timer.Stop();
                return;
            }
            else if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        public void Start(PasswordsPage pwPage, ObservableCollection<PasswordManagerItem> pwItems)
        {
            UpdateSettings();

            timer.Tick += Timer_Tick;
            timer.Start();

            this.passwordItems = pwItems;
            this.passwordsPage = pwPage;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Start();
            if (passwordItems == null)
                return;

            if (DatabaseBackupPath.Length == 0)
                return;

            DatabaseHelper.SaveDatabase(passwordItems, passwordsPage.masterPassword, DatabaseBackupPath);
        }
    }
}
