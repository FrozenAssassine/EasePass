using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using EasePass.AvaloniaUI;
using EasePass.Core;
using EasePass.Core.Database;
using EasePass.Helper.App;
using EasePass.Manager;
using EasePass.Services;
using EasePass.ViewModels;
using System;

namespace EasePass.Views
{
    public partial class MainWindow : Window
    {
        private readonly WindowStateManager _windowStateManager;
        private readonly RestoreWindowManager _restoreWindowManager;
        public static IStorageProvider storageProvider { get; private set; }
        public static IClipboard clipboardInst { get; private set; }
        public MainWindow()
        {
            InitializeComponent();

            storageProvider = this.StorageProvider;
            clipboardInst = this.Clipboard;
            DialogService.MainWindow = this;

            DataContext = new MainViewModel();

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