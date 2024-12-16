/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using System;
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
