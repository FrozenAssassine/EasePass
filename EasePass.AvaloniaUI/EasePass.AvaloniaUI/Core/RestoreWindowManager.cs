using Avalonia.Controls;
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

        //todo: set window state here
    }

    private void SaveSettings()
    {
        var windowPosSize = windowStateManager.GetWindowSizePosStateIndependent();

        //todo do not convert to int here
        AppSettings.WindowWidth = (int)windowPosSize.Size.Width;
        AppSettings.WindowHeight = (int)windowPosSize.Size.Height;
        AppSettings.WindowLeft = windowPosSize.Position.X;
        AppSettings.WindowTop = windowPosSize.Position.Y;
        AppSettings.WindowState = windowPosSize.State;
    }
}
