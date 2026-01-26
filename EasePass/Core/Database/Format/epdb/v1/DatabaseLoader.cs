using EasePass.Core.Database.Format.Serialization;
using EasePass.Helper.Database;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Core.Database.Format.epdb.v1
{
    [Obsolete("Just used for backward compatibility.")]
    internal class DatabaseLoader : IDatabaseLoader
    {
        #region Property
        public static double Version => 1.0;
        public static DatabaseVersionTag DBVersionTag => DatabaseVersionTag.EpdbV1DbVersion;
        #endregion

        #region Load
        public static async Task<DatabaseValidationResult> Load(IDatabaseSource source, SecureString password, bool showWrongPasswordError, byte[] preloaded = null)
        {
            if (source.Availability == IDatabaseSource.DatabaseAvailability.LockedByOtherUser)
                return new(PasswordValidationResult.LockedByOtherUser, default);

            byte[] content = preloaded != null ? preloaded : await source.GetDatabaseFileBytes();
            if (content == null || content.Length == 0)
                return new(PasswordValidationResult.DatabaseNotFound, default);
            if (!IDatabaseLoader.DecryptData(content, password, showWrongPasswordError, out string data))
                return new(PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return new(PasswordValidationResult.WrongFormat, default);

            DatabaseSettings settings = new DatabaseSettings();
            settings.SecondFactorType = Enums.SecondFactorType.None;
            settings.UseSecondFactor = false;

            DatabaseFile database = new DatabaseFile();
            database.DatabaseFileType = Enums.DatabaseFileType.epdb;
            database.Version = Version;
            database.Settings = settings;
            database.Items = items;

            return new(PasswordValidationResult.Success, database);
        }

        public static async Task<DatabaseValidationResult> LoadInternal(SecureString password, DatabaseFile database, bool showWrongPasswordError)
        {
            return new(PasswordValidationResult.Success, database);
        }
        #endregion
    }
}