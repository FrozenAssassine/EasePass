using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace EasePass.Helper
{
    internal class QRCodeScanner
    {
        private readonly SoftwareBitmapBarcodeReader _reader;

        public QRCodeScanner()
        {
            _reader = new SoftwareBitmapBarcodeReader
            {
                AutoRotate = true
            };
            _reader.Options.PossibleFormats = new[] { BarcodeFormat.QR_CODE };
            _reader.Options.TryHarder = true;
        }

        public string Scan(SoftwareBitmap bmp)
        {
            var res = _reader.Decode(bmp);
            return res != null ? res.Text : null;
        }

        public static ImageSource GenerateQRCode(string content)
        {
            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 500,
                Height = 500
            };

            BarcodeWriter writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                writer.Write(content).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                bitmapImage.SetSource(stream.AsRandomAccessStream());
            }
            return bitmapImage;
        }
    }

    public class SoftwareBitmapBarcodeReader : BarcodeReader<SoftwareBitmap>
    {
        public SoftwareBitmapBarcodeReader()
            : base(bmp => new SoftwareBitmapLuminanceSource(bmp))
        {
        }
    }

    // from https://github.com/micjahn/ZXing.Net/blob/master/Source/lib/BitmapLuminanceSource.SoftwareBitmap.cs
    public class SoftwareBitmapLuminanceSource : BaseLuminanceSource
    {
        protected SoftwareBitmapLuminanceSource(int width, int height)
          : base(width, height)
        {
        }

        public SoftwareBitmapLuminanceSource(SoftwareBitmap softwareBitmap)
            : base(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight)
        {
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Gray8)
            {
                using SoftwareBitmap convertedSoftwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Gray8);
                convertedSoftwareBitmap.CopyToBuffer(luminances.AsBuffer());
                return;
            }
            softwareBitmap.CopyToBuffer(luminances.AsBuffer());
        }

        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
            => new SoftwareBitmapLuminanceSource(width, height) { luminances = newLuminances };
    }
}
