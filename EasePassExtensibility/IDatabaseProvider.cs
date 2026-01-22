using System;
using System.Collections.Generic;
using System.Text;

namespace EasePassExtensibility
{
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
    }
}
