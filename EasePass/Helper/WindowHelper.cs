using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;

namespace EasePass.Helper
{
    internal class WindowHelper
    {
        public static void MakeToolWindow(Window window)
        {
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            presenter.SetBorderAndTitleBar(false, false);
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = true;
        }
    }
}
