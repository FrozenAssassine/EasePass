using EasePass.Extensions;
using EasePass.Helper;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using WindowsDisplayAPI;

namespace EasePass.AppWindows
{
    public sealed partial class ScreenScannerWindow : Window, IScannerWindow
    {
        public string Result { get; set; } = "";

        private DispatcherTimer timer = new DispatcherTimer();
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
            this.AppWindow.Closing += AppWindow_Closing;

            WindowHelper.MakeToolWindow(this);
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO init width and height with screen bounds
            width = 2000;
            height = 2000;

            scanner = new QRCodeScanner();
            timer.Interval = new TimeSpan(0,0,5);
            timer.Tick += Timer_Tick; 
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
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
                if (Result != "")
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        timer.Stop();
                        this.Close();
                    });
                }
            }
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            timer.Stop();
        }
    }
}
