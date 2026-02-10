using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using EasePass.AvaloniaUI;
using EasePass.Core;
using EasePass.Core.Database;
using EasePass.Helper.AppHelper;
using EasePass.Manager;
using EasePass.Services;
using EasePass.ViewModels;
using System;

namespace EasePass.Views
{
    //this window is only visible on desktop, so core instances we need
    //are created inside the app.axaml.cs.
    public partial class MainWindow : Window
    {
        private readonly WindowStateManager _windowStateManager;
        private readonly RestoreWindowManager _restoreWindowManager;

        public MainWindow()
        {
            InitializeComponent();

            App.VisualRoot = this;
            App.StorageProvider = this.StorageProvider;
            App.Clipboard = this.Clipboard;

            _windowStateManager = new WindowStateManager(this);
            _restoreWindowManager = new RestoreWindowManager(this, _windowStateManager);
            _restoreWindowManager.RestoreSettings();

            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Start at Login
            NavigationHelper.ToLoginPage();
        }

        private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;

            if (Database.LoadedInstance != null && Database.LoadedInstance.deferredSaver.SaveScheduled)
            {
                e.Cancel = true;

                bool saved = await vm.SaveDatabaseAsync();
                if (saved)
                {
                    Database.LoadedInstance?.Dispose();
                    this.Close();
                }
            }
        }
    }
}