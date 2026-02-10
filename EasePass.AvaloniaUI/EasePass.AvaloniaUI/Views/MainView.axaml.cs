using Avalonia;
using Avalonia.Controls;
using EasePass.Services;
using EasePass.Helper.AppHelper;

namespace EasePass.AvaloniaUI;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Loaded += MainView_Loaded;
    }

    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Loaded -= MainView_Loaded;
        NavigationHelper.ToLoginPage();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            App.VisualRoot = topLevel;
            App.StorageProvider = topLevel.StorageProvider;
            App.Clipboard = topLevel.Clipboard;

            DialogService.topLevel = topLevel;
        }
    }
}