using EasePass.Models;
using EasePass.Views;

namespace EasePass.Helper;

internal class LogoutHelper
{
    public static void Logout()
    {
        Database.LoadedInstance.Dispose();
        App.m_frame.Navigate(typeof(LoginPage));
    }
}