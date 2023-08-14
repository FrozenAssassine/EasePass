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
