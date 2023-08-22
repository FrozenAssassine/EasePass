using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeScannerFor2FA
{
    public static class ScannerManager
    {
        public static void ShowQRCode(string content)
        {
            var gen = new QRCodeGenerator();
            gen.Show(content);
            gen.ShowDialog();
        }

        public static string ScanWebCam()
        {
            var webcam = new WebcamScanner();
            webcam.ShowDialog();
            return webcam.Result;
        }

        public static string ScanScreen()
        {
            var screen = new ScreenScanner();
            screen.ShowDialog();
            return screen.Result;
        }
    }
}
