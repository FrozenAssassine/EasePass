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

using EasePass.Core.Database;
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
    public InfoBar PasswordInfo => passwordInfo;

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

        var newDB = new DatabaseItem(Database.LoadedInstance.Path);
        var enteredPW = changePW_currentPw.Password.ConvertToSecureString();
        if (newDB.CheckPasswordCorrect(enteredPW).result != PasswordValidationResult.Success)
            return ChangePasswordPageResult.IncorrectPassword;
        
        if (!changePW_newPw.Password.Equals(changePW_repeatPw.Password))
        {
            return ChangePasswordPageResult.PWNotMatching;
        }

        newDB.Load(enteredPW);

        Database.LoadedInstance.MasterPassword = changePW_newPw.Password.ConvertToSecureString();
        Database.LoadedInstance.Save();

        return ChangePasswordPageResult.Success;
    }
}
