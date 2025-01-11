using EasePass.Core.Database.Enums;
using System;

namespace EasePass.Core.Database.Format.Serialization
{
    /// <summary>
    /// Includes all Settings of the Database
    /// This Class will be used for the Serialization/Deserialization
    /// </summary>
    public class DatabaseSettings : ICloneable
    {
        #region Properties
        /// <summary>
        /// Specifies if the Database uses a SecondFactor
        /// </summary>
        public bool UseSecondFactor { get; set; }

        /// <summary>
        /// The Type of the SecondFactor
        /// </summary>
        public SecondFactorType SecondFactorType { get; set; }
        #endregion

        #region Deserialize
        /// <summary>
        /// Deserialize the given <paramref name="json"/> to the <see cref="DatabaseSettings"/>
        /// </summary>
        /// <param name="json">The JSON String, which should be deserialized to a <see cref="DatabaseSettings"/> object</param>
        /// <returns>Returns an Instance of <see cref="DatabaseSettings"/> if the Deserialization was successfull, otherwise <see cref="default"/> will be returned</returns>
        public static DatabaseSettings Deserialize(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<DatabaseSettings>(json);
            }
            catch { }
            return default;
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Serialize the current Instance of <see cref="DatabaseSettings"/> to a <see cref="string"/>
        /// </summary>
        /// <returns>Returns an Instance of <see cref="string"/> if the Serialization was successfull, otherwise <see cref="string.Empty"/> will be returned</returns>
        public string Serialize() => Serialize(this);
        /// <summary>
        /// Serialize the given <paramref name="settings"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="settings">The JSON String, which should be serialized to a <see cref="string"/></param>
        /// <returns>Returns an Instance of <see cref="string"/> if the Serialization was successfull, otherwise <see cref="string.Empty"/> will be returned</returns>
        public static string Serialize(DatabaseSettings settings)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(settings);
            }
            catch { }
            return string.Empty;
        }
        #endregion

        #region Clone
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}