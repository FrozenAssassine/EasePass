/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

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