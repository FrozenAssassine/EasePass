using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EasePass.Models.Logger
{
    internal class DebugLineLogger : ILogger
    {
        public void Flush()
        {
            Debug.Flush();
        }

        public void Init(string easepassVersion, string framework, string runtimeVersion, string processArch, string osArch, string osDescription, string osVersion, bool is64BitOS, int cpuCount, bool is64BitProcess, long totalMemory, long heapSize, string culture, string uiCulture, string timezone, bool isAdministrator, bool isContainer, string dotNetRoot)
        {
            // Ignore this. If someone sees the debug log, he knows on which machine he is working.
        }

        public void Log(string message)
        {
            Debug.WriteLine("[log] " + message);
        }

        public void LogError(string message)
        {
            Debug.WriteLine("[warn] " + message);
        }

        public void LogException(Exception exception)
        {
            Debug.WriteLine("[error] " + exception.ToString());
        }

        public void LogWarning(string message)
        {
            Debug.WriteLine("[error] " + message);
        }
    }
}
