using CommunityToolkit.Mvvm.ComponentModel;

namespace EasePass.ViewModels;

public partial class ViewModelBase : ObservableObject { }

public partial class LoginViewModel : ViewModelBase { }
public partial class HomeViewModel : ViewModelBase { }
public partial class SettingsViewModel : ViewModelBase { }

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainViewModel()
    {
        // Start at Login
        _currentPage = new LoginViewModel();
    }

    public void NavigateToHome() => CurrentPage = new HomeViewModel();
    public void NavigateToSettings() => CurrentPage = new SettingsViewModel();
    public void NavigateToLogin() => CurrentPage = new LoginViewModel();

}