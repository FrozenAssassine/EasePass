using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper.Database;
using EasePass.Helper.Security;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Core.Database.Format.epdb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        #region Properties
        public static double Version => 1.4;
        public static DatabaseVersionTag DBVersionTag => DatabaseVersionTag.EpdbV2DbVersion;

        /// <summary>
        /// The Salt, which will be used for the Argon Hash algorithm
        /// </summary>
        private readonly static byte[] salt = Encoding.UTF8.GetBytes("EasePassArgonHash");

        /// <summary>
        /// The Associated Data, which will be used for the Argon Hash algorithm
        /// </summary>
        private readonly static byte[] associatedData = Encoding.UTF8.GetBytes("Database_Version_" + Version);
        #endregion

        #region Load
        public static async Task<DatabaseValidationResult> Load(IDatabaseSource source, SecureString password, bool showWrongPasswordError, byte[] preloaded = null)
        {
            if (source.GetAvailability() == IDatabaseSource.DatabaseAvailability.LockedByOtherUser)
                return new(PasswordValidationResult.LockedByOtherUser, default);

            byte[] pass = HashHelper.HashPasswordWithArgon2id(password, salt);

            byte[] content = preloaded != null ? preloaded : source.GetDatabaseFileBytes();
            if (content == null || content.Length == 0)
                return new(PasswordValidationResult.DatabaseNotFound, default);

            if (!IDatabaseLoader.DecryptData(content, pass, showWrongPasswordError, out string data))
                return new(PasswordValidationResult.WrongPassword, default);

            DatabaseFile database = DatabaseFile.Deserialize(data);

            if (database == default)
                return new(PasswordValidationResult.WrongFormat, default);

            if (database.Settings != null && database.Settings.UseSecondFactor)
            {
                EnterSecondFactorDialog secondFactorDialog = new EnterSecondFactorDialog();
                await secondFactorDialog.ShowAsync();

                database.SecondFactor = secondFactorDialog.Token;
                pass = HashHelper.HashPasswordWithArgon2id(secondFactorDialog.Token, salt, associatedData);
            }
            else
            {
                char[] base64 = password.ToBytes().ToBase64();
                Array.Reverse(base64);
                pass = HashHelper.HashPasswordWithArgon2id(base64, salt, associatedData);
                base64.ZeroOut();
            }

            if (!IDatabaseLoader.DecryptData(database.Data, pass, showWrongPasswordError, out data))
                return new(PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return new(PasswordValidationResult.WrongFormat, default);

            database.Items = items;
            database.Data = Array.Empty<byte>();

            return new(PasswordValidationResult.Success, database);
        }

        public static async Task<DatabaseValidationResult> LoadInternal(SecureString password, DatabaseFile database, bool showWrongPasswordError)
        {
            byte[] pass;

            if (database == default)
                return new(PasswordValidationResult.WrongFormat, default);

            if (database.Settings.UseSecondFactor)
            {
                EnterSecondFactorDialog secondFactorDialog = new EnterSecondFactorDialog();
                await secondFactorDialog.ShowAsync();

                database.SecondFactor = secondFactorDialog.Token;
                pass = HashHelper.HashPasswordWithArgon2id(secondFactorDialog.Token, salt, associatedData);
            }
            else
            {
                char[] base64 = password.ToBytes().ToBase64();
                Array.Reverse(base64);
                pass = HashHelper.HashPasswordWithArgon2id(base64, salt, associatedData);
            }

            if (!IDatabaseLoader.DecryptData(database.Data, pass, showWrongPasswordError, out string data))
                return new(PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return new(PasswordValidationResult.WrongFormat, default);

            database.Items = items;
            database.Data = Array.Empty<byte>();

            return default;
        }
        #endregion

        #region Save
        public static bool Save(IDatabaseSource source, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            byte[] data;
            string json = PasswordManagerItem.SerializeItems(items);
            byte[] pass;
            
            if (secondFactor == null)
            {
                char[] base64 = password.ToBytes().ToBase64();
                Array.Reverse(base64);
                pass = HashHelper.HashPasswordWithArgon2id(base64, salt, associatedData);
                data = EncryptDecryptHelper.EncryptStringAES(json, pass);
            }
            else
            {
                pass = HashHelper.HashPasswordWithArgon2id(secondFactor, salt, associatedData);
                data = EncryptDecryptHelper.EncryptStringAES(json, pass);
            }

            DatabaseFile database = new DatabaseFile();
            database.DatabaseFileType = Enums.DatabaseFileType.epdb;
            database.Version = Version;
            database.Settings = settings;
            database.Data = data;

            json = database.Serialize();

            pass = HashHelper.HashPasswordWithArgon2id(password, salt);
            data = EncryptDecryptHelper.EncryptStringAES(json, pass);

            return source.SaveDatabaseFileBytes(DatabaseVersionTagHelper.AddVersionTag(data, (int)DBVersionTag));
        }
        #endregion
    }
}