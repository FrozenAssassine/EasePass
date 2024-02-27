using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    /// <summary>
    /// Implement this interface to create a password generator for Ease Pass.
    /// </summary>
    [Obsolete("This functionality isn't fully implemented yet!")]
    public interface IPasswordGenerator
    {
        /// <summary>
        /// Name of the password generator.
        /// </summary>
        string GeneratorName { get; }
        /// <summary>
        /// Tells the plugin the current user settings about password generating.
        /// </summary>
        /// <param name="desiredLength">The desired password length, specified by the user.</param>
        /// <param name="desiredChars">The desired chars the password should contain, specified by the user.</param>
        /// <param name="verifyNotLeaked">True, if the password should not be leaked.</param>
        /// <param name="isLeaked">Delegate to check for leaks.</param>
        void Init(int desiredLength, string desiredChars, bool verifyNotLeaked, Func<string, Task<bool?>> isLeaked);
        /// <summary>
        /// Ease Pass will call this function to generate a new password.
        /// </summary>
        /// <returns>The new generated password.</returns>
        string Generate();
    }
}
