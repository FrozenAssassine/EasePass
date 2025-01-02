using EasePass.Core.Database.Format.Serialization;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Core.Database.Format.epdb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        #region Properties
        public static double Version => 1.1;
        #endregion

        #region Load
        public static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            if (!File.Exists(path))
                return (PasswordValidationResult.DatabaseNotFound, default);

            if (!IDatabaseLoader.DecryptData(IDatabaseLoader.ReadFile(path), password, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            DatabaseFile database = DatabaseFile.Deserialize(data);

            // TODO:
            // - Need to check if SecondFactor is in use
            // - If SecondFactor is in use get the Token
            // - After receiving the Token check the SecondFactor Type
            // - Need to do that how the Decryption have to be done

            if (!IDatabaseLoader.DecryptData(database.Data, password, showWrongPasswordError, out data))
                return (PasswordValidationResult.WrongPassword, default);

            database.Items = IDatabaseLoader.DeserializePasswordManagerItems(data);
            database.Data = Array.Empty<byte>();

            return (PasswordValidationResult.Success, database);
        }
        #endregion

        #region Save
        public static bool Save(string path, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            byte[] data;
            string json = IDatabaseLoader.SerializePasswordManagerItems(items);
            if (secondFactor == null)
            {
                data = EncryptDecryptHelper.EncryptStringAES(json, password);
            }
            else
            {
                data = EncryptDecryptHelper.EncryptStringAES(json, secondFactor);
            }

            DatabaseFile database = new DatabaseFile();
            database.Settings = settings;
            database.Data = data;
            database.Version = Version;

            json = database.Serialize();
            data = EncryptDecryptHelper.EncryptStringAES(json, password);

            return IDatabaseLoader.SaveFile(path, data);
        }
        #endregion
    }
}