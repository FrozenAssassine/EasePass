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
    /// Use this interface to provide databases to Ease Pass. This could be files from disk, cloud storage or other sources.
    /// </summary>
    public interface IDatabaseProvider : IExtensionInterface
    {
        /// <summary>
        /// Source from which you get the databasefile.
        /// </summary>
        string SourceName { get; }
        /// <summary>
        /// Icon of the source
        /// </summary>
        Uri SourceIcon { get; }
        /// <summary>
        /// Retrieves all available database sources.
        /// </summary>
        /// <returns>An array of <see cref="IDatabaseSource"/> objects representing the available databases. The array is empty if no databases are available.</returns>
        IDatabaseSource[] GetDatabases();
        /// <summary>
        /// Retrieves the current configuration as a JSON-formatted string.
        /// </summary>
        /// <returns>A string containing the configuration data in JSON format. The string is empty if no configuration is
        /// available.</returns>
        string GetConfigurationJSON();
        /// <summary>
        /// Sets the configuration using the specified JSON string.
        /// </summary>
        /// <param name="configJson">A JSON-formatted string that defines the configuration settings to apply. Cannot be null or empty. The JSON
        /// must conform to the expected schema for configuration.</param>
        /// <returns>true if the configuration was successfully applied; otherwise, false.</returns>
        bool SetConfigurationJSON(string configJson);
        /// <summary>
        /// Returns a sample JSON configuration string that demonstrates the expected format for configuration files.
        /// </summary>
        /// <returns>A string containing a sample JSON configuration. The returned string can be used as a template or reference
        /// for creating valid configuration files.</returns>
        string GetSampleJsonConfig();
    }
}
