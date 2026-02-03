using CommunityToolkit.Mvvm.ComponentModel;

namespace EasePass.AvaloniaUI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Avalonia!";
    }
}
