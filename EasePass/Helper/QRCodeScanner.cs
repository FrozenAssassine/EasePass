using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using ZXing;

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
            return res != null ? res.Text : "";
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
