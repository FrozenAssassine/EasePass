using EasePass.Core.Database.Format.Serialization;
using EasePass.Extensions;
using EasePass.Models;
using EasePass.Settings;
using EasePassExtensibility;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace EasePass.Tests.Core.DatabaseMigrationTests;

internal class MigrateV1ToV2_Helper
{
    public static V2DatabaseItem CreateV2DB(SecureString str)
    {
        string newPath = DatabaseTestHelper.GetTempDatabasePath();
        V2DatabaseItem dbItem = new V2DatabaseItem(newPath);
        dbItem.MasterPassword = str;
        return dbItem;
    }

    public static byte[] EncryptStringAES(string plainText, SecureString password, string salt = "")
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetCryptionKey(password, salt);
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                {
                    swEncrypt.Write(plainText);
                }

                return msEncrypt.ToArray();
            }
        }
    }

    public static (string decryptedString, bool correctPassword) DecryptStringAES(byte[] cipherText, SecureString password, string salt = "")
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetCryptionKey(password, salt);
            aesAlg.IV = cipherText.Take(16).ToArray();

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(cipherText.Skip(16).ToArray()))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
            {
                try
                {
                    var reader = srDecrypt.ReadToEnd();
                    return (reader, true);
                }
                catch (CryptographicException)
                {
                    return (null, false);
                }
            }
        }
    }

    private static byte[] ToBytes(SecureString secureString)
    {
        var pUnicodeBytes = Marshal.SecureStringToGlobalAllocUnicode(secureString);
        try
        {
            byte[] unicodeBytes = new byte[secureString.Length * 2];
            byte[] bytes = new byte[unicodeBytes.Length];

            for (var idx = 0; idx < unicodeBytes.Length; ++idx)
            {
                bytes[idx] = Marshal.ReadByte(pUnicodeBytes, idx);
            }

            return bytes;
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(pUnicodeBytes);
        }
    }

    public static byte[] DeriveEncryptionKey(SecureString password, byte[] salt, int keySizeInBytes, int iterations)
    {
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(ToBytes(password), salt, iterations))
        {
            return pbkdf2.GetBytes(keySizeInBytes);
        }
    }

    private static byte[] GetCryptionKey(SecureString pw, string salt = "")
    {
        byte[] saltFromDatabase = Encoding.UTF8.GetBytes(salt);
        int keySizeInBytes = 32;
        int iterations = 10000;

        return DeriveEncryptionKey(pw, saltFromDatabase, keySizeInBytes, iterations);
    }
}

public class V1PasswordManagerItem
{
    private string _Password;
    public string Password { get => _Password; set { _Password = value; } }
    private string _Username;
    public string Username { get => _Username; set { _Username = value; } }

    private string _Email;
    public string Email { get => _Email; set { _Email = value; } }

    private string _Notes;
    public string Notes { get => _Notes; set { _Notes = value; } }

    private string _Secret;
    public string Secret { get => _Secret; set { _Secret = value; } }
    public string Digits { get; set; } = "6";
    public string Interval { get; set; } = "30";
    public string Algorithm { get; set; } = "SHA1";
    public List<string> Clicks { get; } = new List<string>();
    [JsonIgnore]
    private string _DisplayName;
    public string DisplayName
    {
        get => _DisplayName;
        set
        {
            _DisplayName = value;
            FirstChar = value?.Length == 0 ? "" : value.Substring(0, 1);
        }
    }
    [JsonIgnore]
    private string _Website = "";
    public string Website
    {
        get => _Website;
        set
        {
            const string iconCache = "icons";
            _Website = value == null ? null : value.Trim();
            if (!ShowIcon)
            {
                Icon = null;
                return;
            }

            if (string.IsNullOrEmpty(_Website))
            {
                Icon = null;
                NotifyPropertyChanged("Icon");
                return;
            }

            if (!_Website.ToLower().StartsWith("http"))
                _Website = "http://" + _Website;

            try
            {
                CacheItem item = CacheItem.FindInCache(iconCache, _Website);
                if (item != null)
                {
                    Icon = new BitmapImage(new Uri(item.GetPath()));
                    Icon.ImageFailed += (object sender, ExceptionRoutedEventArgs e) => { Icon = null; NotifyPropertyChanged("Icon"); };

                    NotifyPropertyChanged("Icon");
                    NotifyPropertyChanged("Website");
                    return;
                }

                IconDownloadImage(item, iconCache);
            }
            catch { /*Invalid URI format*/ }

            NotifyPropertyChanged("Icon");
            NotifyPropertyChanged("Website");
        }
    }

    [JsonIgnore]
    public BitmapImage Icon = null;
    [JsonIgnore]
    public Brush BackColor
    {
        get
        {
            MD5 md5Hasher = MD5.Create();
            byte[] bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(DisplayName));
            return new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));
        }
    }
    [JsonIgnore]
    public Brush ForeColor
    {
        get
        {
            SolidColorBrush c = BackColor as SolidColorBrush;
            byte average = (byte)((c.Color.R + c.Color.G + c.Color.B) / 3);
            return average > 127 ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)) : new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }
    }
    [JsonIgnore]
    public string FirstChar = "";
    [JsonIgnore]
    public bool ShowIcon => AppSettings.ShowIcons;

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void IconDownloadImage(CacheItem item, string iconCache)
    {
        item = CacheItem.Create(iconCache, _Website);
        if (item == null)
        {
            Icon = null;

            return;
        }

        //if (await RequestsHelper.DownloadFileAsync(_Website + "/favicon.ico", item.GetPath(), 30000))
        //{
        //    Icon = new BitmapImage(new Uri(item.GetPath()));
        //    Icon.ImageFailed += (object sender, ExceptionRoutedEventArgs e) => { Icon = null; NotifyPropertyChanged("Icon"); };
        //}
        //else
        //    Icon = null;

        Icon = null;

        //check for valid item
        if (item.GetCacheSize() < 10)
        {
            item.Remove();
        }
    }
}


