using System;
using System.Collections.Generic;
using System.Text;

namespace EasePassExtensibility
{
    /// <summary>
    /// Implement this interface to get a logger instance.
    /// </summary>
    public interface ILoggerInjectable : IExtensionInterface
    {
        /// <summary>
        /// A logger instance.
        /// </summary>
        ILogger Logger { get; set; }
    }
}
