using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Core.Database.Format.epdb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        #region Properties
        public static double Version => 1.1;
        #endregion

        #region Load
        public static async Task<(PasswordValidationResult result, DatabaseFile database)> Load(string path, SecureString password, bool showWrongPasswordError)
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

            if (database == default)
                return (PasswordValidationResult.WrongFormat, default);

            SecureString pw;
            if (database.Settings.UseSecondFactor)
            {
                EnterSecondFactorDialog secondFactorDialog = new EnterSecondFactorDialog();
                await secondFactorDialog.ShowAsync();
                pw = secondFactorDialog.Token;
                database.SecondFactor = pw;
            }
            else
            {
                pw = password;
            }

            if (!IDatabaseLoader.DecryptData(database.Data, pw, showWrongPasswordError, out data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = IDatabaseLoader.DeserializePasswordManagerItems(data);
            if (items == default)
                return (PasswordValidationResult.WrongFormat, default);

            database.Items = items;
            database.Data = Array.Empty<byte>();

            return (PasswordValidationResult.Success, database);
        }

        public static async Task<(PasswordValidationResult result, DatabaseFile database)> LoadInternal(SecureString password, DatabaseFile database)
        {
            return default;
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
            database.DatabaseFileType = Enums.DatabaseFileType.epdb;
            database.Version = Version;
            database.Settings = settings;
            database.Data = data;

            json = database.Serialize();
            data = EncryptDecryptHelper.EncryptStringAES(json, password);

            return IDatabaseLoader.SaveFile(path, data);
        }
        #endregion
    }
}