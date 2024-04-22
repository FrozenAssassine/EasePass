using Windows.Storage;

namespace EasePass.Settings
{
    internal static class DefaultSettingsValues
    {
        public const int InactivityLogoutTime = 3; //Minutes
        public const string PasswordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!\"§$%&/()=?*+'#-_.:,;<>";
        public const int PasswordLength = 15;
        public const int inactivityLogoutTime = 3; //Minutes
        public const bool doubleTapToCopy = true;
        public const bool showIcons = true;
        public const bool disableLeakedPasswords = false;
        public static readonly string databasePaths = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "easepass.epdb") + "|";
        public static string defaultLanguage = "en-US";
        public const int ClipboardClearTimeoutSec = 10;
    }
}
