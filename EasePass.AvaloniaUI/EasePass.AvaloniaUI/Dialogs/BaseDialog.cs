using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using EasePass.AvaloniaUI;
using EasePass.Views;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

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

public class BaseDialog : Window
{
    private TaskCompletionSource<DialogResult>? _tcs;
    private readonly StackPanel _buttonPanel;
    private readonly ContentControl _contentContainer;

    // Use a unique name to avoid hiding the base Window.Closing event incorrectly
    public delegate void DialogClosingEvent(object? sender, BaseDialogClosingArgs args);
    public event DialogClosingEvent? Closing;

    public DialogResult Result { get; private set; } = DialogResult.None;

    public string? PrimaryButtonText { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? CloseButtonText { get; set; }

    public BaseDialog()
    {
        CanResize = false;
        Width = 450;
        SizeToContent = SizeToContent.Height;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        // Visual Wrapper
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

        base.Content = rootLayout;

        KeyDown += OnKeyDown;
    }

    // Shadowing the original Content property
    public new object? Content
    {
        get => _contentContainer.Content;
        set => _contentContainer.Content = value;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var args = new BaseDialogClosingArgs { Result = this.Result };

        // Trigger our custom event
        Closing?.Invoke(this, args);

        if (args.Cancel)
        {
            e.Cancel = true;
            return;
        }

        base.OnClosing(e);
        _tcs?.TrySetResult(this.Result);
    }

    private void CreateButtons()
    {
        _buttonPanel.Children.Clear();

        if (!string.IsNullOrEmpty(PrimaryButtonText))
            AddButton(PrimaryButtonText, DialogResult.Primary, true);

        if (!string.IsNullOrEmpty(SecondaryButtonText))
            AddButton(SecondaryButtonText, DialogResult.Secondary);

        if (!string.IsNullOrEmpty(CloseButtonText))
            AddButton(CloseButtonText, DialogResult.Cancel);
    }

    private void AddButton(string text, DialogResult result, bool isDefault = false)
    {
        var btn = new Button
        {
            Content = text,
            MinWidth = 80,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };

        btn.Click += (_, __) => { Result = result; Close(); };

        if (isDefault)
        {
            btn.HotKey = new KeyGesture(Key.Enter);
            btn.Classes.Add("accent");
        }

        _buttonPanel.Children.Add(btn);
    }

    public async Task<DialogResult> ShowDialogAsync(Window owner)
    {
        _tcs = new TaskCompletionSource<DialogResult>();
        CreateButtons();

        await ShowDialog(owner);

        return await _tcs.Task;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}