using CommunityToolkit.Mvvm.ComponentModel;
using EasePass.Core.Database;
using EasePass.Extensions;
using EasePass.Models;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.ViewModels.Dialog;

public enum ChangePasswordPageResult
{
    IncorrectPassword,
    PWNotMatching,
    PWTooShort,
    Success,
}

public partial class ChangePasswordViewModel : ObservableObject
{
    [ObservableProperty] private string _currentPassword = "";
    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _repeatPassword = "";

    public async Task<ChangePasswordPageResult> ChangePassword()
    {
        if (NewPassword.Length < 4)
            return ChangePasswordPageResult.PWTooShort;

        if (NewPassword != RepeatPassword)
            return ChangePasswordPageResult.PWNotMatching;

        SecureString enteredPW = CurrentPassword.ConvertToSecureString();
        DatabaseItem newDB = new DatabaseItem(Database.LoadedInstance.DatabaseSource);
        if ((await newDB.CheckPasswordCorrect(enteredPW)).result != PasswordValidationResult.Success)
            return ChangePasswordPageResult.IncorrectPassword;

        await newDB.Load(enteredPW);
        Database.LoadedInstance.MasterPassword = NewPassword.ConvertToSecureString();
        await Database.LoadedInstance.ForceSaveAsync();

        return ChangePasswordPageResult.Success;
    }
}
