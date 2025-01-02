using EasePass.Core.Database.Serialization;
using EasePass.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Core.Database.epdb
{
    internal interface IDatabaseLoader
    {
        public static virtual double Version => 1.1;

        /// <summary>
        /// Loads the given Database in the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path to the Database</param>
        /// <returns>Returns </returns>
        public static virtual (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            if (password == null)
                return (PasswordValidationResult.WrongPassword, null);

            if (!File.Exists(path))
                return (PasswordValidationResult.DatabaseNotFound, null);

            var oldImporterRes = OldDatabaseImporter.CheckValidPassword(path, password, showWrongPasswordError);
            if (oldImporterRes.result == PasswordValidationResult.Success)
            {
                
            }

            var (data, success) = Database.ReadFile(path, password, showWrongPasswordError);
            if (success)
                //return (PasswordValidationResult.Success, data);
                return (PasswordValidationResult.Success, null);

            return (PasswordValidationResult.WrongPassword, null);

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static virtual bool Save(string path, SecureString password, ObservableCollection<PasswordManagerItem> items)
        {


            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static virtual string Decrypt()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static virtual byte[] Encrypt()
        {
            return null;
        }
    }
}
