using EasePass.Helper;
using EasePass.Settings;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace EasePass
{
    public partial class App : Application
    {
        public static Frame m_frame;
        public static MainWindow m_window;
        private readonly SingleInstanceDesktopApp _singleInstanceApp;

        public App()
        {
            this.InitializeComponent();

            _singleInstanceApp = new SingleInstanceDesktopApp("easepass.juliuskirsch");
            _singleInstanceApp.Launched += _singleInstanceApp_Launched; ;
        }

        private void _singleInstanceApp_Launched(object sender, SingleInstanceLaunchEventArgs e)
        {
            m_window = new MainWindow();

            m_frame = m_window.MainFrame;
            m_frame.NavigationFailed += OnNavigationFailed;

            var pwHash = AppSettings.GetSettings(AppSettingsValues.pHash, "");
            if (pwHash.Length <= 0)
                m_frame.Navigate(typeof(RegisterPage));
            else
                m_frame.Navigate(typeof(LoginPage));

            m_window.Activate();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _singleInstanceApp.Launch(args.Arguments);
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
