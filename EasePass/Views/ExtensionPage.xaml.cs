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
using EasePass.Helper;
using EasePass.Models;
using EasePassExtensibility;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace EasePass.Views;

public sealed partial class ExtensionPage : Page
{
    public ExtensionPage()
    {
        this.InitializeComponent();
        App.m_window.ShowBackArrow = true;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        App.m_window.ShowBackArrow = false;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        App.m_window.ShowBackArrow = true;

        FetchAndLoadExtensions();

        extensionView.Items.Clear();
        for (int i = 0; i < ExtensionHelper.Extensions.Count; i++)
            extensionView.Items.Add(ExtensionHelper.Extensions[i]);
    }
    public void FetchAndLoadExtensions()
    {
        var result = ExtensionHelper.GetExtensionsFromSources();

        if (result == null || result.Count == 0)
        {
            downloadView.ItemsSource = null;
            hintBox.Visibility = Visibility.Visible;
        }
        else
        {
            downloadView.ItemsSource = result;
            hintBox.Visibility = Visibility.Collapsed;
        }
    }


    private async void Uninstall_Click(object sender, RoutedEventArgs e)
    {
        Extension extension = (sender as MenuFlyoutItem).Tag as Extension;
        DeleteExtensionConfirmationDialog dialog = new DeleteExtensionConfirmationDialog();
        if (await dialog.ShowAsync(extension))
        {
            ExtensionHelper.Extensions.Remove(extension);
            extensionView.Items.Remove(extension);
            File.AppendAllLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", new string[] { extension.ID });

            FetchAndLoadExtensions();
        }
    }

    private async void authorName_Tapped(object sender, TappedRoutedEventArgs e)
    {
        Extension extension = (sender as TextBlock).Tag as Extension;
        string url = extension.AboutPlugin.PluginAuthorURL;
        if (!string.IsNullOrEmpty(url))
            await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
    }

    private async void AddExtension_Click(object sender, RoutedEventArgs e)
    {
        var res = await FilePickerHelper.PickOpenFile(new string[] { ".dll" });
        if (!res.success)
            return;

        string path = res.path;

        var destinationPath = ApplicationData.Current.LocalFolder.Path + "\\extensions\\";
        if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
        string p = destinationPath + Guid.NewGuid().ToString().Replace('-', '_').Replace(' ', '_') + ".dll";
        File.Copy(path, p);
        string hashCode = HashHelper.HashFile(p);
        string hashfilename = System.IO.Path.GetDirectoryName(p) + "\\" + hashCode + ".dll";

        if (File.Exists(hashfilename))
        {
            List<string> deleteExtensions = new List<string>();
            deleteExtensions.AddRange(File.ReadAllLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"));
            int index = deleteExtensions.IndexOf(hashCode);
            if (index >= 0)
            {
                deleteExtensions.RemoveAt(index);
                File.WriteAllLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", deleteExtensions);
                InstallExtensionFromFile(hashfilename);
                File.Delete(p);
                return;
            }
            else
            {
                InfoMessages.ExtensionAlreadyInstalled();
                File.Delete(p);
                return;
            }
        }
        try
        {
            File.Move(p, hashfilename);
        }
        catch
        {
            InfoMessages.PluginMovedWhileInstallingLocal();
            return;
        }

        InstallExtensionFromFile(hashfilename);
    }

    private async void ShowRequestedAuthorizations_Click(object sender, RoutedEventArgs e)
    {
        Extension extension = (sender as MenuFlyoutItem).Tag as Extension;
        await new InfoDialog().ShowAsync(extension.ToString(), extension.AboutPlugin.PluginName);
    }
    private async void fetchedExtensionsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (downloadView.SelectedItem == null)
            return;

        if (downloadView.SelectedItem is FetchedExtension fetchedExtension)
        {
            var destinationPath = ApplicationData.Current.LocalFolder.Path + "\\extensions\\";
            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
            string p = destinationPath + Guid.NewGuid().ToString().Replace('-', '_').Replace(' ', '_') + ".dll";

            var downloadInfoBar = InfoMessages.DownloadingPluginInfo();

            await Task.Run(async () =>
            {
                var res = await RequestsHelper.DownloadFileAsync(fetchedExtension.URL, p, 5000, (msg)=> InfoMessages.CouldNotGetExtensions(msg));
                if (!res) //error while downloading plugin to file.
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        downloadInfoBar.IsOpen = false;
                    });
                    return;
                }

                string hashCode = HashHelper.HashFile(p);
                string hashfilename = System.IO.Path.GetDirectoryName(p) + "\\" + hashCode + ".dll";
                if (File.Exists(hashfilename))
                {
                    List<string> deleteExtensions = new List<string>();
                    deleteExtensions.AddRange(File.ReadAllLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"));
                    int index = deleteExtensions.IndexOf(hashCode);
                    if (index >= 0)
                    {
                        deleteExtensions.RemoveAt(index);
                        File.WriteAllLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", deleteExtensions);
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            InstallExtensionFromFile(hashfilename);
                            downloadInfoBar.IsOpen = false;
                        });

                        File.Delete(p);
                        return;
                    }
                    else
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            InfoMessages.ExtensionAlreadyInstalled();
                            downloadInfoBar.IsOpen = false;
                        });

                        File.Delete(p);
                        return;
                    }
                }
                File.Move(p, hashfilename);

                DispatcherQueue.TryEnqueue(() =>
                {
                    InstallExtensionFromFile(hashfilename);
                    downloadInfoBar.IsOpen = false;
                });
            });
        }
    }
    private async void InstallExtensionFromFile(string p)
    {
        Extension extension = new Extension(ReflectionHelper.GetAllExternalInstances(p), System.IO.Path.GetFileNameWithoutExtension(p));
        if (extension.Interfaces.Length == 0
            || (extension.Interfaces.Where((ext) => { return ext is IWarning; }).Count() > 0
            && !(await new SensitiveDataDialog().Dialog(extension))))
        {
            File.AppendAllLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", new string[] { extension.ID });
            if (extension.Interfaces.Length == 0)
                InfoMessages.FileIsNotAnExtensions();
            return;
        }
        ExtensionHelper.Extensions.Add(extension);
        extensionView.Items.Add(ExtensionHelper.Extensions[ExtensionHelper.Extensions.Count - 1]);

        FetchAndLoadExtensions();
    }

    private async void OpenWebsite_Click(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/FrozenAssassine/EasePass/tree/master/Plugins"));
    }
}
