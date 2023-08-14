using EasePass.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using Windows.Storage;

namespace EasePass.Helper
{
    internal class DatabaseHelper
    {
        private static string DatabaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "easepass.db");

        public static string CreateJsonstring(ObservableCollection<PasswordManagerItem> pwItems)
        {
            return JsonConvert.SerializeObject(pwItems, Formatting.Indented);
        }
        public static ObservableCollection<PasswordManagerItem> LoadItems(string json)
        {
            return ((JArray)JsonConvert.DeserializeObject(json)).ToObject<ObservableCollection<PasswordManagerItem>>();
        }

        private static string ReadFile(SecureString pw)
        {
            try
            {
                var bytes = File.ReadAllBytes(DatabaseFilePath);
                return EncryptDecryptHelper.DecryptStringAES(bytes, pw);
            }
            catch(FileNotFoundException)
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
