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
    /// Instances of this interface represent a database source provided by an <see cref="IDatabaseProvider"/>.
    /// </summary>
    public interface IDatabaseSource
    {
        /// <summary>
        /// Name of the database.
        /// </summary>
        string DatabaseName { get; }
        /// <summary>
        /// Returns a unique description of the database source, e.g. the path of the file or a URL.
        /// </summary>
        /// <returns>Database source description</returns>
        string SourceDescription { get; }
        /// <summary>
        /// Availability of the database.
        /// </summary>
        /// <returns><see cref="DatabaseAvailability"/> of the database.</returns>
        DatabaseAvailability GetAvailability();
        /// <summary>
        /// Retrieves the contents of the database file as a byte array.
        /// </summary>
        /// <returns>A byte array containing the full contents of the database file. The array is empty or null if the database file does not exist or is empty.</returns>
        byte[] GetDatabaseFileBytes();
        /// <summary>
        /// Saves the specified database file as a byte array to persistent storage.
        /// </summary>
        /// <param name="databaseFileBytes">The byte array containing the contents of the database file to save. Cannot be null.</param>
        /// <returns>true if the file was saved successfully; otherwise, false.</returns>
        bool SaveDatabaseFileBytes(byte[] databaseFileBytes);
        /// <summary>
        /// Gets the date and time when the object was last modified.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value representing the last modification timestamp of the object.</returns>
        DateTime GetLastTimeModified();
        /// <summary>
        /// Is called when the user logs in to the database. (Use to update DatabaseAvailability)
        /// </summary>
        void Login();
        /// <summary>
        /// Is called when the user logs out of the database. (Use to update DatabaseAvailability)
        /// </summary>
        void Logout();
        /// <summary>
        /// Specifies the availability status of a database for operations.
        /// </summary>
        /// <remarks>Use this enumeration to determine whether a database is accessible, locked by another
        /// user, or in an unknown state. The value can be used to control application logic that depends on database
        /// access.</remarks>
        public enum DatabaseAvailability
        {
            Available,
            LockedByOtherUser,
            UnknownState,
            Unavailable
        }
    }
}
