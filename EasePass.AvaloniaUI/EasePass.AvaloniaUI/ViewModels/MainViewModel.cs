using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Controls;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper.AppHelper;
using EasePass.Helper.Logout;
using EasePass.Helper.Security.Generator;
using EasePass.Manager;
using EasePass.Models.Logger;
using EasePass.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public InactivityManager InactivityHelper { get; } = new();
    public ExtensionManager ExtensionManager { get; } = new();

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private bool _showBackArrow = false;

    [ObservableProperty]
    private bool _isSaving = false;

    public ObservableCollection<NotificationViewModel> Notifications { get; } = new();

    public MainViewModel()
    {
        LoggingManager.Logger = new MultiLogger(new FileLogger(), new DebugLineLogger());
        LoggingManager.InitializeCurrentLogger();

        ExtensionManager.Init();
        PasswordHelper.Init();

        InactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted;
    }

    [RelayCommand]
    public void BackButtonPressed()
    {
        NavigationHelper.GoBack();
    }

    private async void InactivityHelper_InactivityStarted()
    {
        if (!AutoLogoutContentDialog.InactivityStarted())
            return;

        if (Database.LoadedInstance != null)
        {
            await SaveDatabaseAsync();
            LogoutHelper.Logout();
            InfoMessages.AutomaticallyLoggedOut();
        }
    }

    public async Task<bool> SaveDatabaseAsync()
    {
        if (Database.LoadedInstance == null) return true;

        IsSaving = true;

        bool result = await Task.Run(async () => await Database.LoadedInstance.ForceSaveAsync());
        IsSaving = false;
        return result;
    }
}