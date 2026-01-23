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
using Microsoft.UI.Windowing;

namespace EasePass.Settings;

public class AppSettings
{
    public static string Language
    {
        get => SettingsManager.GetSettings(AppSettingsValues.language, DefaultSettingsValues.defaultLanguage);
        set => SettingsManager.SaveSettings(AppSettingsValues.language, value);
    }

    public static int WindowWidth
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.windowWidth, DefaultSettingsValues.windowWidth);
        set => SettingsManager.SaveSettings(AppSettingsValues.windowWidth, value);
    }
    public static int WindowHeight
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.windowHeight, DefaultSettingsValues.windowHeight);
        set => SettingsManager.SaveSettings(AppSettingsValues.windowHeight, value);
    }
    public static int WindowLeft
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.windowLeft, DefaultSettingsValues.windowLeft);
        set => SettingsManager.SaveSettings(AppSettingsValues.windowLeft, value);
    }
    public static int WindowTop
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.windowTop, DefaultSettingsValues.windowTop);
        set => SettingsManager.SaveSettings(AppSettingsValues.windowTop, value);
    }
    public static int PasswordLength
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.passwordLength);
        set => SettingsManager.SaveSettings(AppSettingsValues.passwordLength, value);
    }
    public static string PasswordChars
    {
        get => SettingsManager.GetSettings(AppSettingsValues.passwordChars, DefaultSettingsValues.passwordChars);
        set => SettingsManager.SaveSettings(AppSettingsValues.passwordChars, value);
    }
    public static int InactivityLogoutTime
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.inactivityLogoutTime);
        set => SettingsManager.SaveSettings(AppSettingsValues.inactivityLogoutTime, value);
    }
    public static bool DoubleTapToCopy
    {
        get => SettingsManager.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy);
        set => SettingsManager.SaveSettings(AppSettingsValues.doubleTapToCopy, value);
    }

    public static bool ShowIcons
    {
        get => SettingsManager.GetSettingsAsBool(AppSettingsValues.showIcons, DefaultSettingsValues.showIcons);
        set => SettingsManager.SaveSettings(AppSettingsValues.showIcons, value);
    }
    public static bool DisableLeakedPasswords
    {
        get => SettingsManager.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords);
        set => SettingsManager.SaveSettings(AppSettingsValues.disableLeakedPasswords, value);
    }
    public static OverlappedPresenterState WindowState
    {
        get => (OverlappedPresenterState)SettingsManager.GetSettingsAsInt(AppSettingsValues.windowState, 2);
        set => SettingsManager.SaveSettings(AppSettingsValues.windowState, value.GetHashCode());
    }
    public static string AppVersion
    {
        get => SettingsManager.GetSettings(AppSettingsValues.appVersion);
        set => SettingsManager.SaveSettings(AppSettingsValues.appVersion, value);
    }
    public static int GridSplitterWidth
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.gridSplitterWidth, DefaultSettingsValues.gridSplitterWidth);
        set => SettingsManager.SaveSettings(AppSettingsValues.gridSplitterWidth, value);
    }
    public static string DatabasePaths
    {
        get => SettingsManager.GetSettings(AppSettingsValues.databasePaths);
        set => SettingsManager.SaveSettings(AppSettingsValues.databasePaths, value);
    }
    public static string LoadedDatabaseName
    {
        get => SettingsManager.GetSettings(AppSettingsValues.loadedDatabaseName, null);
        set => SettingsManager.SaveSettings(AppSettingsValues.loadedDatabaseName, value);
    }
    public static int ClipboardClearTimeoutSec
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.clipboardClearTimeoutSec, DefaultSettingsValues.ClipboardClearTimeoutSec);
        set => SettingsManager.SaveSettings(AppSettingsValues.clipboardClearTimeoutSec, value);
    }
}
