using EasePass.Core.Database.Format.epdb;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;

namespace EasePass.Core.Database.Format
{
    /// <summary>
    /// Interface for the DatabaseLoader
    /// </summary>
    public interface IDatabaseLoader
    {
        #region Properties
        /// <summary>
        /// The Version of the File.
        /// </summary>
        public static abstract double Version { get; }
        #endregion

        #region DecryptData
        /// <summary>
        /// Checks if the <paramref name="content"/> can be decrypted with the given <paramref name="password"/>
        /// </summary>
        /// <param name="content">The Content, which should be decrypted</param>
        /// <param name="password">The Password, which should be used for the decryption</param>
        /// <param name="showWrongPasswordError">Specifies if a Message will be shown if the Password is wrong</param>
        /// <param name="decryptedData">Contains the decrypted Data if the <paramref name="content"/> could be decrypted, otherwise contain <see cref="string.Empty"/></param>
        /// <returns>Returns <see langword="true"/> if the <paramref name="content"/> could be encrypted, otherwise <see langword="false"/></returns>
        private protected static bool DecryptData(byte[] content, SecureString password, bool showWrongPasswordError, out string decryptedData)
        {
            decryptedData = string.Empty;
            var result = EncryptDecryptHelper.DecryptStringAES(content, password);

            if (!result.correctPassword)
            {
                if (showWrongPasswordError)
                {
                    InfoMessages.ImportDBWrongPassword();
                }
                return false;
            }

            decryptedData = result.decryptedString;
            return true;
        }
        #endregion

        #region Deserialize/SerializePasswordManagerItems
        /// <summary>
        /// Deserialize the given <paramref name="json"/> to the <see cref="ObservableCollection{PasswordManagerItem}"/>
        /// </summary>
        /// <param name="json">The JSON String, which should be deserialized to a <see cref="ObservableCollection{PasswordManagerItem}"/> object</param>
        /// <returns>Returns an Instance of <see cref="ObservableCollection{PasswordManagerItem}"/> if the Deserialization was successfull, otherwise <see cref="default"/> will be returned</returns>
        [Obsolete]
        private protected static ObservableCollection<PasswordManagerItem> DeserializePasswordManagerItems(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<PasswordManagerItem>>(json);
            }
            catch { }
            return default;
        }
        /// <summary>
        /// Serialize the given <paramref name="items"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="json">The JSON String, which should be serialized to a <see cref="string"/></param>
        /// <returns>Returns an Instance of <see cref="string"/> if the Serialization was successfull, otherwise <see cref="string.Empty"/> will be returned</returns>
        [Obsolete]
        private protected static string SerializePasswordManagerItems(ObservableCollection<PasswordManagerItem> items)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(items);
            }
            catch { }
            return string.Empty;
        }
        #endregion

        #region ReadFile
        /// <summary>
        /// Reads the <see cref="byte"/>[] of the File in the given <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see cref="byte"/>[] of the Databasefile. If a Error occures <see cref="Array.Empty{T}"/> will be returned</returns>
        private protected static byte[] ReadFile(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                InfoMessages.DatabaseFileNotFoundAt(path);
            }
            catch (UnauthorizedAccessException)
            {
                InfoMessages.NoAccessToPathDatabaseNotLoaded(path);
            }
            return Array.Empty<byte>();
        }
        #endregion

        #region SaveFile
        /// <summary>
        /// Saves the <paramref name="content"/> to the File given in the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path to the File, which should be saved</param>
        /// <param name="content">The Content, which should be written in the File</param>
        /// <returns>Returns <see langword="true"/> if the File was saved successfully, otherwise <see langword="false"/> will be returned</returns>
        private protected static bool SaveFile(string path, byte[] content)
        {
            try
            {
                File.WriteAllBytes(path, content);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                InfoMessages.NoAccessToPathDatabaseNotSaved(path);
            }
            catch (IOException)
            {
                InfoMessages.DatabaseSaveToFileError(path);
            }
            return false;
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
        public abstract static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError);
        #endregion

        #region Save
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <param name="password">The Master Password of the Database</param>
        /// <param name="secondFactor">The SecondFactor Token of the Database if <see cref="DatabaseSettings.UseSecondFactor"/> is <see langword="true"/></param>
        /// <param name="settings">The Settings of the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            return MainDatabaseLoader.Save(path, password, secondFactor, settings, items);
        }
        #endregion
    }
}