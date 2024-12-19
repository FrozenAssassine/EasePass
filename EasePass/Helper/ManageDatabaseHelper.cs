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
using EasePass.Models;
using System.Threading.Tasks;

namespace EasePass.Helper;

internal class ManageDatabaseHelper
{
    public static async Task<Database> ImportDatabase()
    {
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
        if (!pickerResult.success)
            return null;

        Database db;
        var password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
            return null; //cancelled by user

        db = new Database(pickerResult.path);
        if (db.ValidatePwAndLoadDB(password) != PasswordValidationResult.Success)
            return null;

        Database.AddDatabasePath(db.Path);
        Database.LoadedInstance.Save();
        return db;
    }

    public static async Task<bool> ImportIntoDatabase(string filePath = "")
    {
        if (filePath.Length == 0)
        {
            var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
            if (!pickerResult.success)
                return false;

            filePath = pickerResult.path;
        }

        var password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
            return false; //cancelled by user

        var importedItems = Database.GetItemsFromDatabase(filePath, password);

        var dialog = new ImportPasswordsDialog();
        dialog.SetPagePasswords(importedItems);

        var importItemsResult = await dialog.ShowAsync(false);
        if (importItemsResult.Items == null)
            return false;

        if (importItemsResult.Override)
        {
            Database.LoadedInstance.Items.Clear();
        }

        Database.LoadedInstance.AddRange(importItemsResult.Items);
        Database.LoadedInstance.Save();
        return true;
    }

    public static async Task<Database> CreateDatabase()
    {
        Database newDB = await new CreateDatabaseDialog().ShowAsync();
        if (newDB == null)
            return null;

        Database.AddDatabasePath(newDB.Path);
        InteropHelper.SetForegroundWindow(InteropHelper.GetWindowHandle(App.m_window));
        return newDB;
    }
}
