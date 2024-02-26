using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Views;

public sealed partial class ManageDatabasePage : Page
{
    ObservableCollection<Database> databases;
    Database selectedDatabase;

    public ManageDatabasePage()
    {
        this.InitializeComponent();


        databases = new ObservableCollection<Database>(Database.GetAllUnloadedDatabases());
        for (int i = 0; i < databases.Count; i++)
            if (databases[i].Path == Database.LoadedInstance.Path)
                databases[i] = Database.LoadedInstance;

        databaseDisplay.ItemsSource = databases;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.m_window.ShowBackArrow = true;
        //LoadBackupsFromFile();
        LoadPrinter();
    }

    private Database GetDatabase(object sender)
    {
        //either use the selected DB or the rightlicked one
        var db = selectedDatabase;
        if (sender is MenuFlyoutItem mf && mf.Tag is Database database)
            db = database;
        return db;

    }
    private void LoadPrinter()
    {
        printerSelector.Items.Clear();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printerSelector.Items.Add(printer);
        }
    }

    //DO NOT DELETE, Needs to be worked on in next update
    //private async void LoadBackupsFromFile()
    //{
    //    var backupFiles = await MainWindow.databaseBackupHelper.GetAllBackupFiles();
    //    databaseBackupDisplay.ItemsSource = backupFiles.Select(x => new Database(x));
    //}
    private async Task DeleteDatabase(Database db)
    {

        if (!await new ConfirmDeleteDatabaseDialog().ShowAsync(db))
            return;

        if (Database.GetAllDatabasePaths().Length == 1)
        {
            InfoMessages.CantDeleteDatabase();
            return;
        }

        if (Database.LoadedInstance == db)
        {
            InfoMessages.CantDeleteLoadedDatabase();
            return;
        }

        File.Delete(db.Path);
        Database.RemoveDatabasePath(db.Path);
        databases.Remove(db);
    }
    
    private async void Delete_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        var db = GetDatabase(sender);
        if (db == null)
            return;

        await DeleteDatabase(db);
    }

    private async void Export_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        var db = GetDatabase(sender);
        if (db == null)
            return;

        var res = await FilePickerHelper.PickSaveFile(("Ease Pass database", new List<string>() { ".epdb" }));
        if (!res.success)
            return;

        Database export = new Database(res.path);
        export.MasterPassword = db.MasterPassword;
        export.SetNew(await new SelectExportPasswordsDialog().ShowAsync(Database.LoadedInstance.Items));
        export.Save();

        InfoMessages.ExportDBSuccess();
    }

    private void Load_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        if (selectedDatabase == null)
            return;

        Database.LoadedInstance = selectedDatabase;
        InfoMessages.DatabaseLoaded();
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

    private async void ImportDatabase_Click(object sender, RoutedEventArgs e)
    {
        var res = await ManageDatabaseHelper.ImportDatabase();
        if (res == null)
            return;

        databases.Add(res);
    }

    private async void ImportIntoDatabase_Click(object sender, RoutedEventArgs e)
    {
        await ManageDatabaseHelper.ImportIntoDatabase();
    }

    private void CopyDatabasePath_Click(object sender, RoutedEventArgs e)
    {
        var db = GetDatabase(sender);
        if (db == null)
            return;

        ClipboardHelper.Copy(db.Path);
    }


    private async void CreateDatabase_Click(object sender, RoutedEventArgs e)
    {
        var newDB = await ManageDatabaseHelper.CreateDatabase();
        if (newDB == null)
            return;

        databases.Add(newDB);
    }

    private async void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        if (selectedDatabase == null)
            return;

        await new ChangePasswordDialog().ShowAsync(selectedDatabase);
    }

    private async void Export_BackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        var rightClicked = (sender as MenuFlyoutItem).Tag as Database;
        if (rightClicked == null)
            return;

        var file = await FilePickerHelper.PickSaveFile(("Ease Pass Database", new List<string> { ".epdb" }));

        try
        {
            File.Copy(rightClicked.Path, file.path);
        }
        catch(Exception ex)
        {
            InfoMessages.UnhandledException(ex);
        }
    }

    private async void LoadBackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        var rightClicked = (sender as MenuFlyoutItem).Tag as Database;
        if (rightClicked == null)
            return;

        var pwDialog = await new EnterPasswordDialog().ShowAsync();
        if (rightClicked.Load(pwDialog.Password))
        {
            Database.LoadedInstance = rightClicked;
            InfoMessages.DatabaseLoaded();
        }
    }

    private async void Delete_BackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        var rightClicked = (sender as MenuFlyoutItem).Tag as Database;
        if (rightClicked == null)
            return;

        await DeleteDatabase(rightClicked);
    }

    private async void databaseDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (databaseDisplay.SelectedItem == null)
        {
            selectedDatabase = null;
            editDatabaseView.Visibility = Visibility.Collapsed;
            return;
        }
        
        //when the database is not loaded, it is required to enter the proper password
        var db = databaseDisplay.SelectedItem as Database;
        if (db.MasterPassword == null)
        {
            var password = (await new EnterPasswordDialog().ShowAsync()).Password;
            if (password == null)
            {
                databaseDisplay.SelectedItem = null;
                return;
            }
            
            var res = db.Load(password, true);
            if(res == false)
            {
                databaseDisplay.SelectedItem = null;
                return;
            }
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
