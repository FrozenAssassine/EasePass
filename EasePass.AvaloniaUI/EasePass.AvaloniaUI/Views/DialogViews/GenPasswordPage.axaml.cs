using Avalonia.Controls;
using EasePass.ViewModels.Dialog;
using System.ComponentModel;

namespace EasePass.Views.DialogViews;

public partial class GenPasswordPage : UserControl
{
    public GenPasswordPage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is GenPasswordViewModel vm)
        {
            vm.PropertyChanged += Vm_PropertyChanged;
            if (!string.IsNullOrEmpty(vm.GeneratedPassword))
                safetyChart.EvaluatePassword(vm.GeneratedPassword);
        }
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GenPasswordViewModel.GeneratedPassword))
        {
            var vm = DataContext as GenPasswordViewModel;
            if (vm != null && !string.IsNullOrEmpty(vm.GeneratedPassword))
                safetyChart.EvaluatePassword(vm.GeneratedPassword);
        }
    }
}