public class V2DatabaseItem
{
    public string Name => MakeDatabaseName();
    public string Path = "";
    public SecureString MasterPassword = null;
    public ObservableCollection<V1PasswordManagerItem> Items = null;
    public DateTime LastModified => File.GetLastWriteTime(Path);
    public string LastModifiedStr => Name; //LastModified.ToString("D");

    public bool IsTemporaryDatabase { get; set; } = false;

    public event PropertyChangedEventHandler PropertyChanged;

    public V2DatabaseItem(string path)
    {
        Path = path;
        Items = new ObservableCollection<V1PasswordManagerItem>();
    }

    private string MakeDatabaseName()
    {
        var name = System.IO.Path.GetFileNameWithoutExtension(Path);

        if (IsTemporaryDatabase)
            name += " (Temp)";

        return name;
    }

    private void CallPropertyChanged(string name)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public static ObservableCollection<V1PasswordManagerItem> LoadItems(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<ObservableCollection<V1PasswordManagerItem>>(json);
        }
        catch
        {
            return null;
        }
    }

    public bool Unlock(SecureString password, bool showWrongPasswordError = true)
    {
        var res = CheckPasswordCorrect(password, showWrongPasswordError);

        if (res.result == PasswordValidationResult.DatabaseNotFound)
        {
            return false;
        }

        if (res.result == PasswordValidationResult.WrongPassword)
            return false;

        MasterPassword = password;

        Items = LoadItems(res.decryptedData);
        ClearOldClicksCache();

        CallPropertyChanged("Items");
        CallPropertyChanged("MasterPassword");

        return true;
    }

    public static void WriteFile(string path, string jsonString, SecureString pw)
    {
        if (pw == null)
            return;

        var bytes = MigrateV1ToV2_Helper.EncryptStringAES(jsonString, pw);
        try
        {
            File.WriteAllBytes(path, bytes);
        }
        catch (UnauthorizedAccessException)
        {
            //No access to file path
        }
        catch (IOException)
        {
            //Save to file error
        }
    }

    public bool Load(SecureString password, bool showWrongPasswordError = true)
    {
        Unlock(password, showWrongPasswordError);
        return true;
    }

    public (PasswordValidationResult result, string decryptedData) CheckPasswordCorrect(SecureString enteredPassword, bool showWrongPasswordError = false)
    {
        if (enteredPassword == null)
            return (PasswordValidationResult.WrongPassword, null);

        if (!File.Exists(Path))
            return (PasswordValidationResult.DatabaseNotFound, null);

        var (data, success) = ReadFile(Path, enteredPassword, showWrongPasswordError);
        if (success)
            return (PasswordValidationResult.Success, data);

        return (PasswordValidationResult.WrongPassword, null);
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

        }
        catch (UnauthorizedAccessException)
        {
        }

        if (fileData != null)
        {
            var res = MigrateV1ToV2_Helper.DecryptStringAES(fileData, pw);
            if (res.correctPassword)
                return (res.decryptedString, true);
            return ("", false);
        }

        return ("", false);
    }

    public static string CreateJsonstring(ObservableCollection<V1PasswordManagerItem> pwItems)
    {
        if (pwItems == null)
            return "";

        return JsonConvert.SerializeObject(pwItems, Formatting.Indented);
    }

    public void Save(string path = null)
    {
        var data = CreateJsonstring(Items);
        WriteFile(path ?? Path, data, MasterPassword);
    }

    public void ClearOldClicksCache()
    {
        if (Items == null)
            return;

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

    public void Dispose()
    {
        Items.Clear();
        Items = null;
        MasterPassword = null;
        CallPropertyChanged("Items");
        CallPropertyChanged("MasterPassword");
    }

    public ObservableCollection<V1PasswordManagerItem> FindItemsByName(string name)
    {
        return new ObservableCollection<V1PasswordManagerItem>(Items.Where(x => x.DisplayName.Contains(name, StringComparison.OrdinalIgnoreCase)));
    }

    public void DeleteItem(V1PasswordManagerItem item)
    {
        Items.Remove(item);
        CallPropertyChanged("Items");
    }

    public void AddItem(V1PasswordManagerItem item)
    {
        Items.Add(item);
        CallPropertyChanged("Items");
    }

    public int PasswordAlreadyExists(string password)
    {
        int count = 0;
        int length = Items.Count;
        for (int i = 0; i < length; i++)
        {
            if (Items[i].Password == password)
            {
                count++;
            }
        }
        return count;
    }

    public void AddRange(V1PasswordManagerItem[] items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }

    public void AddRange(ObservableCollection<V1PasswordManagerItem> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }

    public void SetNewPasswords(ObservableCollection<V1PasswordManagerItem> items)
    {
        Items.Clear();

        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }
    public void SetNewPasswords(V1PasswordManagerItem[] items)
    {
        if (items == null)
            return;

        Items.Clear();

        foreach (var item in items)
        {
            Items.Add(item);
        }

        CallPropertyChanged("Items");
    }
}
