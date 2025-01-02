using EasePass.Core.Database.Serialization;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Core.Database.epdb
{
    internal class MainDatabaseLoader : ADatabaseLoader, IDatabaseLoader
    {
        #region Properties
        public static double Version => 1.1;
        #endregion

        #region Decrypt/Encrypt
        public static string Decrypt()
        {
            return null;
        }

        public static byte[] Encrypt()
        {
            return null;
        }
        #endregion

        #region Load
        /// <summary>
        /// Loads the given Database in the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <param name="password">The Password of the Database</param>
        /// <param name="showWrongPasswordError">Specifies if an Error should occure if the Password is wrong</param>
        /// <returns>Returns the <see cref="PasswordValidationResult"/> and the <see cref="DatabaseFile"/>.
        /// If the <see cref="PasswordValidationResult"/> is not equal to <see cref="PasswordValidationResult.Success"/> the
        /// <see cref="DatabaseFile"/> is equal to <see cref="default"/></returns>
        public new static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            if (!File.Exists(path))
                return (PasswordValidationResult.DatabaseNotFound, default);

            if (!DecryptData(ReadFile(path), password, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            DatabaseFile database = DatabaseFile.Deserialize(data);

            // TODO:
            // - Need to check if SecondFactor is in use
            // - If SecondFactor is in use get the Token
            // - After receiving the Token check the SecondFactor Type
            // - Need to do that how the Decryption have to be done

            if (!DecryptData(database.Data, password, showWrongPasswordError, out data))
                return (PasswordValidationResult.WrongPassword, default);

            database.Items = DeserializePasswordManagerItems(data);
            database.Data = Array.Empty<byte>();

            return (PasswordValidationResult.Success, database);
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            byte[] data;
            string json = SerializePasswordManagerItems(items);
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
            json = database.Serialize();
            data = EncryptDecryptHelper.EncryptStringAES(json, password);

            return SaveFile(path, data);
        }

        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, ObservableCollection<PasswordManagerItem> items)
        {


            return false;
        }
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, SecureString secondFactor, ObservableCollection<PasswordManagerItem> items)
        {


            return false;
        }
        #endregion
    }
}
