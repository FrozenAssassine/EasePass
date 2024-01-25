﻿using EasePass.Views;

namespace EasePass.Helper;

internal class LogoutHelper
{
    public static void Logout()
    {
        App.m_window.database.Dispose();
        App.m_frame.Navigate(typeof(LoginPage));
    }
}