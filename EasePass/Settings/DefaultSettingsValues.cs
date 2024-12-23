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

using Windows.Storage;

namespace EasePass.Settings
{
    internal static class DefaultSettingsValues
    {
        public const string passwordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!\"§$%&/()=?*+'#-_.:,;<>";
        public const int passwordLength = 15;
        public const int inactivityLogoutTime = 3; //Minutes
        public const bool doubleTapToCopy = true;
        public const bool showIcons = true;
        public const bool disableLeakedPasswords = false;
        public static readonly string databasePaths = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "easepass.epdb");
        public static string defaultLanguage = "en-US";
        public const int ClipboardClearTimeoutSec = 15;
        public const int gridSplitterWidth = 240;

        public const int windowWidth = 1100;
        public const int windowHeight = 700;
        public const int windowLeft = -5000;
        public const int windowTop = -5000;
    }
}
