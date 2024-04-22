using EasePass.Extensions;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Views.DialogPages;
public enum ChangePasswordPageResult
{
    IncorrectPassword,
    PWNotMatching,
    PWTooShort,
    Success,
}

public sealed partial class ChangePasswordPage : Page
{

    public ChangePasswordPage()
    {
        this.InitializeComponent();
    }

    public ChangePasswordPageResult ChangePassword()
    {
        if (changePW_newPw.Password.Length < 4)
        {
            return ChangePasswordPageResult.PWTooShort;
        }

        var newDB = new Database(Database.LoadedInstance.Path);
        if (newDB.ValidatePwAndLoadDB(changePW_currentPw.Password.ConvertToSecureString()) != PasswordValidationResult.Success)
            return ChangePasswordPageResult.IncorrectPassword;
        
        if (!changePW_newPw.Password.Equals(changePW_repeatPw.Password))
        {
            return ChangePasswordPageResult.PWNotMatching;
        }

        Database.LoadedInstance.MasterPassword = changePW_newPw.Password.ConvertToSecureString();
        Database.LoadedInstance.Save();

        return ChangePasswordPageResult.Success;
    }
}
