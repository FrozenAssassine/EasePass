using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.IO;
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

        await FetchAndLoadExtensions();

        extensionView.Items.Clear();
        for (int i = 0; i < ExtensionHelper.Extensions.Count; i++)
            extensionView.Items.Add(ExtensionHelper.Extensions[i]);
    }
    public async Task FetchAndLoadExtensions()
    {
        var result = await ExtensionHelper.GetExtensionsFromServer();
        if (result == null)
            return;

        downloadView.ItemsSource = result;
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
        }
    }

    private async void authorName_Tapped(object sender, TappedRoutedEventArgs e)
    {
        Extension extension = (sender as TextBlock).Tag as Extension;
        await Windows.System.Launcher.LaunchUriAsync(new Uri(extension.AboutPlugin.PluginAuthorURL));
    }

    private async void AddExtension_Click(object sender, RoutedEventArgs e)
    {
        var res = await FilePickerHelper.PickOpenFile(new string[] { ".dll" });
        if (!res.success)
            return;

        string path = res.path;

        var destinationPath = ApplicationData.Current.LocalFolder.Path + "\\extensions\\";
        if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
        File.Copy(path, destinationPath + Guid.NewGuid().ToString().Replace('-', '_').Replace(' ', '_') + ".dll");
        
        ExtensionHelper.Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(path), System.IO.Path.GetFileNameWithoutExtension(path)));
        extensionView.Items.Add(ExtensionHelper.Extensions[ExtensionHelper.Extensions.Count - 1]);
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

        if(downloadView.SelectedItem is FetchedExtension fetchedExtension)
        {
            var destinationPath = ApplicationData.Current.LocalFolder.Path + "\\extensions\\";
            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
            string p = destinationPath + Guid.NewGuid().ToString().Replace('-', '_').Replace(' ', '_') + ".dll";

            var downloadInfoBar = InfoMessages.DownloadingPluginInfo();

            await Task.Run(async () =>
            {
                var res = await RequestsHelper.DownloadFileAsync(fetchedExtension.URL, p);
                if (!res) //error while downloading plugin to file.
                {
                    downloadInfoBar.IsOpen = false;
                    return;
                }

                ExtensionHelper.Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(p), System.IO.Path.GetFileNameWithoutExtension(p)));

                DispatcherQueue.TryEnqueue(() =>
                {
                    extensionView.Items.Add(ExtensionHelper.Extensions[ExtensionHelper.Extensions.Count - 1]);
                    downloadInfoBar.IsOpen = false;
                });
            });
        }
    }
}
