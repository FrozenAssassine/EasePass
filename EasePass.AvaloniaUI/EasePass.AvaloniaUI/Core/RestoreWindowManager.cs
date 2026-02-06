using Avalonia.Controls;
using EasePass.Helper.Window;
using EasePass.Settings;

namespace EasePass.Core;

public class RestoreWindowManager
{
    private Window window;
    private WindowStateManager windowStateManager;

    public RestoreWindowManager(Window window, WindowStateManager windowStateManager)
    {
        this.window = window;
        this.windowStateManager = windowStateManager;
        window.Closing += Window_Closing; ;
    }

    private void Window_Closing(object? sender, WindowClosingEventArgs e)
    {
        SaveSettings();
    }

    public void RestoreSettings()
    {
        var width = AppSettings.WindowWidth;
        var height = AppSettings.WindowHeight;
        var left = AppSettings.WindowLeft;
        var top = AppSettings.WindowTop;

        if (width < 200)
            width = 1100;
        if (height < 100)
            height = 700;

        window.Position = new Avalonia.PixelPoint(left, top);
        window.Width = width;
        window.Height = height;
        WindowStateHelper.SetWindowState(window, AppSettings.WindowState);
    }

    private void SaveSettings()
    {
        var windowPosSize = windowStateManager.GetWindowSizePosStateIndependent();

        AppSettings.WindowWidth = windowPosSize.size.Width;
        AppSettings.WindowHeight = windowPosSize.size.Height;
        AppSettings.WindowLeft = windowPosSize.position.X;
        AppSettings.WindowTop = windowPosSize.position.Y;
        AppSettings.WindowState = windowPosSize.state;
    }
}
