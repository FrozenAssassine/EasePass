/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper.Security.Generator;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace EasePass.Helper
{
    internal static class PrinterHelper
    {
        public static string SelectedPrinter = "";
        private static ObservableCollection<PasswordManagerItem> items;
        private static int PageIndex = 0;
        private static int ItemIndex = 0;

        private const float LeftSpacing = 1.5f; // cm
        private const float TopSpacing = 1.5f; // cm
        private const float RightSpacing = 1.5f; // cm
        private const float BottomSpacing = 1.5f; // cm

        private static readonly Brush Brush = Brushes.Black;

        private static readonly Font HeadingFont = new Font(FontFamily.GenericSansSerif, 72, FontStyle.Bold);
        private static readonly Font DisplayNameFont = new Font(FontFamily.GenericSansSerif, 13, FontStyle.Bold);
        private static readonly Font BodyFont = new Font(FontFamily.GenericSansSerif, 13);

        public static async void Print(ObservableCollection<PasswordManagerItem> items)
        {
            if (SelectedPrinter == null || SelectedPrinter.Length == 0)
                return;

            await Task.Run(() =>
            {
                PrinterHelper.items = items;
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = SelectedPrinter;
                pd.DefaultPageSettings.Landscape = false;
                pd.PrintPage += Pd_PrintPage;
                pd.EndPrint += Pd_EndPrint;
                PageIndex = 0;
                ItemIndex = 0;
                pd.Print();
            });
        }

        private static void Pd_EndPrint(object sender, PrintEventArgs e)
        {
            items = null;
        }

        private static void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            float dpix = e.Graphics.DpiX / 6;
            float dpiy = e.Graphics.DpiY / 6;
            float width = e.PageBounds.Width / 100 * dpix;
            float height = e.PageBounds.Height / 100 * dpiy;
            float yOffset = Math.Max(CmToInch(TopSpacing) * dpiy, 0/*e.MarginBounds.Top*/);
            float bottomSpace = Math.Max(CmToInch(BottomSpacing) * dpiy, 0/*e.MarginBounds.Bottom*/);
            float leftSpace = Math.Max(CmToInch(LeftSpacing) * dpix, 0/*e.MarginBounds.Left*/);
            float rightSpace = Math.Max(CmToInch(RightSpacing) * dpix, 0/*e.MarginBounds.Right*/);
            if (PageIndex == 0)
            {
                SizeF size = e.Graphics.MeasureString("Ease Pass", HeadingFont);
                e.Graphics.DrawString("Ease Pass", HeadingFont, Brush, new PointF(width / 2 - size.Width / 2, yOffset));
                yOffset += size.Height + dpiy * 0.75f; // add 3/4 inch spacing
            }

            while (ItemIndex < items.Count)
            {
                var item = BuildServiceString(items[ItemIndex]);
                var qrCode = CreateQrCodeFrom2FA(items[ItemIndex]);
                float textHeight = 0;
                SizeF displayNameSize = e.Graphics.MeasureString(item.name, DisplayNameFont);
                textHeight += displayNameSize.Height;
                textHeight += 0.1f * dpiy;
                SizeF bodySize = e.Graphics.MeasureString(item.body, BodyFont);
                textHeight += bodySize.Height;
                if (textHeight > height)
                {
                    InfoMessages.PrinterItemSkipped(item.name);
                    ItemIndex++;
                    continue;
                }
                if (yOffset + textHeight + bottomSpace > height) break;
                float yOffBack = yOffset;
                e.Graphics.DrawString(item.name, DisplayNameFont, Brush, new PointF(leftSpace, yOffset));
                yOffset += displayNameSize.Height;
                yOffset += 0.1f * dpiy;
                e.Graphics.DrawString(item.body, BodyFont, Brush, new PointF(leftSpace, yOffset));
                if (qrCode.status)
                    e.Graphics.DrawImage(qrCode.qrCode, new RectangleF(width - rightSpace - 200, yOffBack, 200, 200));
                yOffset += bodySize.Height;
                yOffset += 0.5f * dpiy; // 1/2 inch spacing between services
                ItemIndex++;
            }

            PageIndex++;
            e.HasMorePages = ItemIndex < items.Count;
        }

        private static float CmToInch(float cm)
        {
            return cm / 2.54f;
        }

        private static (string name, string body) BuildServiceString(PasswordManagerItem item)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(item.Username)) sb.AppendLine("Username:".Localized("PrintDB_Username/Text") + " " + item.Username);
            if (!string.IsNullOrEmpty(item.Email)) sb.AppendLine("E-Mail:".Localized("PrintDB_Email/Text") + " " + item.Email);
            if (!string.IsNullOrEmpty(item.Password)) sb.AppendLine("Password:".Localized("PrintDB_Password/Text") + " " + item.Password);
            if (!string.IsNullOrEmpty(item.Website)) sb.AppendLine("Website:".Localized("PrintDB_Website/Text") + " " + item.Website);
            if (!string.IsNullOrEmpty(item.Secret))
            {
                sb.AppendLine("TOTP-Secret:".Localized("PrintDB_TOTPSecret/Text") + " " + item.Secret);
                sb.AppendLine("TOTP-Algorithm:".Localized("PrintDB_TOTPAlgorithm/Text") + " " + item.Algorithm);
                sb.AppendLine("TOTP-Interval:".Localized("PrintDB_TOTPInterval/Text") + " " + item.Interval);
            }
            if (!string.IsNullOrEmpty(item.Notes))
            {
                sb.AppendLine("Notes:".Localized("PrintDB_Notes/Text"));
                string[] lines = item.Notes.Replace("\n","\n\n").Replace('\r','\n').Replace("\n\n","\n").Split('\n');
                foreach (string line in lines)
                {
                    sb.AppendLine(line);
                }
            }
            return (item.DisplayName, sb.ToString());
        }

        private static (bool status, Bitmap qrCode) CreateQrCodeFrom2FA(PasswordManagerItem item)
        {
            if (string.IsNullOrEmpty(item.Secret))
                return (false, null);

            string codeContent = TOTP.EncodeUrl(item.DisplayName, string.IsNullOrEmpty(item.Username) ? item.Email : item.Username, item.Secret, TOTP.StringToHashMode(item.Algorithm), Convert.ToInt32(item.Digits), Convert.ToInt32(item.Interval));

            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 300,
                Height = 300
            };

            BarcodeWriter writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

            return (true, writer.Write(codeContent));
        }
    }
}
