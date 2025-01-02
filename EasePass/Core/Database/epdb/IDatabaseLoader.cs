using System;
using System.Linq;

namespace EasePass.Core.Database.epdb
{
    public interface IDatabaseLoader
    {
        #region Properties
        /// <summary>
        /// The Version of the File.
        /// </summary>
        public static abstract double Version { get; }
        #endregion

        #region Decrypt/Encrypt
        /// <summary>
        /// Decrypts the Database content
        /// </summary>
        /// <returns>Returns the decrypted Databasecontent</returns>
        public static abstract string Decrypt();

        /// <summary>
        /// Encrypts the Database content
        /// </summary>
        /// <returns>Returns the encrypted Databasecontent</returns>
        public static abstract byte[] Encrypt();
        #endregion

        #region GetDatabaseLoader
        /// <summary>
        /// Gets the DatabaseLoader as <see cref="Type"/>
        /// </summary>
        /// <param name="version">The Version of the Database</param>
        /// <returns>Returns the <see cref="Type"/> of the DatabaseLoader</returns>
        public static Type GetDatabaseLoader(double version)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => typeof(IDatabaseLoader).IsAssignableFrom(p)).ToArray();

            foreach (var type in types)
            {
                double versionValue = ((double?)type.GetProperty(nameof(Version)).GetValue(type, null)) ?? 0;

                if (versionValue == version)
                    return type;
            }
            return typeof(MainDatabaseLoader);
        }
        #endregion
    }
}