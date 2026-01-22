using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper.FileSystem;
using EasePass.Helper.Security;
using EasePass.Models;
using EasePassExtensibility;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Core.Database.Format.epeb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        public static double Version => 1.0;

        public static async Task<(PasswordValidationResult result, DatabaseFile database)> Load(IDatabaseSource source, SecureString password, bool showWrongPasswordError)
        {
            if (source is not NativeDatabaseSource nds)
                return (PasswordValidationResult.WrongFormat, default);
            if (!FileHelper.HasExtension(nds.Path, ".epeb") || !File.Exists(nds.Path))
                return (PasswordValidationResult.DatabaseNotFound, default);

            string pw = new System.Net.NetworkCredential(string.Empty, password).Password;
            EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(nds.Path));
            if (!AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, pw))
            {
                if (showWrongPasswordError)
                {
                    InfoMessages.ImportDBWrongPassword();
                }
                return (PasswordValidationResult.WrongPassword, default);
            }

            if (!IDatabaseLoader.DecryptData(decryptedJson.Data, password, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return (PasswordValidationResult.WrongFormat, default);

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

        public static async Task<(PasswordValidationResult result, DatabaseFile database)> LoadInternal(SecureString password, DatabaseFile database, bool showWrongPasswordError)
        {
            return (PasswordValidationResult.Success, database);
        }
    }
}
