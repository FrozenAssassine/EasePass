using EasePass.Dialogs;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace EasePass.Views;

public sealed partial class ManageDatabasePage : Page
{
    public ManageDatabasePage()
    {
        this.InitializeComponent();


        databaseDisplay.ItemsSource = new List<string> { "Julius", "Ben", "Finn", "Sna8xs" };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.m_window.ShowBackArrow = true;
        LoadBackupsFromFile();

        base.OnNavigatedTo(e);
    }

    private async void LoadBackupsFromFile()
    {
        var backupFiles = await MainWindow.databaseBackupHelper.GetAllBackupFiles();
        databaseBackupDisplay.ItemsSource = backupFiles.Select(x => new DatabaseBackupFile(x));
    }


    private void Delete_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        if (databaseDisplay.SelectedItem == null)
            return;

        //delete here:
    }

    private void Export_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        if (databaseDisplay.SelectedItem == null)
            return;

        //export here:
    }

    private async void CreateDatabase_Click(object sender, RoutedEventArgs e)
    {
        await new CreateDatabaseDialog().ShowAsync();
    }

    private void ImportDatabase_Click(object sender, RoutedEventArgs e)
    {
        if (databaseDisplay.SelectedItem == null)
            return;

        //import here:
    }

    private void CopyDatabasePath_Click(object sender, RoutedEventArgs e)
    {
        if (databaseDisplay.SelectedItem == null)
            return;

        //copy the path of the selected database item
    }


    //Backup databases
    private void Export_BackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        //var rightClicked = (sender as MenuFlyoutItem).Tag as Database);
    }

    private void LoadBackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        //var rightClicked = (sender as MenuFlyoutItem).Tag as Database);
    }

    private void Delete_BackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        //var rightClicked = (sender as MenuFlyoutItem).Tag as Database);
    }
}
