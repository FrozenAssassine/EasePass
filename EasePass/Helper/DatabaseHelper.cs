using EasePass.Core;
using EasePass.Dialogs;
using EasePass.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using Windows.Storage;

namespace EasePass.Helper
{
    internal class DatabaseHelper
    {
        private static string DatabaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "easepass.epdb");

        public static string CreateJsonstring(ObservableCollection<PasswordManagerItem> pwItems)
        {
            if (pwItems == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(pwItems, Formatting.Indented);
        }
        public static ObservableCollection<PasswordManagerItem> LoadItems(string json)
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

        private static string ReadFile(string path, SecureString pw, bool checkOldFileExtension)
        {
            try
            {
                byte[] fileData;

                //Alternative open old .db file:
                var oldPath = Path.ChangeExtension(path, ".db");
                if (checkOldFileExtension && File.Exists(oldPath) && !File.Exists(path)) 
                {
                    fileData = File.ReadAllBytes(oldPath);
                    File.Delete(oldPath);
                    File.WriteAllBytes(path, fileData);
                }
                else
                    fileData = File.ReadAllBytes(path);

                return EncryptDecryptHelper.DecryptStringAES(fileData, pw);
            }
            catch(FileNotFoundException)
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

        public static void SaveDatabase(PasswordItemsManager pwItems, SecureString pw, string path = "")
        {
            var data = CreateJsonstring(pwItems.PasswordItems);
            WriteFile(path: path.Length == 0 ? DatabaseFilePath : path, jsonString: data, pw);
        }
        public static ObservableCollection<PasswordManagerItem> LoadDatabase(SecureString pw, string path = "")
        {
            string data = ReadFile(path.Length == 0 ? DatabaseFilePath : path , pw, path.Length == 0);
            if (data.Length == 0)
                return null;

            return LoadItems(data);
        }
        
        //Used to create a file for the database on the very first start, so no error will be send, that the database was not found:
        public static void CreateInitialDatabaseFile(SecureString pw)
        {
            SaveDatabase(new PasswordItemsManager(new ObservableCollection<PasswordManagerItem>()), pw);
        }
    }
}
