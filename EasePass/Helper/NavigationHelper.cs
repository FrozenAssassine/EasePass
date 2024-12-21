using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Helper;

public class NavigationHelper
{
    private static void Navigate(System.Type page, object param = null)
    {
        App.m_frame.Navigate(page, param);
    }
    public static void ToPasswords(object param = null)
    {
        Navigate(typeof(PasswordsPage), param);
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

        Navigate(typeof(ManageDatabasePage), param);
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