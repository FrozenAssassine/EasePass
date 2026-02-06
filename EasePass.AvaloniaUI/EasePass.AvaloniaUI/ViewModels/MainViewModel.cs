using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Core;
using System.Collections.ObjectModel;
using Avalonia;
using EasePass.Models;

namespace EasePass.AvaloniaUI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Avalonia!";

        [ObservableProperty]
        private ObservableCollection<PasswordManagerItem> _passwordItems = new ObservableCollection<PasswordManagerItem>();

        [RelayCommand]
        public void CopyPassword(string password)
        {
            if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow?.Clipboard?.SetTextAsync(password);
            }
            else if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime singleView)
            {
                // Access clipboard on mobile/browser if possible, or handle differently
                // For now, focusing on desktop behavior or general avalanche clipboard abstraction
                 var topLevel = Avalonia.Controls.TopLevel.GetTopLevel(singleView.MainView);
                 topLevel?.Clipboard?.SetTextAsync(password);
            }
        }
    }
}
