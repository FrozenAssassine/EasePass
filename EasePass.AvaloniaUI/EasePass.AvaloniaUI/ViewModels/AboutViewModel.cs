using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Helper.AppHelper;
using EasePass.Settings;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace EasePass.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    [ObservableProperty] private string _appVersion;
    [ObservableProperty] private string _changelogText = "";

    public AboutViewModel()
    {
        AppVersion = AppVersionHelper.GetAppVersion();
        LoadChangelog();
    }

    private void LoadChangelog()
    {
        try
        {
            string filePath = Path.Combine(ApplicationData.InstalledLocation, "Assets", "changelog.txt");
            if (File.Exists(filePath))
            {
                ChangelogText = File.ReadAllText(filePath);
            }
            else
            {
                ChangelogText = "No changelog available.";
            }
        }
        catch
        {
            ChangelogText = "Could not load changelog.";
        }
    }

    [RelayCommand]
    private void OpenGitHub()
    {
        OpenUrl("https://github.com/FrozenAssassine/EasePass");
    }

    [RelayCommand]
    private void OpenPrivacyPolicy()
    {
        OpenUrl("https://github.com/FrozenAssassine/EasePass/blob/master/PrivacyPolicy.md");
    }

    [RelayCommand]
    private void OpenLicense()
    {
        OpenUrl("https://github.com/FrozenAssassine/EasePass/blob/master/LICENSE");
    }

    private static void OpenUrl(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", url);
            else
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch { }
    }
}
