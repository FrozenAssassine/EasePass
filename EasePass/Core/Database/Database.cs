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
using EasePass.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace EasePass.Core.Database;

public class Database
{
    private static DatabaseItem loadedInstance;
    public static DatabaseItem LoadedInstance
    {
        get => loadedInstance;
        set
        {
            loadedInstance = value;
            if (loadedInstance != null)
                AppSettings.LoadedDatabaseName = loadedInstance.Name;

            loadedInstance.LoadedInstanceChanged();
        }
    }

    public static DatabaseItem CreateNewDatabase(string path, SecureString password)
    {
        DatabaseItem db = new DatabaseItem(path);
        db.MasterPassword = password;
        db.Save();
        return db;
    }
    public static bool HasDatabasePath()
    {
        var dbPath = AppSettings.DatabasePaths;
        var loadedDb = AppSettings.LoadedDatabaseName;

        //app first start
        if (dbPath.Length == 0 && loadedDb == null)
            return false;

        if (dbPath.Length > 0 && loadedDb.Length > 0)
            return true;

        //fallback -> old version never saved the database to the settings, 
        //when there was only the default database loaded.
        if (dbPath.Length == 0 && loadedDb.Length > 0)
        {
            if (File.Exists(DefaultSettingsValues.databasePaths))
            {
                AppSettings.DatabasePaths = DefaultSettingsValues.databasePaths;
                return true;
            }
        }
        return false;
    }
    public static string[] GetAllDatabasePaths()
    {
        string paths = AppSettings.DatabasePaths;
        ReadOnlySpan<char> chars = paths.AsSpan();

        int length = chars.Count('|') + 1;
        Span<Range> ranges = length < 1024 ? stackalloc Range[length] : new Range[length];

        int splittedPaths = chars.Split(ranges, '|');

        List<string> res = new List<string>();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < splittedPaths; i++)
        {
            string path = chars[ranges[i]].ToString();
            if (File.Exists(path))
            {
                sb.Append(path);
                sb.Append('|');
                res.Add(path);
            }
        }
        SetAllDatabasePaths(sb.ToString());
        return res.ToArray();
    }
    public static void SetAllDatabasePaths(string paths)
    {
        AppSettings.DatabasePaths = paths;
    }
    public static void AddDatabasePath(string path)
    {
        List<string> paths = [.. GetAllDatabasePaths(), path];
        SetAllDatabasePaths(string.Join('|', paths));
    }
    public static void RemoveDatabasePath(string path)
    {
        string[] paths = GetAllDatabasePaths();
        if (paths == default || paths.Length == 0)
        {
            SetAllDatabasePaths(string.Empty);
            return;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < paths.Length; i++)
        {
            if (!paths[i].Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                sb.Append(paths[i]);
                sb.Append('|');
            }
        }

        if (sb.Length <= 0)
        {
            SetAllDatabasePaths(string.Empty);
            return;
        }
        sb.Remove(sb.Length - 1, 1);
        SetAllDatabasePaths(sb.ToString());
    }
    public static DatabaseItem[] GetAllUnloadedDatabases()
    {
        return GetAllDatabasePaths().Select(x => new DatabaseItem(x)).ToArray();
    }
    public static ObservableCollection<PasswordManagerItem> LoadItems(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<ObservableCollection<PasswordManagerItem>>(json);
        }
        catch
        {
            InfoMessages.DatabaseInvalidData();
            return null;
        }
    }
    public static ObservableCollection<PasswordManagerItem> GetItemsFromFile(string path, SecureString password)
    {
        var decryptedString = OldDatabaseImporter.GetDecryptedString(path, password);
        if (decryptedString != null)
        {
            return LoadItems(decryptedString);
        }

        var readFileResult = ReadFile(path, password);
        if (!readFileResult.success)
        {
            return null;
        }

        return LoadItems(readFileResult.data);
    }
    public static (string data, bool success) ReadFile(string path, SecureString pw, bool showWrongPasswordError = true)
    {
        byte[] fileData = null;

        try
        {
            fileData = File.ReadAllBytes(path);
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
        {
            InfoMessages.DatabaseFileNotFoundAt(path);
        }
        catch (UnauthorizedAccessException)
        {
            InfoMessages.NoAccessToPathDatabaseNotLoaded(path);
        }

        if (fileData != null)
        {
            var res = EncryptDecryptHelper.DecryptStringAES(fileData, pw);
            if (res.correctPassword)
                return (res.decryptedString, true);

            if (showWrongPasswordError)
                InfoMessages.ImportDBWrongPassword();
        }

        return ("", false);
    }
    public static void WriteFile(string path, string jsonString, SecureString pw)
    {
        if (pw == null)
            return;

        var bytes = EncryptDecryptHelper.EncryptStringAES(jsonString, pw);
        try
        {
            File.WriteAllBytes(path, bytes);
        }
        catch (UnauthorizedAccessException)
        {
            InfoMessages.NoAccessToPathDatabaseNotSaved(path);
        }
        catch (IOException)
        {
            InfoMessages.DatabaseSaveToFileError(path);
        }
    }
    public static string CreateJsonstring(ObservableCollection<PasswordManagerItem> pwItems)
    {
        if (pwItems == null)
            return "";

        return JsonConvert.SerializeObject(pwItems, Formatting.Indented);
    }
}