using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace EasePass.Helper
{
    internal class ClipboardHelper
    {
        public static void Copy(string text)
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);
        }
    }
}
