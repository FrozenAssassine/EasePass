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
    }
}
