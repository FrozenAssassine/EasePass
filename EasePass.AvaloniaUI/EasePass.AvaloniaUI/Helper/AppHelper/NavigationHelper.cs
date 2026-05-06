using EasePass.Views;
using EasePass.Models;
using EasePass.ViewModels;
using System;
using System.Collections.Generic;

namespace EasePass.Helper.AppHelper;

public class NavigationHelper
{
    public static MainViewModel? MainVM { get; set; }
    private static Stack<ViewModelBase> _navigationStack = new();

    private static void Navigate(ViewModelBase viewModel, bool addToStack = true)
    {
        if (MainVM != null)
        {
            if (addToStack && MainVM.CurrentPage != null)
            {
                _navigationStack.Push(MainVM.CurrentPage);
            }
            MainVM.CurrentPage = viewModel;
            MainVM.ShowBackArrow = _navigationStack.Count > 0;
        }
    }

    public static void GoBack()
    {
        if (MainVM != null && _navigationStack.Count > 0)
        {
            var previous = _navigationStack.Pop();
            MainVM.CurrentPage = previous;
            MainVM.ShowBackArrow = _navigationStack.Count > 0;
        }
    }

    public static void ClearStack()
    {
        _navigationStack.Clear();
        if (MainVM != null)
            MainVM.ShowBackArrow = false;
    }

    public static void ToPasswords()
    {
        ClearStack();
        Navigate(new PasswordsViewModel(), false);
    }

    public static void ToSettings() => Navigate(new SettingsViewModel());

    public static void ToManageDB(object param = null) => Navigate(new ManageDatabaseViewModel());

    public static void ToAboutPage(object param = null) => Navigate(new AboutViewModel());

    public static void ToLoginPage()
    {
        ClearStack();
        Navigate(new LoginViewModel(), false);
    }
}