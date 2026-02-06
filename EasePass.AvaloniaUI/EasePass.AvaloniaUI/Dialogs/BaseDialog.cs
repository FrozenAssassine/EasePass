using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

public enum DialogResult
{
    None,
    Primary,
    Secondary,
    Cancel
}

public class BaseDialog : Window
{
    private TaskCompletionSource<DialogResult>? _tcs;
    private readonly StackPanel _buttonPanel;
    private readonly ContentControl _contentContainer;

    public DialogResult Result { get; private set; } = DialogResult.None;

    // Button Text Properties
    public string? PrimaryButtonText { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? CloseButtonText { get; set; }

    public BaseDialog()
    {
        CanResize = false;
        Width = 450;
        SizeToContent = SizeToContent.Height;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        // Setup the UI Wrapper
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

        KeyDown += OnKeyDown;
        Closing += (_, __) => _tcs?.TrySetResult(Result);
    }

    // Shadowing the original Content property to place it in our container
    public new object? Content
    {
        get => _contentContainer.Content;
        set => _contentContainer.Content = value;
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
            // Enter key triggers this
            btn.HotKey = new KeyGesture(Key.Enter);
        }

        _buttonPanel.Children.Add(btn);
    }

    public async Task<DialogResult> ShowDialogAsync(Window owner)
    {
        _tcs = new TaskCompletionSource<DialogResult>();
        CreateButtons();
        await ShowDialog(owner);
        return _tcs.Task.IsCompleted ? await _tcs.Task : Result;
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