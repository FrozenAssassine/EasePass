using Avalonia;
using Avalonia.Controls;
using EasePass.Helper;
using EasePass.Helper.Window;

namespace EasePass.Core;

public class WindowSizePosState
{
    public WindowState State;
    public PixelPoint Position;
    public Size Size;
}
public class WindowStateManager
{
    public bool IsMinimized => window.WindowState == WindowState.Minimized;
    private Window window;

    private PixelPoint previousWindowPosition;
    private Size previousWindowSize;

    public WindowStateManager(Window window)
    {
        this.window = window;
        previousWindowSize = window.ClientSize;
        previousWindowPosition = window.Position;

        //todo implement OnWindowChanged and 
        this.window.SizeChanged += Window_SizeChanged;
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {

    }

    private void OnWindowChanged()
    {
        if (window.WindowState == WindowState.Normal)
        {
            previousWindowSize = window.ClientSize;
            previousWindowPosition = window.Position;
        }
    }

    public WindowSizePosState GetWindowSizePosStateIndependent()
    {
        throw new System.Exception("Not implemented yet");
        // Return the last restored size/position even if minimized or maximized
        return new WindowSizePosState
        {
            Size = previousWindowSize,
            Position = previousWindowPosition,
            State = window.WindowState
        };
    }
}