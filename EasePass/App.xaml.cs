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
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
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

        public static Frame m_frame;
        public static MainWindow m_window;

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
