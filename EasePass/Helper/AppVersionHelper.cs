using EasePass.Dialogs;
using EasePass.Settings;
using System;
using Windows.ApplicationModel;

namespace EasePass.Helper
{
    internal class AppVersionHelper
    {
        private static bool IsOnNewVersion(string version)
        {
            string lastSavedVersion = AppSettings.GetSettings(AppSettingsValues.appVersion);
            AppSettings.SaveSettings(AppSettingsValues.appVersion, version);

            //no version saved -> first start
            if (lastSavedVersion.Length == 0)
                return false;

            if (!version.Equals(lastSavedVersion, StringComparison.Ordinal))
                return true;

            return false;
        }

        public static string GetAppVersion()
        {
            return Package.Current.Id.Version.Major + "." +
                Package.Current.Id.Version.Minor + "." +
                Package.Current.Id.Version.Build;
        }

        public static void CheckNewVersion()
        {
            var version = GetAppVersion();
            if (IsOnNewVersion(version))
            {
                InfoMessages.NewVersionInfo(version);
            }
        }
    }
}