using EasePass.Core.Database;
using EasePass.Core.Database.Format;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Helper.FileSystem;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Helper.Database
{
    internal static class DatabaseFormatHelper
    {
        #region Load

        public static DatabaseValidationResult ValidateDatabaseWithLatestFormat(
            DatabaseValidationResult file,
            IDatabaseSource source,
            SecureString password)
        {
            // Validate using latest format.
            if (file.result == PasswordValidationResult.Success)
                Core.Database.Format.epdb.MainDatabaseLoader.Save(source, password, default, file.database.Settings, file.database.Items);
            return file;
        }

        /// <summary>
        /// Loads the given Database in the <paramref name="path"/>
        /// </summary>
        /// <param name="source">The Source to the Database</param>
        /// <param name="password">The Password of the Database</param>
        /// <param name="showWrongPasswordError">Specifies if an Error should occure if the Password is wrong</param>
        /// <returns>Returns the <see cref="PasswordValidationResult"/> and the <see cref="DatabaseFile"/>.
        /// If the <see cref="PasswordValidationResult"/> is not equal to <see cref="PasswordValidationResult.Success"/> the
        /// <see cref="DatabaseFile"/> is equal to <see cref="default"/></returns>
        public static async Task<DatabaseValidationResult> Load(IDatabaseSource source, SecureString password, bool showWrongPasswordError)
        {
            if (source is NativeDatabaseSource nativeDBSource && FileHelper.HasExtension(nativeDBSource.Path, "epeb"))
            {
                var resFile = await Core.Database.Format.epeb.MainDatabaseLoader.Load(nativeDBSource, password, showWrongPasswordError);
                nativeDBSource.Path = Path.ChangeExtension(nativeDBSource.Path, "epdb");

                return ValidateDatabaseWithLatestFormat(resFile, source, password);
            }

            DatabaseValidationResult validationRes;
            var preloadedDB = DatabaseVersionTagHelper.GetVersionTag(source.GetDatabaseFileBytes());
            if (preloadedDB.versionTag != DatabaseVersionTag.Undefined)
            {
                // Check all versions from versiontag to prevent decrypting twice
                switch (preloadedDB.versionTag)
                {
                    case DatabaseVersionTag.EpdbV1DbVersion:
                        validationRes = await Core.Database.Format.epdb.v1.DatabaseLoader.Load(source, password, showWrongPasswordError, preloadedDB.data);
                        break;
                    case DatabaseVersionTag.EpdbV2DbVersion:
                        return await Core.Database.Format.epdb.MainDatabaseLoader.Load(source, password, showWrongPasswordError, preloadedDB.data);
                }
                validationRes = new(PasswordValidationResult.WrongFormat, default);
            }
            else
            {    // Versiontag missing. Decrypt using every format ("Bruteforce")

                // Do not show an error because we do not know if the Password is for real wrong since it has changed in the new Version
                validationRes = await Core.Database.Format.epdb.v1.DatabaseLoader.Load(source, password, false, preloadedDB.data);

                if (validationRes.result == PasswordValidationResult.WrongFormat || validationRes.result == PasswordValidationResult.WrongPassword)
                    return await Core.Database.Format.epdb.MainDatabaseLoader.Load(source, password, showWrongPasswordError, preloadedDB.data);
            }

            return ValidateDatabaseWithLatestFormat(validationRes, source, password);
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves the Database to the given <paramref name="source"/> and encrypts the content with the <paramref name="password"/>
        /// </summary>
        /// <param name="source">The Source to the Database</param>
        /// <param name="password">The Master Password of the Database</param>
        /// <param name="secondFactor">The SecondFactor Token of the Database if <see cref="DatabaseSettings.UseSecondFactor"/> is <see langword="true"/></param>
        /// <param name="settings">The Settings of the Database</param>
        /// <param name="items">The PasswordItems of the Database</param>
        /// <returns>Returns the <see langword="true"/> if the Database was saved successfully</returns>
        public static bool Save(IDatabaseSource source, SecureString password, SecureString secondFactor, DatabaseSettings settings, ObservableCollection<PasswordManagerItem> items)
        {
            return IDatabaseLoader.Save(source, password, secondFactor, settings, items);
        }
        #endregion

        #region GetDatabaseLoader
        /// <summary>
        /// Gets the DatabaseLoader as <see cref="Type"/>
        /// </summary>
        /// <param name="version">The Version of the Database</param>
        /// <returns>Returns the <see cref="Type"/> of the DatabaseLoader</returns>
        private static Type GetDatabaseLoader(double version)
        {
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => typeof(IDatabaseLoader).IsAssignableFrom(p)).ToArray();

            foreach (Type type in types)
            {
                double versionValue = (double?)type.GetProperty(nameof(Version)).GetValue(type, null) ?? 0;

                if (versionValue == version)
                    return type;
            }
            return typeof(Core.Database.Format.epdb.MainDatabaseLoader);
        }
        #endregion
    }
}
