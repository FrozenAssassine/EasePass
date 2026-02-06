using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Models.Logger
{
    internal class MultiLogger : ILogger
    {
        ILogger[] loggers;

        public MultiLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        public void Flush()
        {
            foreach (var logger in loggers)
                logger.Flush();
        }

        public void Init(string easepassVersion, string framework, string runtimeVersion, string processArch, string osArch, string osDescription, string osVersion, bool is64BitOS, int cpuCount, bool is64BitProcess, long totalMemory, long heapSize, string culture, string uiCulture, string timezone, bool isAdministrator, bool isContainer, string dotNetRoot)
        {
            foreach (var logger in loggers)
                logger.Init(easepassVersion, framework, runtimeVersion, processArch, osArch, osDescription, osVersion, is64BitOS, cpuCount, is64BitProcess, totalMemory, heapSize, culture, uiCulture, timezone, isAdministrator, isContainer, dotNetRoot);
        }

        public void Log(string message)
        {
            foreach(var logger in loggers)
                logger.Log(message);
        }

        public void LogError(string message)
        {
            foreach(var logger in loggers)
                logger.LogError(message);
        }

        public void LogException(Exception exception)
        {
            foreach(var logger in loggers)
                logger.LogException(exception);
        }

        public void LogWarning(string message)
        {
            foreach(var logger in loggers)
                logger.LogWarning(message);
        }
    }
}
