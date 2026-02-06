using EasePass.Helper.App;
using EasePassExtensibility;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace EasePass.Manager
{
    internal static class LoggingManager
    {
        public static ILogger Logger { get; set; } = null;

        public static void InitializeCurrentLogger()
        {
            if (Logger == null)
                return;

            try
            {
                Logger.Init(
                    AppVersionHelper.GetAppVersion(),
                    RuntimeInformation.FrameworkDescription,
                    Environment.Version.ToString(),
                    RuntimeInformation.ProcessArchitecture.ToString(),
                    RuntimeInformation.OSArchitecture.ToString(),
                    RuntimeInformation.OSDescription,
                    Environment.OSVersion.VersionString,
                    Environment.Is64BitOperatingSystem,
                    Environment.ProcessorCount,
                    Environment.Is64BitProcess,
                    GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
                    GC.GetTotalMemory(forceFullCollection: false),
                    CultureInfo.CurrentCulture.Name,
                    CultureInfo.CurrentUICulture.Name,
                    TimeZoneInfo.Local.Id,
                    Environment.IsPrivilegedProcess,
                    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true",
                    Environment.GetEnvironmentVariable("DOTNET_ROOT"));
            } catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
