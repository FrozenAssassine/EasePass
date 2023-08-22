using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace QRCodeScannerFor2FA
{
    public class Scanner
    {
        BarcodeReader reader;

        public Scanner()
        {
            reader = new BarcodeReader();
            reader.AutoRotate = true;
        }

        public string Scan(Bitmap bitmap)
        {
            if (bitmap == null) return "";
            Result res = reader.Decode(bitmap);
            return res == null ? "" : res.ToString();
        }
    }
}
