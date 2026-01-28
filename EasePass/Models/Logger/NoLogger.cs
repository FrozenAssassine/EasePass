using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Models.Logger
{
    /// <summary>
    /// This logger could be used to suppress everything.
    /// </summary>
    internal class NoLogger : ILogger
    {
        public void Flush()
        {
            
        }

        public void Init(string framework, string runtimeVersion, string processArch, string osArch, string osDescription, string osVersion, bool is64BitOS, int cpuCount, bool is64BitProcess, long totalMemory, long heapSize, string culture, string uiCulture, string timezone, bool isAdministrator, bool isContainer, string dotNetRoot)
        {
            
        }

        public void Log(string message)
        {
            
        }

        public void LogError(string message)
        {
            
        }

        public void LogException(Exception exception)
        {
            
        }

        public void LogWarning(string message)
        {
            
        }
    }
}
