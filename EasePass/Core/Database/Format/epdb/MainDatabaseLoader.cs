using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasePass.Core.Database.Format.epdb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        #region Properties
        public static double Version => 1.1;

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
        public static async Task<(PasswordValidationResult result, DatabaseFile database)> Load(string path, SecureString password, bool showWrongPasswordError)
        {
            if (!File.Exists(path))
                return (PasswordValidationResult.DatabaseNotFound, default);

            byte[] pass = HashHelper.HashPasswordWithArgon2id(password, salt, associatedData);

            if (!IDatabaseLoader.DecryptData(IDatabaseLoader.ReadFile(path), pass, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            DatabaseFile database = DatabaseFile.Deserialize(data);

            // TODO:
            // - Need to check if SecondFactor is in use
            // - If SecondFactor is in use get the Token
            // - After receiving the Token check the SecondFactor Type
            // - Need to do that how the Decryption have to be done

            if (database == default)
                return (PasswordValidationResult.WrongFormat, default);

            if (database.Settings.UseSecondFactor)
            {
                EnterSecondFactorDialog secondFactorDialog = new EnterSecondFactorDialog();
                await secondFactorDialog.ShowAsync();

                database.SecondFactor = secondFactorDialog.Token;
                pass = HashHelper.HashPasswordWithArgon2id(secondFactorDialog.Token, salt, associatedData);
            }

            if (!IDatabaseLoader.DecryptData(database.Data, pass, showWrongPasswordError, out data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return (PasswordValidationResult.WrongFormat, default);

            database.Items = items;
            database.Data = Array.Empty<byte>();

            return (PasswordValidationResult.Success, database);
        }

        public static async Task<(PasswordValidationResult result, DatabaseFile database)> LoadInternal(SecureString password, DatabaseFile database, bool showWrongPasswordError)
        {
            byte[] pass;

            if (database == default)
                return (PasswordValidationResult.WrongFormat, default);

            if (database.Settings.UseSecondFactor)
            {
                EnterSecondFactorDialog secondFactorDialog = new EnterSecondFactorDialog();
                await secondFactorDialog.ShowAsync();

                database.SecondFactor = secondFactorDialog.Token;
                pass = HashHelper.HashPasswordWithArgon2id(secondFactorDialog.Token, salt, associatedData);
            }
            else
            {
                pass = HashHelper.HashPasswordWithArgon2id(password, salt, associatedData);
            }

            if (!IDatabaseLoader.DecryptData(database.Data, pass, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = PasswordManagerItem.DeserializeItems(data);
            if (items == default)
                return (PasswordValidationResult.WrongFormat, default);

            database.Items = items;
            database.Data = Array.Empty<byte>();

            return default;
        }
        #endregion

        #region Save
        public static bool Save(string path, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            byte[] data;
            string json = PasswordManagerItem.SerializeItems(items);
            byte[] pass;
            
            if (secondFactor == null)
            {
                pass = HashHelper.HashPasswordWithArgon2id(password, salt, associatedData);
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

            if (secondFactor != null)
            {
                pass = HashHelper.HashPasswordWithArgon2id(password, salt, associatedData);
            }
            data = EncryptDecryptHelper.EncryptStringAES(json, pass);

            return IDatabaseLoader.SaveFile(path, data);
        }
        #endregion
    }
}