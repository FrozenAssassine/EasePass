using EasePass.Core.Database.Format.Serialization;
using EasePass.Models;
using EasePassExtensibility;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Core.Database.Format.epdb.v1
{
    internal class DatabaseLoader : IDatabaseLoader
    {
        #region Property
        public static double Version => 1.0;
        #endregion

        #region Load
        public static async Task<(PasswordValidationResult result, DatabaseFile database)> Load(IDatabaseSource source, SecureString password, bool showWrongPasswordError, byte[] preloaded = null)
        {
            if (source.GetAvailability() == IDatabaseSource.DatabaseAvailability.LockedByOtherUser)
                return (PasswordValidationResult.LockedByOtherUser, default);

            byte[] content = preloaded != null ? preloaded : source.GetDatabaseFileBytes();
            if (content == null || content.Length == 0)
                return (PasswordValidationResult.DatabaseNotFound, default);
            if (!IDatabaseLoader.DecryptData(content, password, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return (PasswordValidationResult.WrongFormat, default);

            DatabaseSettings settings = new DatabaseSettings();
            settings.SecondFactorType = Enums.SecondFactorType.None;
            settings.UseSecondFactor = false;

            DatabaseFile database = new DatabaseFile();
            database.DatabaseFileType = Enums.DatabaseFileType.epdb;
            database.Version = Version;
            database.Settings = settings;
            database.Items = items;

            return (PasswordValidationResult.Success, database);
        }

        public static async Task<(PasswordValidationResult result, DatabaseFile database)> LoadInternal(SecureString password, DatabaseFile database, bool showWrongPasswordError)
        {
            return (PasswordValidationResult.Success, database);
        }
        #endregion
    }
}