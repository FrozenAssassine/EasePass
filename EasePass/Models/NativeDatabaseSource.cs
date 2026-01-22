using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasePass.Models
{
    internal class NativeDatabaseSource : IDatabaseSource
    {
        public string Path;

        public NativeDatabaseSource(string databaseFilePath)
        {
            Path = databaseFilePath;
        }

        public string DatabaseName => System.IO.Path.GetFileNameWithoutExtension(Path);

        public string SourceDescription => Path;

        public IDatabaseSource.DatabaseAvailability GetAvailability()
        {
            return IDatabaseSource.DatabaseAvailability.Available;
        }

        public byte[] GetDatabaseFileBytes()
        {
            if (!File.Exists(Path))
                return null;
            return File.ReadAllBytes(Path);
        }

        public DateTime GetLastTimeModified()
        {
            return File.GetLastWriteTime(Path);
        }

        public void Login() { }

        public void Logout() { }

        public bool SaveDatabaseFileBytes(byte[] databaseFileBytes)
        {
            try
            {
                File.WriteAllBytes(Path, databaseFileBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
