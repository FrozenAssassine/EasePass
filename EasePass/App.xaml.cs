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
using EasePass.Models;
using EasePass.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;

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

            string[] dbPaths = Database.GetAllDatabasePaths();
            if (dbPaths.Length == 1 && !File.Exists(dbPaths[0]))
                NavigationHelper.ToRegisterPage();
            else
                NavigationHelper.ToLoginPage();

            m_window.Activate();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _singleInstanceApp.Launch(args.Arguments);
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
