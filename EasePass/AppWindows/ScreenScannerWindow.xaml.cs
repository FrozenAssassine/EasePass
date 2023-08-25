using EasePass.Extensions;
using EasePass.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;

namespace EasePass.AppWindows
{
    public sealed partial class ScreenScannerWindow : Window, IScannerWindow
    {
        public string Result { get; set; } = "";

        private Timer timer;
        private QRCodeScanner scanner;

        int width = 0;
        int height = 0;

        public ScreenScannerWindow()
        {
            this.InitializeComponent();
            this.SetTopmost();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(panel);
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(400, 70));
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            // TODO init width and height with screen bounds
            width = 2000;
            height = 2000;

            scanner = new QRCodeScanner();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Size(width, height));
            g.Flush();
            g.Dispose();
            using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                bmp.Save(stream.AsStream(), ImageFormat.Bmp);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                Result = scanner.Scan(softwareBitmap);
                if(Result != "")
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        timer.Stop();
                        this.Close();
                    });
                }
            }
        }

        private void Close_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
