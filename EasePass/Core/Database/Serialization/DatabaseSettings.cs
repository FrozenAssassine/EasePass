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
        public string DatabaseVersion { get; set; }
    }
}
