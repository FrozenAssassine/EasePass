using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;

namespace EasePass.Views;

public sealed partial class ManageDatabasePage : Page
{
    ObservableCollection<Database> databases;

    public ManageDatabasePage()
    {
        this.InitializeComponent();

        App.m_window.ShowBackArrow = true;

        databases = new ObservableCollection<Database>(Database.GetAllUnloadedDatabases());

        databaseDisplay.ItemsSource = databases;

        LoadBackupsFromFile();
    }

    private async void LoadBackupsFromFile()
    {
        var backupFiles = await MainWindow.databaseBackupHelper.GetAllBackupFiles();
        databaseBackupDisplay.ItemsSource = backupFiles.Select(x => new Database(x));
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
        Database newDB = await new CreateDatabaseDialog().ShowAsync();
        if (newDB == null)
            return;
        Database.AddDatabasePath(newDB.Path);
        databases.Add(newDB);
    }

    private async void AddExisting_Click(object sender, RoutedEventArgs e)
    {
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { "*.epdb", "*.eped" });
        if (!pickerResult.success)
            return;

        Database db;
        string password = await new EnterPasswordDialog().ShowAsync();
        try
        {
            db = new Database(pickerResult.path, password.ConvertToSecureString());
        }
        catch
        {
            InfoMessages.ImportDBWrongPassword();
            return;
        }

        Database.AddDatabasePath(db.Path); // Do not change "db.Path" to "pickerResult.path" for example. "Database.Load()" is called in the constructor and could modify the path maybe.
        databases.Add(db);
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
