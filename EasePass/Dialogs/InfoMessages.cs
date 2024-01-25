using EasePass.Extensions;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;

namespace EasePass.Dialogs
{
    internal class InfoMessages
    {
        public static void EnteredWrongPassword(int attempts) => new InfoBar().Show("Wrong password", $"You entered the wrong password.\nPlease try again\n({attempts}/3)", InfoBarSeverity.Error);
        public static void TooManyPasswordAttempts() => new InfoBar().Show("Too many attempts", "You entered the password wrong three times.", InfoBarSeverity.Error);
        public static void ImportDBWrongPassword() => new InfoBar().Show("Wrong password", "Could not import database, because you've entered the wrong password.", InfoBarSeverity.Error);
        public static void ImportDBSuccess() => new InfoBar().Show("Imported successfully", "Imported data successfully", InfoBarSeverity.Success);
        public static void ExportDBSuccess() => new InfoBar().Show("Exported successfully", "Exported data successfully", InfoBarSeverity.Success);
        public static void PasswordTooShort() => new InfoBar().Show("Password too short", "The password is too short", InfoBarSeverity.Error);
        public static void PasswordAlreadyUsed() => new InfoBar().Show("Password already used", "We highly recommend using a different password for each service!", InfoBarSeverity.Warning, 10);
        public static void ChangePasswordWrong() => new InfoBar().Show("Wrong password", "The current password is incorrect", InfoBarSeverity.Error);
        public static void PasswordsDoNotMatch() => new InfoBar().Show("Passwords do not match", "", InfoBarSeverity.Error);
        public static void SuccessfullyChangedPassword() => new InfoBar().Show("Password successfully changed", "Your password was successfully changed", InfoBarSeverity.Success);
        public static void AutomaticallyLoggedOut() => new InfoBar().ShowUntilLogin("We automatically logged you out due to inactivity", "", InfoBarSeverity.Informational);
        public static void NoAccessToPathDatabaseNotSaved(string path) => new InfoBar().Show("Database could not be saved", $"Database could not be saved. No access to the path {path}", InfoBarSeverity.Error, 15);
        public static void NoAccessToPathDatabaseNotLoaded(string path) => new InfoBar().Show("Database could not be loaded", $"Database could not be loaded. No access to the path {path}", InfoBarSeverity.Error, 15);
        public static void DatabaseSaveToFileError(string path) => new InfoBar().Show("Error while saving database", $"Data save error at {path}. Verify connection or device storage, then retry.", InfoBarSeverity.Error, 15);
        public static void DatabaseFileNotFoundAt(string path) => new InfoBar().Show("Database not found", $"Your database with the stored passwords could not be found at path {path}", InfoBarSeverity.Error, 15);
        public static void DatabaseFileNotFoundAt() => new InfoBar().Show("Database creation", "New database created!", InfoBarSeverity.Informational, 15);
        public static void DatabaseInvalidData() => new InfoBar().Show("Invalid Database", $"Database could not be loaded. It may be corrupted or invalid.", InfoBarSeverity.Error, 15);
        public static void PrinterNotSelected() => new InfoBar().Show("Printer not selected", $"Please select a printer before printing.", InfoBarSeverity.Error, 15);
        public static void PrinterItemSkipped(string name) => new InfoBar().Show("Password skipped", $"The service \"" + name + "\" was skipped, because it doesn't fit into the page.", InfoBarSeverity.Error, 15);
        public static void CouldNotGetExtensions(string exception) => new InfoBar().Show("Could not get Plugins", "Could not get the plugins from the Server:\n" + exception, InfoBarSeverity.Error, 10);
        public static void FileIsNotAnExtensions() => new InfoBar().Show("File is not a plugin", "The selected file is not an Ease Pass plugin!", InfoBarSeverity.Error, 10);
        public static void ExtensionAlreadyInstalled() => new InfoBar().Show("Plugin already installed", "The plugin is already installed!", InfoBarSeverity.Error, 10);
        public static void Invalid2FA() => new InfoBar().Show("Invalid 2FA secret", "This 2FA secret is not valid!", InfoBarSeverity.Error, 10);
        public static void Error() => new InfoBar().Show("Error", "An error has occurred!", InfoBarSeverity.Error, 5);
        public static void CantDeleteDatabase() => new InfoBar().Show("Can't delete database!", "Create another database to delete this database!", InfoBarSeverity.Error, 5);
        public static void CantDeleteLoadedDatabase() => new InfoBar().Show("Can't delete database!", "You can't delete a loaded database!", InfoBarSeverity.Error, 5);
        public static void DatabaseDeleted() => new InfoBar().Show("Database deleted successfully!", "", InfoBarSeverity.Success, 5);
        public static void DatabaseLoaded() => new InfoBar().Show("Database loaded successfully!", "", InfoBarSeverity.Success, 5);
        public static void NewVersionInfo(string version)
        {
            var btn = new Button { Content = "Changelog" };
            btn.Click += delegate
            {
                App.m_frame.Navigate(typeof(AboutPage), 0);
            };

            new InfoBar().Show("New version", "Welcome to Ease Pass version " + version + "\n", btn, InfoBarSeverity.Success);
        }

        public static InfoBar DownloadingPluginInfo()
        {
            var progressbar = new ProgressBar { ShowPaused = false, ShowError = false, IsIndeterminate = true };

            var infobar = new InfoBar();
            infobar.ShowInfobar("Downloading plugin in progress", "", progressbar, InfoBarSeverity.Informational);
            return infobar;
        }

        public static void CouldNotCreateDatabaseBackupFolder(Exception ex) => new InfoBar().Show("Could not create database backup folder", ex.Message, InfoBarSeverity.Error, 10);
    }
}
