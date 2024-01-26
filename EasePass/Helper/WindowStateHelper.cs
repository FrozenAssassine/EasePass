using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace EasePass.Helper
{
    internal class WindowStateHelper
    {
        public static OverlappedPresenterState GetWindowState(Window window)
        {
            return (window.AppWindow.Presenter as OverlappedPresenter).State;
        }

        public static void SetWindowState(Window window, OverlappedPresenterState state)
        {
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            if (state == OverlappedPresenterState.Maximized)
                presenter.Maximize();
            else if (state == OverlappedPresenterState.Minimized)
                presenter.Minimize();
        }
    }
}
