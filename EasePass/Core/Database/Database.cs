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

using EasePass.Core.Database.Format;
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
using System.Threading.Tasks;

namespace EasePass.Core.Database;

public class Database
{
    #region Properties
    private static DatabaseItem loadedInstance;
    public static DatabaseItem LoadedInstance
    {
        get => loadedInstance;
        set
        {
            loadedInstance = value;
            if (loadedInstance != null)
            {
                AppSettings.LoadedDatabaseName = loadedInstance.Name;
            }

            loadedInstance.LoadedInstanceChanged();
        }
    }
    #endregion

    #region AddDatabasePath
    public static void AddDatabasePath(string path)
    {
        List<string> paths = [.. GetAllDatabasePaths(), path];
        SetAllDatabasePaths(string.Join('|', paths));
    }
    #endregion

    #region CreateNewDatabase
    public static DatabaseItem CreateNewDatabase(string path, SecureString password)
    {
        DatabaseItem db = new DatabaseItem(path);
        db.MasterPassword = password;
        db.Save();
        return db;
    }
    #endregion

    #region GetAllDatabasePaths
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
    #endregion

    #region GetAllUnloadedDatabases
    public static DatabaseItem[] GetAllUnloadedDatabases()
    {
        return GetAllDatabasePaths().Select(x => new DatabaseItem(x)).ToArray();
    }
    #endregion

    #region GetItemsFromFile
    public async static Task<ObservableCollection<PasswordManagerItem>> GetItemsFromFile(string path, SecureString password)
    {
        var result = await DatabaseFormatHelper.Load(path, password, true);
        if (result.result != PasswordValidationResult.Success)
            return new ObservableCollection<PasswordManagerItem>();
        
        return result.database.Items;
    }
    #endregion

    #region HasDatabasePath
    public static bool HasDatabasePath()
    {
        string dbPath = AppSettings.DatabasePaths;
        string loadedDb = AppSettings.LoadedDatabaseName;

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
    #endregion

    #region LoadItems
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
    #endregion

    #region ReadFile
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
    #endregion

    #region RemoveDatabasePath
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
    #endregion

    #region SetAllDatabasePaths
    public static void SetAllDatabasePaths(string paths)
    {
        AppSettings.DatabasePaths = paths;
    }
    #endregion

    #region WriteFile
    public static void WriteFile(string path, string jsonString, SecureString pw)
    {
        if (pw == null)
            return;

        byte[] bytes = EncryptDecryptHelper.EncryptStringAES(jsonString, pw);
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
    #endregion
}