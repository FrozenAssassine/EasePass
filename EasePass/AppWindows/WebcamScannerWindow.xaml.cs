using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using EasePass.Helper;

namespace EasePass.AppWindows
{
    public sealed partial class WebcamScannerWindow : Window, IScannerWindow // https://stackoverflow.com/questions/76956862/how-to-scan-a-qr-code-in-winui-3-using-webcam?noredirect=1#comment135668044_76956862
    {
        QRCodeScanner scanner;
        private MediaCapture _capture;
        private MediaFrameReader _frameReader;
        private MediaSource _mediaSource;

        private bool closing = false;

        public string Result { get; set; } = "";

        public WebcamScannerWindow()
        {
            this.InitializeComponent();

            scanner = new QRCodeScanner();
        }

        private async void Close_Btn_Click(object sender, RoutedEventArgs e)
        {
            closing = true;
            await TerminateCaptureAsync();
            this.Close();
        }

        private async Task LoadCameras()
        {
            cameras.Items.Clear();
            foreach(MediaFrameSourceGroup source in await MediaFrameSourceGroup.FindAllAsync())
            {
                cameras.Items.Add(source.DisplayName);
            }
            cameras.SelectedIndex = 0;
        }

        private async Task InitializeCaptureAsync()
        {
            if (!closing)
            {
                if (_capture != null) await TerminateCaptureAsync();

                // get first capture device (change this if you want)
                var sourceGroup = (await MediaFrameSourceGroup.FindAllAsync())[cameras.SelectedIndex];
                if (sourceGroup == null)
                    return; // not found!

                // init capture & initialize
                _capture = new MediaCapture();
                await _capture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    SourceGroup = sourceGroup,
                    SharingMode = MediaCaptureSharingMode.SharedReadOnly,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu, // to ensure we get SoftwareBitmaps
                });

                // initialize source
                var source = _capture.FrameSources[sourceGroup.SourceInfos[0].Id];

                // create reader to get frames & pass reader to player to visualize the webcam
                _frameReader = await _capture.CreateFrameReaderAsync(source, MediaEncodingSubtypes.Bgra8);
                _frameReader.FrameArrived += OnFrameArrived;
                await _frameReader.StartAsync();

                _mediaSource = MediaSource.CreateFromMediaFrameSource(source);
                player.Source = _mediaSource;
            }
        }

        private void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (!closing)
            {
                var bmp = sender.TryAcquireLatestFrame()?.VideoMediaFrame?.SoftwareBitmap;
                if (bmp == null)
                    return;

                string result = scanner.Scan(bmp);
                if (result != "")
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        this.Result = result;
                        closing = true;
                        await TerminateCaptureAsync();
                        this.Close();
                    });
                }
            }
        }

        private async Task TerminateCaptureAsync()
        {
            player.Source = null;

            _mediaSource?.Dispose();
            _mediaSource = null;

            if (_frameReader != null)
            {
                _frameReader.FrameArrived -= OnFrameArrived;
                await _frameReader.StopAsync();
                _frameReader?.Dispose();
                _frameReader = null;
            }

            _capture?.Dispose();
            _capture = null;
        }

        private async void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            await LoadCameras();
            await InitializeCaptureAsync();
        }

        private async void cameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await InitializeCaptureAsync();
        }
    }
}
