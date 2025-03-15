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

using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Helper.Extension;
using EasePass.Helper.Logout;
using EasePass.Helper.Security.Generator;
using EasePass.Helper.Window;
using EasePass.Settings;
using EasePass.Views;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.ApplicationModel;

namespace EasePass
{
    public sealed partial class MainWindow : Window
    {
        public InactivityHelper inactivityHelper = new InactivityHelper();
        public static StackPanel InfoMessagesPanel;
        public Frame MainFrame => navigationFrame;
        public bool ShowBackArrow { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public static MainWindow CurrentInstance = null;

        public static DispatcherQueue UIDispatcherQueue = null;
        public static XamlRoot XamlRoot = null;
        public static LocalizationHelper localizationHelper = new LocalizationHelper();

        public MainWindow()
        {
            this.InitializeComponent();

            CurrentInstance = this;
            UIDispatcherQueue = DispatcherQueue.GetForCurrentThread();

            localizationHelper.Initialize();

            Title = Package.Current.DisplayName;
            this.AppWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\AppIcon\\appicon.ico"));

            ExtensionHelper.Init();
            inactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted;

            PasswordHelper.Init();

            InfoMessagesPanel = infoMessagesPanel;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            ShowBackArrow = false;
            RestoreSettings();

            this.AppWindow.Closing += AppWindow_Closing;
        }

        private void RestoreSettings()
        {
            WindowStateHelper.SetWindowState(this, AppSettings.WindowState);

            var width = AppSettings.WindowWidth; ;
            var height = AppSettings.WindowHeight; 
            var left = AppSettings.WindowLeft;
            var top = AppSettings.WindowTop;

            //when closing the window from a minimized state the size will be wrong:
            if (width < 200)
                width = 1100;
            if (height < 100)
                height = 700;

            if (left != -5000 || top != -5000)
            {
                var windowSize = new Windows.Graphics.RectInt32(left, top, width, height);
                this.AppWindow.MoveAndResize(windowSize);
            }
            else
            {
                var windowSize = new Windows.Graphics.SizeInt32(width, height);
                this.AppWindow.Resize(windowSize);
            }
        }
        private void InactivityHelper_InactivityStarted()
        {
            if (this.navigationFrame.CurrentSourcePageType != typeof(LoginPage) &&
                this.navigationFrame.CurrentSourcePageType != typeof(RegisterPage))
            {
                //do not trigger auto logout, when there is an important dialog open e.g. edit or add item dialog
                if (!AutoLogoutContentDialog.InactivityStarted())
                    return;
                
                LogoutHelper.Logout();
                InfoMessages.AutomaticallyLoggedOut();
                Database.LoadedInstance.Dispose();
            }
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            //save settings:
            AppSettings.WindowWidth = this.AppWindow.Size.Width;
            AppSettings.WindowHeight = this.AppWindow.Size.Height;
            AppSettings.WindowLeft = this.AppWindow.Position.X;
            AppSettings.WindowTop = this.AppWindow.Position.Y;
            AppSettings.WindowState = WindowStateHelper.GetWindowState(this);
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            navigationFrame.GoBack();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                inactivityHelper.WindowDeactivated();
            else
                inactivityHelper.WindowActivated();
        }
    }
}
