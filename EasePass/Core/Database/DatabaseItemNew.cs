using EasePass.Core.Database.Enums;
using EasePass.Core.Database.Serialization;
using EasePass.Helper;
using EasePass.Models;
using Newtonsoft.Json;
using System.IO;
using System.Security;

namespace EasePass.Core.Database
{
    /// <summary>
    /// New Implementation of the DatabaseItem to allow SecondFactors
    /// </summary>
    internal class DatabaseItemNew : DatabaseItem
    {
        #region Fields & Properties
        /// <summary>
        /// The Settings of the Database
        /// </summary>
        private DatabaseSettings settings;

        /// <summary>
        /// Specifies if the Database uses a SecondFactor
        /// </summary>
        public bool UseSecondFactor => settings.UseSecondFactor;
        
        /// <summary>
        /// The Type of the SecondFactor
        /// </summary>
        public SecondFactorType SecondFactorType => settings.SecondFactorType;
        
        /// <summary>
        /// The Token of the SecondFactor, which will be used for decryption/encryption
        /// </summary>
        public SecureString SecondFactorToken { get; set; }
        #endregion

        #region Constructor
        public DatabaseItemNew(string path) : base(path) { }
        #endregion

        #region Methods


        public override (PasswordValidationResult result, string decryptedData) CheckPasswordCorrect(SecureString enteredPassword, bool showWrongPasswordError = false)
        {
            if (enteredPassword == null)
                return (PasswordValidationResult.WrongPassword, null);

            if (!File.Exists(Path))
                return (PasswordValidationResult.DatabaseNotFound, null);

            var oldImporterRes = OldDatabaseImporter.CheckValidPassword(this.Path, enteredPassword, showWrongPasswordError);
            if (oldImporterRes.result == PasswordValidationResult.Success)
                return oldImporterRes;

            var (data, success) = Database.ReadFile(Path, enteredPassword, showWrongPasswordError);
            if (success)
                return (PasswordValidationResult.Success, data);

            return (PasswordValidationResult.WrongPassword, null);
        }

        public override void Save(string path = null)
        {
            if (MasterPassword == default || string.IsNullOrWhiteSpace(path))
                return;

            byte[] bytes;
            string json = CreateJsonstring(Items);
            if (UseSecondFactor)
            {
                if (SecondFactorToken == default)
                    return;

                bytes = EncryptDecryptHelper.EncryptStringAES(json, SecondFactorToken);
            }
            else
            {
                bytes = EncryptDecryptHelper.EncryptStringAES(json, MasterPassword);
            }

            DatabaseFile databaseFile = new DatabaseFile();
            databaseFile.Data = bytes;
            databaseFile.Settings = settings;

            Database.WriteFile(path, CreateJsonstring(databaseFile), MasterPassword);
        }

        public override bool Unlock(SecureString password, bool showWrongPasswordError = true)
        {
            return base.Unlock(password, showWrongPasswordError);
        }
        public static string CreateJsonstring<T>(T value)
        {
            if (value == null)
                return string.Empty;

            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
        #endregion
    }
}
