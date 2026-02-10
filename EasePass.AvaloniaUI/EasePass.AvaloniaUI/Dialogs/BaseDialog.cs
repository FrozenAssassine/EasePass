using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using EasePass.AvaloniaUI;
using EasePass.Views;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Dialogs;


//TODO: Use Proper dialog host for android and desktop: https://github.com/AvaloniaUtils/DialogHost.Avalonia/

public enum DialogResult
{
    None,
    Primary,
    Secondary,
    Cancel
}
public class BaseDialogClosingArgs
{
    public DialogResult Result { get; set; }
    public bool Cancel { get; set; }
}

public class BaseDialog : UserControl
{
    private TaskCompletionSource<DialogResult>? _tcs;
    private readonly StackPanel _buttonPanel;
    private readonly ContentControl _contentContainer;
    private Window? _hostWindow;
    private Grid? _overlayWrapper;

    public delegate void DialogClosingEvent(object? sender, BaseDialogClosingArgs args);
    public event DialogClosingEvent? Closing;

    public DialogResult Result { get; private set; } = DialogResult.None;
    public string? PrimaryButtonText { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? CloseButtonText { get; set; }
    public string? Title { get; set; }

    public BaseDialog()
    {
        // Visual Setup
        _buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 10,
            Margin = new Thickness(20)
        };

        _contentContainer = new ContentControl();

        var rootLayout = new DockPanel();
        DockPanel.SetDock(_buttonPanel, Dock.Bottom);
        rootLayout.Children.Add(_buttonPanel);
        rootLayout.Children.Add(_contentContainer);

        this.Content = rootLayout;
        this.Background = Brushes.Transparent;
    }

    public new object? Content
    {
        get => _contentContainer.Content;
        set => _contentContainer.Content = value;
    }

    private void CreateButtons()
    {
        _buttonPanel.Children.Clear();
        if (!string.IsNullOrEmpty(PrimaryButtonText)) AddButton(PrimaryButtonText, DialogResult.Primary, true);
        if (!string.IsNullOrEmpty(SecondaryButtonText)) AddButton(SecondaryButtonText, DialogResult.Secondary);
        if (!string.IsNullOrEmpty(CloseButtonText)) AddButton(CloseButtonText, DialogResult.Cancel);
    }

    private void AddButton(string text, DialogResult result, bool isDefault = false)
    {
        var btn = new Button { Content = text, MinWidth = 80 };
        btn.Click += (_, _) => Close(result);
        if (isDefault) btn.Classes.Add("accent");
        _buttonPanel.Children.Add(btn);
    }

    public async Task<DialogResult> ShowDialogAsync(TopLevel owner)
    {
        _tcs = new TaskCompletionSource<DialogResult>();
        CreateButtons();

        if (owner is Window desktopWindow)
        {
            // DESKTOP LOGIC
            _hostWindow = new Window
            {
                Content = this,
                Title = Title,
                SizeToContent = SizeToContent.Height,
                Width = 450,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _hostWindow.Closing += (s, e) =>
            {
                var args = new BaseDialogClosingArgs { Result = this.Result };
                Closing?.Invoke(this, args);
                if (args.Cancel) e.Cancel = true;
            };
            await _hostWindow.ShowDialog(desktopWindow);
        }
        else
        {
            // ANDROID LOGIC (Overlay)
            var overlayLayer = owner.GetVisualDescendants().OfType<OverlayLayer>().FirstOrDefault();
            if (overlayLayer != null)
            {
                _overlayWrapper = new Grid
                {
                    Background = new SolidColorBrush(Color.Parse("#80000000")),
                    Children = { new Border {
                        Background = Brushes.White,
                        CornerRadius = new CornerRadius(10),
                        Margin = new Thickness(20),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        MaxWidth = 500,
                        Child = this
                    }}
                };
                overlayLayer.Children.Add(_overlayWrapper);
            }
        }

        return await _tcs.Task;
    }

    public void Close(DialogResult result)
    {
        this.Result = result;
        var args = new BaseDialogClosingArgs { Result = result };
        Closing?.Invoke(this, args);
        if (args.Cancel) return;

        if (_hostWindow != null)
        {
            _hostWindow.Close();
        }
        else if (_overlayWrapper != null)
        {
            var overlayLayer = _overlayWrapper.Parent as OverlayLayer;
            overlayLayer?.Children.Remove(_overlayWrapper);
        }

        _tcs?.TrySetResult(result);
    }
}