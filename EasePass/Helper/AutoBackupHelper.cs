using EasePass.Models;
using EasePass.Settings;
using EasePass.Views;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

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
