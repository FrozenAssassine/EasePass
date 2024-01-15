using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    public interface IPasswordGenerator
    {
        string GeneratorName { get; }
        /// <summary>
        /// Tells the plugin the current user settings about password generating.
        /// </summary>
        /// <param name="desiredLength">The desired password length, specified by the user.</param>
        /// <param name="desiredChars">The desired chars the password should contain, specified by the user.</param>
        /// <param name="verifyNotLeaked">True, if the password should not be leaked.</param>
        /// <param name="isLeaked">Delegate to check for leaks.</param>
        void Init(int desiredLength, string desiredChars, bool verifyNotLeaked, Func<string, Task<bool?>> isLeaked);
        string Generate();
    }
}
