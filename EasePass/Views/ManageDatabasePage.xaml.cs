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
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Helper.Database;
using EasePass.Helper.FileSystem;
using EasePass.Helper.Security;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Views;

public sealed partial class ManageDatabasePage : Page
{
    ObservableCollection<DatabaseItem> databases;
    DatabaseItem selectedDatabase;

    public ManageDatabasePage()
    {
        this.InitializeComponent();

        databases = new ObservableCollection<DatabaseItem>(Database.GetAllUnloadedDatabases());
        for (int i = 0; i < databases.Count; i++)
        {
            if (databases[i].Path == Database.LoadedInstance.Path)
            {
                databases[i] = Database.LoadedInstance;
            }
        }
        databaseDisplay.ItemsSource = databases;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.m_window.ShowBackArrow = true;
        //LoadBackupsFromFile();
        LoadPrinter();
    }

    private DatabaseItem GetDatabaseItem(object sender)
    {
        //either use the selected DB or the rightlicked one
        DatabaseItem db = selectedDatabase;
        if (sender is MenuFlyoutItem mf && mf.Tag is DatabaseItem database)
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

    private async Task DeleteDatabase(DatabaseItem db)
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
        DatabaseItem db = GetDatabaseItem(sender);
        if (db == null)
            return;

        await DeleteDatabase(db);
    }

    private async void Export_DatabaseItem_Click(object sender, RoutedEventArgs e)
    {
        DatabaseItem db = GetDatabaseItem(sender);
        if (db == null)
            return;

        await ExportPasswordsHelper.Export(db, Database.LoadedInstance.Items);
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
        DatabaseItem res = await ManageDatabaseHelper.ImportDatabase();
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
        DatabaseItem db = GetDatabaseItem(sender);
        if (db == null)
            return;

        ClipboardHelper.Copy(db.Path);
    }

    private async void CreateDatabase_Click(object sender, RoutedEventArgs e)
    {
        DatabaseItem newDB = await ManageDatabaseHelper.CreateDatabase();
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
        var rightClicked = (sender as MenuFlyoutItem).Tag as DatabaseItem;
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
        DatabaseItem rightClicked = (sender as MenuFlyoutItem).Tag as DatabaseItem;
        if (rightClicked == null)
            return;

        EnterPasswordDialog pwDialog = await new EnterPasswordDialog().ShowAsync();
        if (await rightClicked.Load(pwDialog.Password))
        {
            Database.LoadedInstance = rightClicked;
            InfoMessages.DatabaseLoaded();
        }
    }

    private async void Delete_BackupDatabase_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as MenuFlyoutItem).Tag is not DatabaseItem rightClicked)
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
        DatabaseItem db = databaseDisplay.SelectedItem as DatabaseItem;
        if (db.MasterPassword == null)
        {
            SecureString password = (await new EnterPasswordDialog().ShowAsync()).Password;
            if (password == null)
            {
                databaseDisplay.SelectedItem = null;
                return;
            }
            
            var res = await db.Unlock(password, true);
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

    private async void Export_DatabaseItem_DiffPW_Click(object sender, RoutedEventArgs e)
    {
        EnterPasswordDialog dialog = await new EnterPasswordDialog().ShowAsync();
        if (dialog.Password == null)
            return;

        DatabaseItem db = GetDatabaseItem(sender);
        if (db == null)
            return;

        await ExportPasswordsHelper.Export(db, Database.LoadedInstance.Items, dialog.Password);
    }

    private async void ManageSecondFactor_Click(object sender, RoutedEventArgs e)
    {
        DatabaseItem database = databaseDisplay.SelectedItem as DatabaseItem;
        var sfPage = await new ManageSecondFactorDialog().ShowAsync(database.Name, database.Settings, database.SecondFactor);

        if (sfPage.Result)
        {
            if (!database.Settings.UseSecondFactor && sfPage.Settings.UseSecondFactor && !await new AddSecondFactorConfirmationDialog().ShowAsync(database.Name))
            {
                return;
            }
            database.Settings = sfPage.Settings;
            database.SecondFactor = sfPage.Token;
            await database.ForceSaveAsync();
        }
    }
}