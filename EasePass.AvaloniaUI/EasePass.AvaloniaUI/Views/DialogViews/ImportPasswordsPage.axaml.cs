using Avalonia.Controls;
using Avalonia.Interactivity;
using EasePass.ViewModels.Dialog;

namespace EasePass.Views.DialogViews;

public partial class ImportPasswordsPage : UserControl
{
    public ImportPasswordsPage()
    {
        InitializeComponent();
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        (DataContext as ImportPasswordsViewModel)?.SelectAll();
    }

    private void DeselectAll_Click(object sender, RoutedEventArgs e)
    {
        (DataContext as ImportPasswordsViewModel)?.DeselectAll();
    }
}
