using EasePass.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace EasePass.Core.Database.Serialization
{
    /// <summary>
    /// The Fileformat of the Database which will be used for Serialization/Deserialization
    /// </summary>
    public class DatabaseFile
    {
        #region Properties
        /// <summary>
        /// The Settings of the Database
        /// </summary>
        public DatabaseSettings Settings { get; set; }

        /// <summary>
        /// The Encrypted Passwords. The Passwords will always be encrypted even if the Database does not use a SecondFactor Method
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// The Passwords in the Database
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<PasswordManagerItem> Items { get; set; } = null;
        #endregion

        #region Deserialize
        /// <summary>
        /// Deserialize the given <paramref name="json"/> to the <see cref="DatabaseFile"/>
        /// </summary>
        /// <param name="json">The JSON String, which should be deserialized to a <see cref="DatabaseFile"/> object</param>
        /// <returns>Returns an Instance of <see cref="DatabaseFile"/> if the Deserialization was successfull, otherwise <see cref="default"/> will be returned</returns>
        public static DatabaseFile Deserialize(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<DatabaseFile>(json);
            }
            catch { }
            return default;
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Serialize the current Instance of <see cref="DatabaseFile"/> to a <see cref="string"/>
        /// </summary>
        /// <returns>Returns an Instance of <see cref="string"/> if the Serialization was successfull, otherwise <see cref="string.Empty"/> will be returned</returns>
        public string Serialize() => Serialize(this);
        /// <summary>
        /// Serialize the given <paramref name="settings"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="settings">The JSON String, which should be serialized to a <see cref="string"/></param>
        /// <returns>Returns an Instance of <see cref="string"/> if the Serialization was successfull, otherwise <see cref="string.Empty"/> will be returned</returns>
        public static string Serialize(DatabaseFile settings)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(settings);
            }
            catch { }
            return string.Empty;
        }
        #endregion
    }
}
