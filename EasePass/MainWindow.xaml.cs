using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EasePass
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            InfoMessagesPanel = infoMessagesPanel;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            ShowBackArrow = false;

            var windowSize = new Windows.Graphics.SizeInt32(AppSettings.GetSettingsAsInt(AppSettingsValues.windowWidth, 1100), AppSettings.GetSettingsAsInt(AppSettingsValues.windowHeight, 700));
            this.AppWindow.Resize(windowSize);

            this.AppWindow.Closing += AppWindow_Closing;
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            AppSettings.SaveSettings(AppSettingsValues.windowWidth, this.AppWindow.Size.Width);
            AppSettings.SaveSettings(AppSettingsValues.windowHeight, this.AppWindow.Size.Height);
        }

        public static StackPanel InfoMessagesPanel;
        public Frame MainFrame => naivgationFrame;

        public bool ShowBackArrow { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            naivgationFrame.GoBack();
        }
    }
}
