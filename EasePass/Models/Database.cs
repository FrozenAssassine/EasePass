using EasePass.Dialogs;
using EasePass.Helper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using EasePass.Settings;
using System.ComponentModel;
using System.Collections.Generic;

namespace EasePass.Models;

public class Database : IDisposable, INotifyPropertyChanged
{
    public string Name = "";
    public string Path = "";
    public SecureString MasterPassword = null;
    public ObservableCollection<PasswordManagerItem> Items = null;
    public DateTime LastModified => File.GetLastWriteTime(Path);
    public string LastModifiedStr => LastModified.ToString("D");

    public event PropertyChangedEventHandler PropertyChanged;

    public Database(string path, SecureString masterPassword = null)
    {
        this.Path = path;
        Items = new ObservableCollection<PasswordManagerItem>();
        Items.CollectionChanged += Items_CollectionChanged;
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
        if (masterPassword != null)
            Load(masterPassword);
        CallPropertyChanged("Name");
        CallPropertyChanged("Path");
        CallPropertyChanged("MasterPassword");
        CallPropertyChanged("Items");
    }

    private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        CallPropertyChanged("Items");
    }

    public static Database CreateEmptyDatabase(string path, SecureString password)
    {
        Database db = new Database(path);
        db.Save(password);
        return db;
    }

    public static string[] GetAllDatabasePaths()
    {
        string paths = AppSettings.GetSettings(AppSettingsValues.databasePaths, DefaultSettingsValues.databasePaths);
        return paths.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static void SetAllDatabasePaths(string[] paths)
    {
        string res = "";
        for(int i = 0; i < paths.Length; i++)
        {
            res += "|" + paths[i];
        }
        res = res.Substring(1);
        AppSettings.GetSettings(res, DefaultSettingsValues.databasePaths);
    }

    public static void AddDatabasePath(string path)
    {
        List<string> paths = new List<string>();
        paths.AddRange(GetAllDatabasePaths());
        paths.Add(path);
        SetAllDatabasePaths(paths.ToArray());
    }

    public static void RemoveDatabasePath(string path)
    {
        List<string> paths = new List<string>();
        paths.AddRange(GetAllDatabasePaths());
        for(int i = 0; i < paths.Count; i++)
        {
            if (paths[i].ToLower() == path.ToLower())
            {
                paths.RemoveAt(i);
                i--;
            }
        }
        SetAllDatabasePaths(paths.ToArray());
    }

    public static Database[] GetAllUnloadedDatabases()
    {
        return GetAllDatabasePaths().Select(x => new Database(x)).ToArray();
    }

    public void Load(SecureString password)
    {
        MasterPassword = password;
        CallPropertyChanged("MasterPassword");

        if (System.IO.Path.GetExtension(Path).ToLower() == ".eped")
        {
            string pw = new System.Net.NetworkCredential(string.Empty, password).Password;
            EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(Path));
            if (!AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, pw))
            {
                InfoMessages.ImportDBWrongPassword();
                return;
            }
            var str = EncryptDecryptHelper.DecryptStringAES(decryptedJson.Data, pw, decryptedJson.Salt);
            Items = LoadItems(str);
            ClearOldClicksCache();
            CallPropertyChanged("Items");

            Path = System.IO.Path.ChangeExtension(Path, "epdb");
            Save();
        }

        string data = ReadFile(Path, password);
        if (data.Length == 0)
        {
            Items = new ObservableCollection<PasswordManagerItem>();
            return;
        }

        Items = LoadItems(data);
        ClearOldClicksCache();
        CallPropertyChanged("Items");
    }

    public void Save(SecureString password = null)
    {
        var data = CreateJsonstring(Items);
        WriteFile(Path, data, password == null ? MasterPassword : password);
    }

    public void ClearOldClicksCache()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            for (int j = 0; j < Items[i].Clicks.Count; j++)
            {
                string[] splitted = Items[i].Clicks[j].Split('.');
                if (splitted.Length != 3) continue;
                DateTime date = new DateTime(Convert.ToInt32(splitted[2]), Convert.ToInt32(splitted[1]), Convert.ToInt32(splitted[0]));
                if (DateTime.Now - date > TimeSpan.FromDays(365))
                {
                    Items[i].Clicks.RemoveAt(j);
                    j--;
                }
            }
        }
        CallPropertyChanged("Items");
    }

    private static string CreateJsonstring(ObservableCollection<PasswordManagerItem> pwItems)
    {
        if (pwItems == null)
        {
            return "";
        }
        return JsonConvert.SerializeObject(pwItems, Formatting.Indented);
    }

    private static ObservableCollection<PasswordManagerItem> LoadItems(string json)
    {
        try
        {
            return ((JArray)JsonConvert.DeserializeObject(json)).ToObject<ObservableCollection<PasswordManagerItem>>();
        }
        catch
        {
            InfoMessages.DatabaseInvalidData();
            return null;
        }
    }

    public void Dispose()
    {
        Items = null;
        MasterPassword = null;
        CallPropertyChanged("Items");
        CallPropertyChanged("MasterPassword");
        //GC.Collect();
    }

    public ObservableCollection<PasswordManagerItem> FindItemsByName(string name)
    {
        return new ObservableCollection<PasswordManagerItem>(Items.Where(x => x.DisplayName.Contains(name, System.StringComparison.OrdinalIgnoreCase)));
    }

    public void DeleteItem(PasswordManagerItem item)
    {
        Items.Remove(item);
        CallPropertyChanged("Items");
    }

    public void AddItem(PasswordManagerItem item)
    {
        Items.Add(item);
        CallPropertyChanged("Items");
    }

    public int PasswordAlreadyExists(string password)
    {
        return Items.Count(x => x.Password == password);
    }

    public void AddRange(PasswordManagerItem[] items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }

    public void AddRange(ObservableCollection<PasswordManagerItem> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }

    public void SetNew(ObservableCollection<PasswordManagerItem> items)
    {
        Items.Clear();

        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }

    private static string ReadFile(string path, SecureString pw)
    {
        try
        {
            byte[] fileData;

            //Alternative open old .db file:
            var oldPath = System.IO.Path.ChangeExtension(path, ".db");
            if (File.Exists(oldPath) && !File.Exists(path))
            {
                fileData = File.ReadAllBytes(oldPath);
                File.Delete(oldPath);
                File.WriteAllBytes(path, fileData);
            }
            else
                fileData = File.ReadAllBytes(path);

            return EncryptDecryptHelper.DecryptStringAES(fileData, pw);
        }
        catch (FileNotFoundException)
        {
            InfoMessages.DatabaseFileNotFoundAt(path);
        }
        catch (UnauthorizedAccessException)
        {
            InfoMessages.NoAccessToPathDatabaseNotLoaded(path);
        }
        return "";
    }

    private static void WriteFile(string path, string jsonString, SecureString pw)
    {
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

    private void CallPropertyChanged(string name)
    {
        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
    }
}
