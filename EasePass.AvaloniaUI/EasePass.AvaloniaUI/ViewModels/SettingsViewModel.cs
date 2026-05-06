using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Core.Database;
using EasePass.Helper;
using EasePass.Helper.AppHelper;
using EasePass.Settings;
using System;
using System.Threading.Tasks;

namespace EasePass.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private int _inactivityLogoutTime;
    [ObservableProperty] private int _clipboardClearTimeout;
    [ObservableProperty] private string _passwordChars;
    [ObservableProperty] private string _passwordLengthText;
    [ObservableProperty] private bool _verifyLeakedPasswords;
    [ObservableProperty] private bool _doubleTapToCopy;
    [ObservableProperty] private bool _showIcons;

    private bool _blockEvents = false;

    public SettingsViewModel()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        _blockEvents = true;

        InactivityLogoutTime = AppSettings.InactivityLogoutTime;
        ClipboardClearTimeout = AppSettings.ClipboardClearTimeoutSec;
        PasswordChars = AppSettings.PasswordChars;
        PasswordLengthText = AppSettings.PasswordLength.ToString();
        VerifyLeakedPasswords = !AppSettings.DisableLeakedPasswords;
        DoubleTapToCopy = AppSettings.DoubleTapToCopy;
        ShowIcons = AppSettings.ShowIcons;

        _blockEvents = false;
    }

    partial void OnInactivityLogoutTimeChanged(int value)
    {
        if (_blockEvents) return;
        AppSettings.InactivityLogoutTime = value;
    }

    partial void OnClipboardClearTimeoutChanged(int value)
    {
        if (_blockEvents) return;
        AppSettings.ClipboardClearTimeoutSec = value;
    }

    partial void OnPasswordCharsChanged(string value)
    {
        if (_blockEvents) return;
        AppSettings.PasswordChars = string.IsNullOrEmpty(value) ? DefaultSettingsValues.passwordChars : value;
    }

    partial void OnPasswordLengthTextChanged(string value)
    {
        if (_blockEvents) return;
        if (string.IsNullOrEmpty(value) || value == "0")
        {
            AppSettings.PasswordLength = DefaultSettingsValues.passwordLength;
            return;
        }
        AppSettings.PasswordLength = Math.Max(ConvertHelper.ToInt(value, DefaultSettingsValues.passwordLength), 8);
    }

    partial void OnVerifyLeakedPasswordsChanged(bool value)
    {
        if (_blockEvents) return;
        AppSettings.DisableLeakedPasswords = !value;
    }

    partial void OnDoubleTapToCopyChanged(bool value)
    {
        if (_blockEvents) return;
        AppSettings.DoubleTapToCopy = value;
    }

    partial void OnShowIconsChanged(bool value)
    {
        if (_blockEvents) return;
        AppSettings.ShowIcons = value;
    }

    [RelayCommand]
    private void ResetPasswordChars()
    {
        PasswordChars = DefaultSettingsValues.passwordChars;
    }

    [RelayCommand]
    private async Task ResetPopularity()
    {
        if (Database.LoadedInstance == null) return;
        for (int i = 0; i < Database.LoadedInstance.Items.Count; i++)
        {
            Database.LoadedInstance.Items[i].Clicks.Clear();
        }
        await Database.LoadedInstance.SaveAsync();
    }

    [RelayCommand]
    private void ManageDatabases() => NavigationHelper.ToManageDB();
}
