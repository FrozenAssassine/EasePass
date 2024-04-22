using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EasePass.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Export2FAPage : Page
    {
        public Export2FAPage(string content)
        {
            this.InitializeComponent();
            qrcode.Source = QRCodeScanner.GenerateQRCode(content);
        }
    }
}
