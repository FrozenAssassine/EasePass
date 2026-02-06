using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Helper.App;

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

    public static void ToSettings(PasswordsViewModel pwVM)
    {
        Navigate(new SettingsViewModel { PasswordViewModel = pwVM });
    }
    public static void ToSettings(PasswordsPage pwPage)
    {
        Navigate(typeof(SettingsPage), new SettingsNavigationParameters
        {
            PasswordPage = pwPage
        });
    }
    public static void ToManageDB(object param = null)
    {
        Navigate(typeof(ManageDatabasePage), param);
    }
    public static void ToExtensions(object param = null)
    {

        Navigate(typeof(ExtensionPage), param);
    }
    public static void ToRegisterPage(object param = null)
    {
        Navigate(typeof(RegisterPage), param);
    }
    public static void ToLoginPage(object param = null)
    {
        Navigate(typeof(LoginPage), param);
    }
    public static void ToAboutPage(object param = null)
    {
        Navigate(typeof(AboutPage), param);
    }
}