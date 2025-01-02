using EasePass.Core.Database.Enums;

namespace EasePass.Core.Database.Serialization
{
    /// <summary>
    /// Includes all Settings of the Database
    /// This Class will be used for the Serialization/Deserialization
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Specifies if the Database uses a SecondFactor
        /// </summary>
        public bool UseSecondFactor { get; set; }

        /// <summary>
        /// The Type of the SecondFactor
        /// </summary>
        public SecondFactorType SecondFactorType { get; set; }

        /// <summary>
        /// The Version of the Database, this will be used to check if the File needs to be converted in the Future
        /// </summary>
        public double Version { get; set; }

        /// <summary>
        /// Deserialize the given <paramref name="json"/> to the <see cref="DatabaseSettings"/>
        /// </summary>
        /// <param name="json">The JSON String, which should be deserialized to a <see cref="DatabaseSettings"/> object</param>
        /// <returns>Returns an Instance of <see cref="DatabaseSettings"/> if the Deserialization was successfull, otherwise <see cref="default"/> will be returned</returns>
        public static DatabaseFile Deserialize(string json)
        {
            try
            {
                System.Text.Json.JsonSerializer.Deserialize<DatabaseFile>(json);
            }
            catch { }
            return default;
        }
    }
}
