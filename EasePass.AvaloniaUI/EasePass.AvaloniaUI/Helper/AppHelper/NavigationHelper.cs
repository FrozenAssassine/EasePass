using EasePass.Views;
using EasePass.Models;
using EasePass.ViewModels;
using System;

namespace EasePass.Helper.AppHelper;

public class NavigationHelper
{
    public static MainViewModel? MainVM { get; set; }
    private static void Navigate(ViewModelBase viewModel)
    {
        if (MainVM != null)
        {
            MainVM.CurrentPage = viewModel;
        }
    }
    public static void ToPasswords() => Navigate(new PasswordsViewModel());
    public static void ToSettings()
    {
        //Navigate(new SettingsViewModel());
    }
    public static void ToManageDB(object param = null)
    {
        //Navigate(typeof(ManageDatabasePage), param);
    }
    public static void ToExtensions(object param = null)
    {

        //Navigate(typeof(ExtensionPage), param);
    }
    public static void ToRegisterPage(object param = null)
    {
        //Navigate(typeof(RegisterPage), param);
    }
    public static void ToLoginPage() => Navigate(new LoginViewModel());
    public static void ToAboutPage(object param = null)
    {
        //Navigate(typeof(AboutPage), param);
    }

}