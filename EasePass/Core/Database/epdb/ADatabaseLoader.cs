using EasePass.Core.Database.Serialization;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Core.Database.epdb
{
    public abstract class ADatabaseLoader
    {
        #region ConvertToDatabaseFile
        private protected static (PasswordValidationResult result, DatabaseFile database) ConvertToDatabaseFile((PasswordValidationResult result, string decryptedData) content)
        {
            DatabaseSettings settings = new DatabaseSettings();
            settings.SecondFactorType = Enums.SecondFactorType.None;
            settings.UseSecondFactor = false;
            settings.Version = MainDatabaseLoader.Version;

            DatabaseFile databaseFile = new DatabaseFile();
            databaseFile.Settings = settings;
            databaseFile.Items = GetItems(content.decryptedData);
            databaseFile.Data = null;
            
            return (content.result, databaseFile);
        }
        private static ObservableCollection<PasswordManagerItem> GetItems(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<ObservableCollection<PasswordManagerItem>>(json);
            }
            catch
            {
                InfoMessages.DatabaseInvalidData();
                return null;
            }
        }
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

        #region GetDatabaseLoader
        /// <summary>
        /// Gets the DatabaseLoader as <see cref="Type"/>
        /// </summary>
        /// <param name="version">The Version of the Database</param>
        /// <returns>Returns the <see cref="Type"/> of the DatabaseLoader</returns>
        public static Type GetDatabaseLoader(double version)
        {
            return IDatabaseLoader.GetDatabaseLoader(version);
        }
        #endregion

        #region Load
        /// <summary>
        /// Loads the given Database in the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <param name="password">The Password of the Database</param>
        /// <param name="showWrongPasswordError">Specifies if an Error should occure if the Password is wrong</param>
        /// <returns>Returns </returns>
        public static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            if (password == null)
                return (PasswordValidationResult.WrongPassword, null);

            if (!File.Exists(path))
                return (PasswordValidationResult.DatabaseNotFound, null);

            var oldImporterRes = OldDatabaseImporter.CheckValidPassword(path, password, showWrongPasswordError);
            if (oldImporterRes.result == PasswordValidationResult.Success)
                return ConvertToDatabaseFile(oldImporterRes);

            var (data, success) = Database.ReadFile(path, password, showWrongPasswordError);
            if (success)
                return ConvertToDatabaseFile((PasswordValidationResult.Success, data));



            return (PasswordValidationResult.WrongPassword, null);
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

        #region Save
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, SecureString secondFactor, DatabaseSettings settings)
        {
            return MainDatabaseLoader.Save(path, password, secondFactor, settings);
        }
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, ObservableCollection<PasswordManagerItem> items)
        {
            return MainDatabaseLoader.Save(path, password, items);
        }
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, SecureString secondFactor, ObservableCollection<PasswordManagerItem> items)
        {
            return MainDatabaseLoader.Save(path, password, secondFactor, items);
        }
        #endregion
    }
}
