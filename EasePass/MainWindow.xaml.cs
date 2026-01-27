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

using EasePass.Core;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper.Extension;
using EasePass.Helper.Logout;
using EasePass.Helper.Security.Generator;
using EasePass.Manager;
using EasePass.Views;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace EasePass;

public sealed partial class MainWindow : Window
{
    public InactivityManager inactivityHelper = new InactivityManager();
    public static StackPanel InfoMessagesPanel;
    public Frame MainFrame => navigationFrame;
    public bool ShowBackArrow { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

    public static MainWindow CurrentInstance = null;

    public static DispatcherQueue UIDispatcherQueue = null;
    public static XamlRoot XamlRoot = null;
    public static LocalizationManager localizationHelper = new LocalizationManager();

    public readonly RestoreWindowManager restoreWindowManager;
    public readonly WindowStateManager windowStateManager;
    public readonly ExtensionManager extensionManager;

    public MainWindow()
    {
        this.InitializeComponent();

        CurrentInstance = this;
        UIDispatcherQueue = DispatcherQueue.GetForCurrentThread();

        windowStateManager = new WindowStateManager(this);
        restoreWindowManager = new RestoreWindowManager(this, windowStateManager);
        extensionManager = new ExtensionManager();
        extensionManager.Init();

        restoreWindowManager.RestoreSettings();

        localizationHelper.Initialize();

        Title = Package.Current.DisplayName;
        this.AppWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\AppIcon\\appicon.ico"));

        inactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted;

        PasswordHelper.Init();

        InfoMessagesPanel = infoMessagesPanel;
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(titleBar);
        ShowBackArrow = false;

        this.AppWindow.Closing += AppWindow_Closing;
    }

    private bool _isClosingForcefully = false;

    public async Task DoMasterSaveWithProgress()
    {
        databaseSavingProgressRing.Visibility = Visibility.Visible;

        await Database.LoadedInstance.ForceSaveAsync();

        databaseSavingProgressRing.Visibility = Visibility.Collapsed;
    }

    private async void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        if (_isClosingForcefully) return;

        if (Database.LoadedInstance != null && Database.LoadedInstance.deferredSaver.SaveScheduled)
        {
            args.Cancel = true;

            await DoMasterSaveWithProgress();

            _isClosingForcefully = true;
            this.Close();
        }

        Database.LoadedInstance?.Dispose();
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
