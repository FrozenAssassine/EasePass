namespace EasePass.Core.Database.Enums
{
    /// <summary>
    /// The Type of the SecondFactor
    /// </summary>
    public enum SecondFactorType
    {
        /// <summary>
        /// No SecondFactor will be used
        /// </summary>
        None = 0,

        /// <summary>
        /// A Token as SecondFactor the Token will be shown once
        /// </summary>
        Token = 1,

        /// <summary>
        /// A One Time Token will be generated after every Login
        /// </summary>
        OneTimeToken = 2,

        /// <summary>
        /// Authenticatior as SecondFactor
        /// </summary>
        Authenticator = 3,
    }
}
