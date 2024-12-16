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

using EasePass.Helper;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.MediaProperties;

namespace EasePass.AppWindows
{
    public sealed partial class WebcamScannerWindow : Window, IScannerWindow // https://stackoverflow.com/questions/76956862/how-to-scan-a-qr-code-in-winui-3-using-webcam?noredirect=1#comment135668044_76956862
    {
        QRCodeScanner scanner;
        private MediaCapture _capture;
        private MediaFrameReader _frameReader;
        private MediaSource _mediaSource;
        public string Result { get; set; } = "";

        public WebcamScannerWindow()
        {
            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(titlebar);
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(620, 660));
            this.AppWindow.Closing += AppWindow_Closing;
            InteropHelper.SetWindowLongPtr(InteropHelper.GetWindowHandle(this), InteropHelper.GWLP_HWNDPARENT, InteropHelper.GetWindowHandle(MainWindow.CurrentInstance));
            (this.AppWindow.Presenter as OverlappedPresenter).IsModal = true;
            scanner = new QRCodeScanner();
            WindowHelper.MakeToolWindow(this);
        }

        private async void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            await TerminateCaptureAsync();
        }

        private async Task LoadCameras()
        {
            cameras.Items.Clear();
            foreach (MediaFrameSourceGroup source in await MediaFrameSourceGroup.FindAllAsync())
            {
                cameras.Items.Add(source.DisplayName);
            }
            cameras.SelectedIndex = 0;
        }

        private async Task InitializeCaptureAsync()
        {
            try
            {
                if (_capture != null) await TerminateCaptureAsync();

                // check for available camera devices
                var mediaframes = await MediaFrameSourceGroup.FindAllAsync();
                if (mediaframes.Count == 0 || cameras.SelectedIndex < 0)
                    return;

                var sourceGroup = mediaframes[cameras.SelectedIndex];
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
            catch { }
        }

        private async void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var bmp = sender.TryAcquireLatestFrame()?.VideoMediaFrame?.SoftwareBitmap;
            if (bmp == null)
                return;

            string result = scanner.Scan(bmp);
            if (result != null)
            {
                this.Result = result;
                _frameReader.FrameArrived -= OnFrameArrived;
                await TerminateCaptureAsync();

                DispatcherQueue.TryEnqueue(() =>
                {
                    this.Close();
                });
            }
        }

        private async Task TerminateCaptureAsync()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                player.Source = null;
            });

            _mediaSource?.Dispose();
            _mediaSource = null;

            _frameReader.FrameArrived -= OnFrameArrived;
            await _frameReader.StopAsync();
            _frameReader.Dispose();

            _capture.Dispose();
            _capture = null;
        }

        private async void cameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cameras.SelectedIndex == -1)
                return;
            if (_capture != null)
                await TerminateCaptureAsync();

            await InitializeCaptureAsync();
        }

        private async void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCameras();
            //await InitializeCaptureAsync();
        }
    }
}
