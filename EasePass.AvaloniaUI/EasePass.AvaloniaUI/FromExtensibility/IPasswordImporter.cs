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

namespace EasePassExtensibility
{
    /// <summary>
    /// Implement this interface to create an element in the "Import passwords" section on the settings page. It's made to import passwords from other services or password storages.
    /// </summary>
    public interface IPasswordImporter : IExtensionInterface
    {
        /// <summary>
        /// Source from which you get the passwords.
        /// </summary>
        string SourceName { get; }
        /// <summary>
        /// Icon of the source
        /// </summary>
        Uri SourceIcon { get; }
        /// <summary>
        /// Ease Pass will call this function to get the passwords from this source.
        /// </summary>
        /// <returns>List of the passwords.</returns>
        PasswordItem[] ImportPasswords();
        /// <summary>
        /// Ease Pass will call this function to check if passwords are available. Ease Pass will not call this function every time.
        /// </summary>
        /// <returns></returns>
        bool PasswordsAvailable();
    }
}
