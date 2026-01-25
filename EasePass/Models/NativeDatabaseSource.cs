/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePassExtensibility;
using System;
using System.IO;

namespace EasePass.Models
{
    public class NativeDatabaseSource : IDatabaseSource
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
            if (string.IsNullOrEmpty(Path))
                return IDatabaseSource.DatabaseAvailability.UnknownState;
            if(!File.Exists(Path))
                return IDatabaseSource.DatabaseAvailability.Unavailable;
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
