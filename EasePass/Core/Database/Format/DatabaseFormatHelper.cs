using EasePass.Core.Database.Enums;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Helper;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Core.Database.Format
{
    internal static class DatabaseFormatHelper
    {
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
        public static async Task<(PasswordValidationResult result, DatabaseFile database)> Load(string path, SecureString password, bool showWrongPasswordError)
        {
            (PasswordValidationResult result, DatabaseFile database) file;
            if (FileHelper.HasExtension(path, "epeb"))
            {
                file = await epeb.MainDatabaseLoader.Load(path, password, showWrongPasswordError);
                path = Path.ChangeExtension(path, "epdb");
            }
            else
            {
                file = await epdb.v1.DatabaseLoader.Load(path, password, showWrongPasswordError);
                
                if (file.result == PasswordValidationResult.WrongFormat)
                    return await epdb.MainDatabaseLoader.Load(path, password, showWrongPasswordError);
            }

            if (file.result == PasswordValidationResult.Success)
            {
                epdb.MainDatabaseLoader.Save(path, password, default, file.database.Settings, file.database.Items);
            }
            return file;
        }
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
            return IDatabaseLoader.Save(path, password, secondFactor, settings, items);
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
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => typeof(IDatabaseLoader).IsAssignableFrom(p)).ToArray();

            foreach (var type in types)
            {
                double versionValue = (double?)type.GetProperty(nameof(Version)).GetValue(type, null) ?? 0;

                if (versionValue == version)
                    return type;
            }
            return typeof(epdb.MainDatabaseLoader);
        }
        #endregion
    }
}
