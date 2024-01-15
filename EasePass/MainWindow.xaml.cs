using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Settings;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using Windows.ApplicationModel;
using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Dispatching;
using EasePass.Core;

namespace EasePass
{
    public sealed partial class MainWindow : Window
    {
        private InactivityHelper inactivityHelper = new InactivityHelper();
        public PasswordItemsManager passwordItemsManager = null;
        public static StackPanel InfoMessagesPanel;
        public Frame MainFrame => navigationFrame;
        public bool ShowBackArrow { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public static MainWindow CurrentInstance = null;

        public static DispatcherQueue UIDispatcherQueue = null;
        public static XamlRoot XamlRoot = null;

        public MainWindow()
        {
            this.InitializeComponent();

            CurrentInstance = this;
            UIDispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Title = Package.Current.DisplayName;
            this.AppWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\AppIcon\\appicon.ico"));

            ExtensionHelper.Init();
            inactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted;

            PasswordHelper.Init();

            InfoMessagesPanel = infoMessagesPanel;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            ShowBackArrow = false;
            RestoreSettings();

            this.AppWindow.Closing += AppWindow_Closing;
        }

        private void RestoreSettings()
        {
            var windowState = (OverlappedPresenterState)Enum.Parse(typeof(OverlappedPresenterState), AppSettings.GetSettings(AppSettingsValues.windowState, "2"));
            WindowStateHelper.SetWindowState(this, windowState);

            var width = AppSettings.GetSettingsAsInt(AppSettingsValues.windowWidth, 1100);
            var height = AppSettings.GetSettingsAsInt(AppSettingsValues.windowHeight, 700);
            var left = AppSettings.GetSettingsAsInt(AppSettingsValues.windowLeft, -5000);
            var top = AppSettings.GetSettingsAsInt(AppSettingsValues.windowTop, -5000);

            //when closing the window from a minimized state the size will be wrong:
            if (width < 200)
                width = 1100;
            if (height < 100)
                height = 700;

            if (left != -5000 || top != -5000)
            {
                var windowSize = new Windows.Graphics.RectInt32(left, top, width, height);
                this.AppWindow.MoveAndResize(windowSize);
            }
            else
            {
                var windowSize = new Windows.Graphics.SizeInt32(width, height);
                this.AppWindow.Resize(windowSize);
            }
        }
        private void InactivityHelper_InactivityStarted()
        {
            if(this.navigationFrame.CurrentSourcePageType != typeof(LoginPage) &&
                this.navigationFrame.CurrentSourcePageType != typeof(RegisterPage))
            {
                AutoLogoutContentDialog.InactivityStarted();
                LogoutHelper.Logout();
                InfoMessages.AutomaticallyLoggedOut();
                passwordItemsManager.Unload();
            }
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            //save settings:
            AppSettings.SaveSettings(AppSettingsValues.windowWidth, this.AppWindow.Size.Width);
            AppSettings.SaveSettings(AppSettingsValues.windowHeight, this.AppWindow.Size.Height);
            AppSettings.SaveSettings(AppSettingsValues.windowLeft, this.AppWindow.Position.X);
            AppSettings.SaveSettings(AppSettingsValues.windowTop, this.AppWindow.Position.Y);
            AppSettings.SaveSettings(AppSettingsValues.windowState, WindowStateHelper.GetWindowState(this));
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            navigationFrame.GoBack();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                inactivityHelper.WindowDeactivated();
            else
                inactivityHelper.WindowActivated();
        }
    }
}
