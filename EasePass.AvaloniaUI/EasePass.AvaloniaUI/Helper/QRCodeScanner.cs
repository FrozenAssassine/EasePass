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

using Avalonia.Media.Imaging;
using System;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;

namespace EasePass.Helper
{
    internal class QRCodeScanner
    {
        /// <summary>
        /// Generates a QR code as an Avalonia Bitmap from the given content string.
        /// </summary>
        public static Bitmap? GenerateAvaloniaBitmap(string content)
        {
            try
            {
                var options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = 300,
                    Height = 300,
                    Margin = 1
                };

                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = options
                };

                var pixelData = writer.Write(content);
                
                // Convert pixel data to PNG via manual bitmap construction
                using var ms = new MemoryStream();
                WriteBmpToStream(pixelData, ms);
                ms.Position = 0;
                return new Bitmap(ms);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Writes raw BGRA pixel data as a BMP to a stream (simple cross-platform approach).
        /// </summary>
        private static void WriteBmpToStream(PixelData pixelData, Stream stream)
        {
            int width = pixelData.Width;
            int height = pixelData.Height;
            byte[] pixels = pixelData.Pixels;

            // BMP file format
            int rowSize = ((width * 3 + 3) / 4) * 4; // rows are padded to 4-byte boundaries
            int imageSize = rowSize * height;
            int fileSize = 54 + imageSize;

            using var bw = new BinaryWriter(stream, System.Text.Encoding.Default, true);
            
            // BMP Header
            bw.Write((byte)'B');
            bw.Write((byte)'M');
            bw.Write(fileSize);
            bw.Write(0); // reserved
            bw.Write(54); // offset to pixel data

            // DIB Header (BITMAPINFOHEADER)
            bw.Write(40); // header size
            bw.Write(width);
            bw.Write(height);
            bw.Write((short)1); // color planes
            bw.Write((short)24); // bits per pixel
            bw.Write(0); // compression
            bw.Write(imageSize);
            bw.Write(2835); // horizontal resolution (72 DPI)
            bw.Write(2835); // vertical resolution
            bw.Write(0); // colors in palette
            bw.Write(0); // important colors

            // Pixel data (BMP is bottom-up)
            byte[] row = new byte[rowSize];
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIdx = (y * width + x) * 4; // BGRA
                    row[x * 3 + 0] = pixels[srcIdx + 2]; // B (from R in RGBA)
                    row[x * 3 + 1] = pixels[srcIdx + 1]; // G
                    row[x * 3 + 2] = pixels[srcIdx + 0]; // R (from B in RGBA)
                }
                bw.Write(row);
            }
        }
    }
}
