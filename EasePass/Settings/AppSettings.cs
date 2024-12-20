using Microsoft.UI.Windowing;

namespace EasePass.Settings;

public class AppSettings
{
    public static string Language
    {
        get => SettingsManager.GetSettings(AppSettingsValues.language, DefaultSettingsValues.defaultLanguage);
        set => SettingsManager.SaveSettings(AppSettingsValues.language, value);
    }

    public static string PasswordSalt
    {
        get => SettingsManager.GetSettings(AppSettingsValues.pSalt);
        set => SettingsManager.SaveSettings(AppSettingsValues.pSalt, value);
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
        set => SettingsManager.SaveSettings(AppSettingsValues.windowState, value);
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
        get => SettingsManager.GetSettings(AppSettingsValues.databasePaths, DefaultSettingsValues.databasePaths);
        set => SettingsManager.SaveSettings(AppSettingsValues.databasePaths, value);
    }
    public static string LoadedDatabaseName
    {
        get => SettingsManager.GetSettings(AppSettingsValues.loadedDatabaseName);
        set => SettingsManager.SaveSettings(AppSettingsValues.loadedDatabaseName, value);
    }
    public static int clipboardClearTimeoutSec
    {
        get => SettingsManager.GetSettingsAsInt(AppSettingsValues.loadedDatabaseName, DefaultSettingsValues.ClipboardClearTimeoutSec);
        set => SettingsManager.SaveSettings(AppSettingsValues.loadedDatabaseName, value);
    }
}
