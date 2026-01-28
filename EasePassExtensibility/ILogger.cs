using System;
using System.Collections.Generic;
using System.Text;

namespace EasePassExtensibility
{
    /// <summary>
    /// Implement this interface, if you provide a new type of logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// This function provides initial environment information to the logger. This function is not necessarily called. All values that cannot be provided are null or empty.
        /// </summary>
        void Init(string framework, string runtimeVersion, string processArch, string osArch, string osDescription, string osVersion, bool is64BitOS, int cpuCount, bool is64BitProcess, long totalMemory, long heapSize, string culture, string uiCulture, string timezone, bool isAdministrator, bool isContainer, string dotNetRoot);
        /// <summary>
        /// Logs something, e.g. for debugging purposes or whatever...
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);
        /// <summary>
        /// Logs a warning to tell the developer to have a closer look at this.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogWarning(string message);
        /// <summary>
        /// Logs an error to tell the developer here is something wrong.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogError(string message);
        /// <summary>
        /// Logs an exception. Has the same job/importance as LogError, but provides additional information.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        void LogException(Exception exception);
        /// <summary>
        /// Tells the logger, that this is maybe the last possibility to save its data.
        /// </summary>
        void Flush();
    }
}
