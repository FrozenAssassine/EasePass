using Avalonia;
using Avalonia.Controls;

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

    public WindowStateManager(Window window)
    {
        this.window = window;
    }


    public WindowSizePosState GetWindowSizePosStateIndependent()
    {
        return new WindowSizePosState
        {
            Size = window.ClientSize,
            Position = window.Position,
            State = window.WindowState
        };
    }
}