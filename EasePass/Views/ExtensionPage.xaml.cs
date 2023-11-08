using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EasePass.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtensionPage : Page
    {
        public ExtensionPage()
        {
            this.InitializeComponent();
            App.m_window.ShowBackArrow = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.m_window.ShowBackArrow = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.m_window.ShowBackArrow = true;

            extensionView.Items.Clear();
            for (int i = 0; i < ExtensionHelper.Extensions.Count; i++)
                extensionView.Items.Add(ExtensionHelper.Extensions[i]);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Extension extension = (sender as Button).Tag as Extension;
        }

        private async void authorName_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Extension extension = (sender as Button).Tag as Extension;
            await Windows.System.Launcher.LaunchUriAsync(new Uri(extension.AboutPlugin.PluginAuthorURL));
        }

        private void AddExtension_Click(object sender, RoutedEventArgs e)
        {
            string path = ""; // @FrozenAssassine load dll from file picker and copy it to appdata folder
            ExtensionHelper.Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(path)));
            extensionView.Items.Add(ExtensionHelper.Extensions[ExtensionHelper.Extensions.Count - 1]);
        }
    }
}
