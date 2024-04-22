using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace EasePass.Helper
{
    internal class WindowHelper
    {
        public static void MakeToolWindow(Window window)
        {
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            //presenter.(false, true);
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
        }
    }
}
