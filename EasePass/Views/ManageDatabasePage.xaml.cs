using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePassExtensibility;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security;

namespace EasePass.Views;

public sealed partial class ManageDatabasePage : Page
{
    ObservableCollection<Database> databases;
    Database selectedDatabase;

    public ManageDatabasePage()
    {
        this.InitializeComponent();

        App.m_window.ShowBackArrow = true;

        databases = new ObservableCollection<Database>(Database.GetAllUnloadedDatabases());

        databaseDisplay.ItemsSource = databases;

        LoadBackupsFromFile();
        LoadPrinter();
    }

    private void LoadPrinter()
    {
        printerSelector.Items.Clear();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printerSelector.Items.Add(printer);
        }
    }

    private async void LoadBackupsFromFile()
    {
        var backupFiles = await MainWindow.databaseBackupHelper.GetAllBackupFiles();
        databaseBackupDisplay.ItemsSource = backupFiles.Select(x => new Database(x));
    }


    private void Delete_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        if (selectedDatabase == null)
            return;

        //delete here:
    }

    private void Export_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        if (selectedDatabase == null)
            return;

        //export here:
        
    }
    private void Print_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        if (selectedDatabase == null)
            return;

        if (string.IsNullOrEmpty(PrinterHelper.SelectedPrinter))
        {
            InfoMessages.PrinterNotSelected();
            return;
        }
        PrinterHelper.Print(selectedDatabase.Items);
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
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
        if (!pickerResult.success)
            return;

        Database db;
        var password = await new EnterPasswordDialog().ShowAsync();
        if (password == null)
        {
            InfoMessages.ImportDBWrongPassword();
            return;
        }

        try
        {
            db = new Database(pickerResult.path, password);
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

    private async void databaseDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (databaseDisplay.SelectedItem == null)
        {
            selectedDatabase = null;
            editDatabaseView.Visibility = Visibility.Collapsed;
            return;
        }

        var db = databaseDisplay.SelectedItem as Database;
        if (db.Path != PasswordsPage.LoadedDatabase.Path) //check whether the database is already loaded so no need to unlock it.
        {
            var pw = await new EnterPasswordDialog().ShowAsync();
            if (pw == null)
                return;

            db.Load(pw);
        }

        loadedDBName.Text = db.Name;
        editDatabaseView.Visibility = Visibility.Visible;
        selectedDatabase = db;
    }

    private void printerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PrinterHelper.SelectedPrinter = (string)e.AddedItems[0];
    }
}
