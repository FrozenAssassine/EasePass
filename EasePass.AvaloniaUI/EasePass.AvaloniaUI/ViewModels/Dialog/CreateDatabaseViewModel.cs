using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper.FileSystem;
using EasePass.Settings;
using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.ViewModels.Dialog;

public partial class CreateDatabaseViewModel : ObservableObject
{
    [ObservableProperty] private string _databaseName = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _repeatPassword = "";
    [ObservableProperty] private string _databasePath = "";

    private string _databaseOutputLocation = "";

    public bool PasswordsMatch => RepeatPassword == Password;
    public bool PathValid => _databaseOutputLocation.Length > 0;
    public bool PasswordLengthCorrect => Password.Length > 3;

    public string CreateDatabasePath()
    {
        var name = string.IsNullOrEmpty(DatabaseName) ? "Database" : DatabaseName;
        return Path.Combine(_databaseOutputLocation, name + ".epdb");
    }

    public bool PathAlreadyExists => PathValid && File.Exists(CreateDatabasePath());

    public (string path, SecureString masterPassword) Evaluate()
    {
        return (CreateDatabasePath(), Password.ConvertToSecureString());
    }

    partial void OnDatabaseNameChanged(string value)
    {
        // Remove invalid filename chars
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string cleaned = value;
        foreach (char c in invalidChars)
            cleaned = cleaned.Replace(c.ToString(), "");
        if (cleaned != value)
            DatabaseName = cleaned;
    }

    [RelayCommand]
    private async Task SelectPath()
    {
        var pickerRes = await FilePickerHelper.PickFolder();
        if (!pickerRes.success)
            return;

        _databaseOutputLocation = pickerRes.path;
        DatabasePath = pickerRes.path;

        if (PathValid && PathAlreadyExists)
        {
            InfoMessages.DatabaseWithThatNameAlreadyExists();
        }
    }
}
