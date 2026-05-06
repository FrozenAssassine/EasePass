using Avalonia.Controls;
using Avalonia.Media;
using EasePass.Extensions;
using EasePass.ViewModels;
using System.ComponentModel;

namespace EasePass.Controls;

public partial class NotificationBar : UserControl
{
    public NotificationBar()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is NotificationViewModel vm)
        {
            ApplySeverityColors(vm.Severity);
            vm.PropertyChanged += Vm_PropertyChanged;
        }
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NotificationViewModel.Severity) && DataContext is NotificationViewModel vm)
            ApplySeverityColors(vm.Severity);
    }

    private void ApplySeverityColors(InfoBarSeverity severity)
    {
        var border = this.FindControl<Border>("ContainerBorder");
        if (border == null) return;

        switch (severity)
        {
            case InfoBarSeverity.Success:
                border.Background = new SolidColorBrush(Color.FromRgb(25, 70, 35));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(45, 120, 60));
                break;
            case InfoBarSeverity.Error:
                border.Background = new SolidColorBrush(Color.FromRgb(80, 20, 20));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(140, 40, 40));
                break;
            case InfoBarSeverity.Warning:
                border.Background = new SolidColorBrush(Color.FromRgb(90, 70, 15));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 120, 30));
                break;
            case InfoBarSeverity.Informational:
            default:
                border.Background = new SolidColorBrush(Color.FromRgb(26, 58, 92));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(42, 96, 144));
                break;
        }
    }
}