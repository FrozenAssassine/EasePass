using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Views.DialogPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;

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
        LoadBackupsFromFile();
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

    private async void LoadBackupsFromFile()
    {
        var backupFiles = await MainWindow.databaseBackupHelper.GetAllBackupFiles();
        databaseBackupDisplay.ItemsSource = backupFiles.Select(x => new Database(x));
    }

    private async void Delete_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        var db = GetDatabase(sender);
        if (db == null)
            return;

        if(!await new ConfirmDeleteDatabaseDialog().ShowAsync(db))
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
        db = null;
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
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
        if (!pickerResult.success)
            return;

        Database db;
        var password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
        {
            InfoMessages.ImportDBWrongPassword();
            return;
        }

        db = new Database(pickerResult.path);
        if (db.ValidatePwAndLoadDB(password) != PasswordValidationResult.Success)
            return;

        Database.AddDatabasePath(db.Path);
        databases.Add(db);
    }

    private async void ImportIntoDatabase_Click(object sender, RoutedEventArgs e)
    {
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
        if (!pickerResult.success)
            return;

        var password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
        {
            InfoMessages.ImportDBWrongPassword();
            return;
        }

        var importedItems = Database.GetItemsFromDatabase(pickerResult.path, password);

        var dialog = new ImportPasswordsDialog();
        dialog.SetPagePasswords(importedItems);

        var importItemsResult = await dialog.ShowAsync(false);
        if (importItemsResult.Items == null)
            return;

        if (importItemsResult.Override)
        {
            Database.LoadedInstance.Items.Clear();
        }

        Database.LoadedInstance.AddRange(importItemsResult.Items);
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
        Database newDB = await new CreateDatabaseDialog().ShowAsync();
        if (newDB == null)
            return;
        Database.AddDatabasePath(newDB.Path);
        databases.Add(newDB);
    }

    private async void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        if (selectedDatabase == null)
            return;

        await new ChangePasswordDialog().ShowAsync(selectedDatabase);
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
