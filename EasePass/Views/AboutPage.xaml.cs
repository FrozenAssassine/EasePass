using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel;

namespace EasePass.Views
{
    public sealed partial class AboutPage : Page
    {
        public string AppVersion =>
                   Package.Current.Id.Version.Major + "." +
                   Package.Current.Id.Version.Minor + "." +
                   Package.Current.Id.Version.Build;

        public string DeveloperName => Package.Current.PublisherDisplayName;


        public AboutPage()
        {
            this.InitializeComponent();
            App.m_window.ShowBackArrow = true;
        }

        private async void NavigateToLink_Click(Controls.SettingsControl sender)
        {
            if (sender.Tag == null)
                return;

            await Windows.System.Launcher.LaunchUriAsync(new Uri(sender.Tag.ToString()));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.m_window.ShowBackArrow = false;
        }
    }
}
