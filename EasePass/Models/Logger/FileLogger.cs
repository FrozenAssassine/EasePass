using EasePassExtensibility;
using System;
using System.IO;
using System.Text;
using Windows.Storage;

namespace EasePass.Models.Logger
{
    internal class FileLogger : ILogger
    {
        private static readonly string LogsFolderPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs");

        private string filename = CreateNewLogFilename();

        StringBuilder sb = new StringBuilder();

        public FileLogger()
        {
            if(!Directory.Exists(LogsFolderPath))
                Directory.CreateDirectory(LogsFolderPath);

            RemoveOldLogs();
        }

        private void RemoveOldLogs()
        {
            const int thresholdWeeks = 4;

            string[] files = Directory.GetFiles(LogsFolderPath);
            DateTime now = DateTime.Now;
            int removeCount = 0;
            foreach (string file in files)
            {
                DateTime lastWrite = File.GetLastWriteTime(file);
                if ((now - lastWrite).TotalDays > thresholdWeeks * 7)
                {
                    File.Delete(file);
                    removeCount++;
                }
            }
            sb.AppendLine("Removed " + removeCount + " old logfiles.");
            sb.AppendLine();
        }

        private static string CreateNewLogFilename()
        {
            string filename = DateTime.Now.ToString("s").Replace(':', '-');
            return Path.Combine(LogsFolderPath, filename + ".txt");
        }

        public void Init(
            string easepassVersion,
            string framework,
            string runtimeVersion,
            string processArch,
            string osArch,
            string osDescription,
            string osVersion,
            bool is64BitOS,
            int cpuCount,
            bool is64BitProcess,
            long totalMemory,
            long heapSize,
            string culture,
            string uiCulture,
            string timezone,
            bool isAdministrator,
            bool isContainer,
            string dotNetRoot)
        {
            sb.AppendLine("Ease Pass Version: " + easepassVersion);
            sb.AppendLine(".NET Framework: " + framework);
            sb.AppendLine(".NET Root: " + dotNetRoot);
            sb.AppendLine("Runtime Version: " + runtimeVersion);
            sb.AppendLine("Process Architecture: " + processArch);
            sb.AppendLine("64 Bit Process: " + is64BitProcess);
            sb.AppendLine("OS Architecture: " + osArch);
            sb.AppendLine("OS Description: " + osDescription);
            sb.AppendLine("OS Version: " + osVersion);
            sb.AppendLine("64 Bit OS: " + is64BitOS);
            sb.AppendLine("CPU Count: " + cpuCount);
            sb.AppendLine("Total Memory (Bytes): " + totalMemory);
            sb.AppendLine("Heap Size: " + heapSize);
            sb.AppendLine("Culture: " + culture);
            sb.AppendLine("UI Culture: " + uiCulture);
            sb.AppendLine("Timezone: " + timezone);
            sb.AppendLine("Administrator: " + isAdministrator);
            sb.AppendLine("Is Container: " + isContainer);
            sb.AppendLine();
        }

        public void Log(string message)
        {
            sb.AppendLine("[log] " + message);
        }

        public void LogWarning(string message)
        {
            sb.AppendLine("[warn] " + message);
        }

        public void LogError(string message)
        {
            sb.AppendLine("[error] " + message);
        }

        public void LogException(Exception exception)
        {
            sb.AppendLine("[error] " + exception.ToString());
        }

        public void Flush()
        {
            File.WriteAllText(filename, sb.ToString());
        }
    }
}
