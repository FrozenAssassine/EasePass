using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Core.Database.Format.epeb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        public static double Version => 1.0;

        public static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            if (!FileHelper.HasExtension(path, ".epeb"))
                return (PasswordValidationResult.DatabaseNotFound, default);

            string pw = new System.Net.NetworkCredential(string.Empty, password).Password;
            EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(path));
            if (!AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, pw))
            {
                if (showWrongPasswordError)
                    InfoMessages.ImportDBWrongPassword();
                return (PasswordValidationResult.WrongPassword, null);
            }

            if (!IDatabaseLoader.DecryptData(decryptedJson.Data, password, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = IDatabaseLoader.DeserializePasswordManagerItems(data);
            DatabaseSettings settings = new DatabaseSettings();
            settings.SecondFactorType = Enums.SecondFactorType.None;
            settings.UseSecondFactor = false;

            DatabaseFile database = new DatabaseFile();
            database.DatabaseFileType = Enums.DatabaseFileType.epeb;
            database.Version = Version;
            database.Items = items;
            database.Settings = settings;
            
            return (PasswordValidationResult.Success, database);
        }
    }
}
