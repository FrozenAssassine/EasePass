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
        }

        public static StackPanel InfoMessagesPanel;
        public Frame MainFrame => naivgationFrame;

    }
}
