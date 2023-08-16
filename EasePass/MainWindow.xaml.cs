using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Settings;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EasePass
{
    public sealed partial class MainWindow : Window
    {
        private InactivityHelper inactivityHelper = new InactivityHelper();

        public static StackPanel InfoMessagesPanel;
        public Frame MainFrame => naivgationFrame;
        public bool ShowBackArrow { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }


        public MainWindow()
        {
            this.InitializeComponent();

            inactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted; 

            InfoMessagesPanel = infoMessagesPanel;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            ShowBackArrow = false;
            
            var width = AppSettings.GetSettingsAsInt(AppSettingsValues.windowWidth, 1100);
            var height = AppSettings.GetSettingsAsInt(AppSettingsValues.windowHeight, 700);

            //when closing the window from a minimized state the size will be wrong:
            if (width < 200)
                width = 1100;
            if (height < 100)
                height = 700;

            var windowSize = new Windows.Graphics.SizeInt32(width, height);
            this.AppWindow.Resize(windowSize);

            this.AppWindow.Closing += AppWindow_Closing;
        }

        private void InactivityHelper_InactivityStarted()
        {
            if(this.naivgationFrame.CurrentSourcePageType != typeof(LoginPage) &&
                this.naivgationFrame.CurrentSourcePageType != typeof(RegisterPage))
            {
                this.naivgationFrame.Navigate(typeof(LoginPage));
                InfoMessages.AutomaticallyLoggedOut();
            }
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            AppSettings.SaveSettings(AppSettingsValues.windowWidth, this.AppWindow.Size.Width);
            AppSettings.SaveSettings(AppSettingsValues.windowHeight, this.AppWindow.Size.Height);
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            naivgationFrame.GoBack();
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
