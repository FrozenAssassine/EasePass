using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

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
