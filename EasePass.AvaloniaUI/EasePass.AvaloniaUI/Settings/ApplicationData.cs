using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasePass.Settings
{
    internal static class ApplicationData
    {
        private static string? _localFolder;

        /// <summary>
        /// Call this once on app start to initialize the folder.
        /// </summary>
        public static void Initialize()
        {
            var systemLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _localFolder = Path.Combine(systemLocal, "EasePass");

            if (!Directory.Exists(_localFolder))
                Directory.CreateDirectory(_localFolder);
        }

        /// <summary>
        /// Path to the app's local folder. Initialize() must be called first.
        /// </summary>
        public static string LocalFolder
        {
            get
            {
                if (_localFolder == null)
                    throw new InvalidOperationException("ApplicationData not initialized. Call ApplicationData.Initialize() first.");

                return _localFolder;
            }
        }

        public static string InstalledLocation
        {
            get => AppContext.BaseDirectory;
        }
    }
}
