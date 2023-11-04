using EasePass.Extensions;
using Microsoft.UI.Xaml.Controls;

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
        public static void ChangePasswordWrong() => new InfoBar().Show("Wrong password", "The current password is incorrect", InfoBarSeverity.Error);
        public static void PasswordsDoNotMatch() => new InfoBar().Show("Passwords do not match", "", InfoBarSeverity.Error);
        public static void SuccessfullyChangedPassword() => new InfoBar().Show("Password successfully changed", "Your password was successfully changed", InfoBarSeverity.Success);
        public static void AutomaticallyLoggedOut() => new InfoBar().ShowUntilLogin("We automatically logged you out due to inactivity", "", InfoBarSeverity.Informational);
        public static void NoAccessToPathDatabaseNotSaved(string path) => new InfoBar().Show("Database could not be saved", $"Database could not be saved. No access to the path {path}", InfoBarSeverity.Error, 15);
        public static void NoAccessToPathDatabaseNotLoaded(string path) => new InfoBar().Show("Database could not be loaded", $"Database could not be loaded. No access to the path {path}", InfoBarSeverity.Error, 15);
        public static void DatabaseSaveToFileError(string path) => new InfoBar().Show("Error while saving database", $"Data save error at {path}. Verify connection or device storage, then retry.", InfoBarSeverity.Error, 15);
        public static void DatabaseFileNotFoundAt(string path) => new InfoBar().Show("Database not found", $"Your database with the stored passwords could not be found at path {path}", InfoBarSeverity.Error, 15);
        public static void DatabaseInvalidData() => new InfoBar().Show("Invalid Database", $"Database could not be loaded. It may be corrupted or invalid.", InfoBarSeverity.Error, 15);
    }
}
