using System;
using System.Collections.Generic;
using System.Text;

namespace EasePassExtensibility
{
    public delegate void SaveString(string key, string value);
    public delegate string LoadString(string key);
    public delegate void SaveFile(string filename, byte[] data);
    public delegate byte[] LoadFile(string filename);
    /// <summary>
    /// Could be used by plugins to save and load key value pairs or files.
    /// </summary>
    public interface IStorageInjectable : IExtensionInterface
    {
        /// <summary>
        /// Saves a string value with the specified key.
        /// </summary>
        SaveString SaveString { get; set; }
        /// <summary>
        /// Gets a string value by the specified key. If not found, returns null.
        /// </summary>
        LoadString LoadString { get; set; }
        /// <summary>
        /// Saves a file with the specified filename and data.
        /// </summary>
        SaveFile SaveFile { get; set; }
        /// <summary>
        /// Loads a file by the specified filename. If not found, returns null.
        /// </summary>
        LoadFile LoadFile { get; set; }
    }
}
