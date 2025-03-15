using EasePass.Core.Database.Format.epdb;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper.Security;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;

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
        private protected static bool DecryptData(byte[] content, byte[] password, bool showWrongPasswordError, out string decryptedData)
        {
            decryptedData = string.Empty;
            var result = EncryptDecryptHelper.DecryptStringAES(content, password);

            return DecryptInternal(result, showWrongPasswordError, ref decryptedData);
        }
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

            return DecryptInternal(result, showWrongPasswordError, ref decryptedData);
        }
        private static bool DecryptInternal((string decryptedString, bool correctPassword) result, bool showWrongPasswordError, ref string decryptedData)
        {
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
        public abstract static Task<(PasswordValidationResult result, DatabaseFile database)> Load(string path, SecureString password, bool showWrongPasswordError);

        /// <summary>
        /// Loads the given Database in the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <param name="password">The Password of the Database</param>
        /// <param name="showWrongPasswordError">Specifies if an Error should occure if the Password is wrong</param>
        /// <returns>Returns the <see cref="PasswordValidationResult"/> and the <see cref="DatabaseFile"/>.
        /// If the <see cref="PasswordValidationResult"/> is not equal to <see cref="PasswordValidationResult.Success"/> the
        /// <see cref="DatabaseFile"/> is equal to <see cref="default"/></returns>
        public static Task<(PasswordValidationResult result, DatabaseFile database)> Load<T>(string path, SecureString password, bool showWrongPasswordError) where T : IDatabaseLoader
        {
            return T.Load(path, password, showWrongPasswordError);
        }

        /// <summary>
        /// Loads the given <paramref name="database"/>
        /// Do not use this Method if you do not know how it works!!!
        /// This Method is used as Callback if a DatabaseLoader noticed that the Database has a Version of a DatabaseLoader.
        /// We do not just call the Load Method to prevent doing the same thing twice since we have already loaded the Database
        /// </summary>
        /// <param name="password">The Password of the Database</param>
        /// <param name="database">The Database, which should be loaded</param>
        /// <returns>Returns the <see cref="PasswordValidationResult"/> and the <see cref="DatabaseFile"/>.
        /// If the <see cref="PasswordValidationResult"/> is not equal to <see cref="PasswordValidationResult.Success"/> the
        /// <see cref="DatabaseFile"/> is equal to <see cref="default"/></returns>
        public abstract static Task<(PasswordValidationResult result, DatabaseFile database)> LoadInternal(SecureString password, DatabaseFile database, bool showWrongPasswordError);
        #endregion

        #region Save
        /// <summary>
        /// Saves the Database to the given <paramref name="path"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <param name="password">The Master Password of the Database</param>
        /// <param name="secondFactor">The SecondFactor Token of the Database if <see cref="DatabaseSettings.UseSecondFactor"/> is <see langword="true"/></param>
        /// <param name="settings">The Settings of the Database</param>
        /// <param name="items">The PasswordItems of the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(string path, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            return MainDatabaseLoader.Save(path, password, secondFactor, settings, items);
        }
        #endregion
    }
}