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
using EasePass.Helper;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace EasePass
{
    public partial class App : Application
    {
        public static Frame m_frame;
        public static MainWindow m_window;
        private readonly SingleInstanceDesktopApp _singleInstanceApp;

        public App()
        {
            this.InitializeComponent();

            _singleInstanceApp = new SingleInstanceDesktopApp("easepass.juliuskirsch");
            _singleInstanceApp.Launched += _singleInstanceApp_Launched;
        }

        private void _singleInstanceApp_Launched(object sender, SingleInstanceLaunchEventArgs e)
        {
            m_window = new MainWindow();
            m_frame = m_window.MainFrame;
            m_frame.NavigationFailed += OnNavigationFailed;

            AppActivationArguments appActivationArguments = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (appActivationArguments.Kind is ExtendedActivationKind.File &&
                appActivationArguments.Data is IFileActivatedEventArgs fileActivatedEventArgs &&
                fileActivatedEventArgs.Files.FirstOrDefault() is IStorageFile storageFile)
            {
                NavigationHelper.ToLoginPage(storageFile.Path);

                m_window.Activate();
                return;
            }

            if (Database.HasDatabasePath())
                NavigationHelper.ToLoginPage();
            else
                NavigationHelper.ToRegisterPage();

            m_window.Activate();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _singleInstanceApp.Launch(args.Arguments);
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
