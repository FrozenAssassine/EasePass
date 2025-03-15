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
using EasePass.Helper.FileSystem;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Helper.Database;

internal class ManageDatabaseHelper
{
    private static readonly string[] extensions = new string[] { ".epdb", ".eped" };
    public static async Task<DatabaseItem> ImportDatabase()
    {
        var pickerResult = await FilePickerHelper.PickOpenFile(extensions);
        if (!pickerResult.success)
            return null;

        SecureString password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
            return null; //cancelled by user

        DatabaseItem db = new DatabaseItem(pickerResult.path);
        if (!await db.Unlock(password))
            return null;

        Core.Database.Database.AddDatabasePath(db.Path);
        return db;
    }

    public static async Task<bool> ImportIntoDatabase(string filePath = "")
    {
        if (filePath.Length == 0)
        {
            var pickerResult = await FilePickerHelper.PickOpenFile(extensions);
            if (!pickerResult.success)
                return false;

            filePath = pickerResult.path;
        }

        SecureString password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
            return false; //cancelled by user

        ObservableCollection<PasswordManagerItem> importedItems = await Core.Database.Database.GetItemsFromFile(filePath, password);

        ImportPasswordsDialog dialog = new ImportPasswordsDialog();
        dialog.SetPagePasswords(importedItems);

        var importItemsResult = await dialog.ShowAsync(false);
        if (importItemsResult.Items == null)
            return false;

        if (importItemsResult.Override)
        {
            Core.Database.Database.LoadedInstance.Items.Clear();
        }

        Core.Database.Database.LoadedInstance.AddRange(importItemsResult.Items);
        Core.Database.Database.LoadedInstance.Save();
        return true;
    }

    public static async Task<DatabaseItem> CreateDatabase()
    {
        DatabaseItem newDB = await new CreateDatabaseDialog().ShowAsync();
        if (newDB == null)
            return null;

        Core.Database.Database.AddDatabasePath(newDB.Path);
        InteropHelper.SetForegroundWindow(InteropHelper.GetWindowHandle(EasePass.App.m_window));
        return newDB;
    }

    public static void LoadDatabasesToCombobox(ComboBox databaseBox)
    {
        DatabaseItem[] databases = Core.Database.Database.GetAllUnloadedDatabases();
        string comboboxIndexDBName = AppSettings.LoadedDatabaseName ?? (databases.Length > 0 ? databases[0].Name : "");
        foreach (DatabaseItem item in databases)
        {
            databaseBox.Items.Add(item);
            if (comboboxIndexDBName.Equals(item.Name, System.StringComparison.OrdinalIgnoreCase))
            {
                databaseBox.SelectedItem = item;
            }
        }

        if (databaseBox.SelectedItem == null)
        {
            databaseBox.SelectedIndex = 0;
        }
    }
}