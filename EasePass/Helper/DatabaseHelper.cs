using EasePass.Models;
using EasePass.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class DatabaseHelper
    {
        private static string DatabaseFilePath = @"C:\Users\Juliu\Desktop\PW.json";

        private static string CreateJsonstring(ObservableCollection<PasswordManagerItem> pwItems)
        {
            return JsonConvert.SerializeObject(pwItems, Formatting.Indented);
        }
        private static ObservableCollection<PasswordManagerItem> LoadItems(string json)
        {
            File.WriteAllText(@"C:\Users\juliu\Desktop\test.json", json);
            return ((JArray)JsonConvert.DeserializeObject(json)).ToObject<ObservableCollection<PasswordManagerItem>>();
        }


        private static string ReadFile(SecureString pw)
        {
            try
            {
                var bytes = File.ReadAllBytes(DatabaseFilePath);
                return EncryptDecryptHelper.DecryptStringAES(bytes, pw);
            }
            catch(FileNotFoundException ex)
            {
                return "";
            }
        }
        private static void WriteFile(string jsonString, SecureString pw)
        {
            var bytes = EncryptDecryptHelper.EncryptStringAES(jsonString, pw);
            File.WriteAllBytes(DatabaseFilePath, bytes);
        }

        public static void SaveDatabase(ObservableCollection<PasswordManagerItem> pwItems, SecureString pw)
        {
            var data = CreateJsonstring(pwItems);
            WriteFile(data, pw);
        }
        public static ObservableCollection<PasswordManagerItem> LoadDatabase(SecureString pw)
        {
            string data = ReadFile(pw);
            if (data == "")
                return null;

            return LoadItems(data);
        }
    }
}
