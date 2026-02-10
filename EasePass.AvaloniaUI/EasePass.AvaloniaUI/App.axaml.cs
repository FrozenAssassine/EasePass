using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using EasePass.Helper.AppHelper;
using EasePass.Settings;
using EasePass.ViewModels;
using EasePass.Views;
using System.Linq;

namespace EasePass.AvaloniaUI
{
    public partial class App : Application
    {
        public static IStorageProvider? StorageProvider { get; set; }
        public static IClipboard? Clipboard { get; set; }
        public static TopLevel? VisualRoot { get; set; }
        public static MainViewModel MainVM;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ApplicationData.Initialize();
            var mainVM = new MainViewModel();
            MainVM = mainVM;
            NavigationHelper.MainVM = mainVM;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //desktop
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainVM
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                //android/ios
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainVM
                };
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}