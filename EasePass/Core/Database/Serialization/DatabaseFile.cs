namespace EasePass.Core.Database.Serialization
{
    /// <summary>
    /// The Fileformat of the Database which will be used for Serialization/Deserialization
    /// </summary>
    public class DatabaseFile
    {
        /// <summary>
        /// The Settings of the Database
        /// </summary>
        public DatabaseSettings Settings { get; set; }

        /// <summary>
        /// The Encrypted Passwords. The Passwords will always be encrypted even if the Database does not use a SecondFactor Method
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Deserialize the given <paramref name="json"/> to the <see cref="DatabaseFile"/>
        /// </summary>
        /// <param name="json">The JSON String, which should be deserialized to a <see cref="DatabaseFile"/> object</param>
        /// <returns>Returns an Instance of <see cref="DatabaseFile"/> if the Deserialization was successfull, otherwise <see cref="default"/> will be returned</returns>
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
