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

using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Manager;
using EasePassExtensibility;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EasePass.Models
{
    /// <summary>
    /// Adds proper error handling to an <see cref="IDatabaseSource"/> implementation. Important for sources that rely on external resources (plugins).
    /// </summary>
    internal class DatabaseSourceErrorHandlingWrapper : IDatabaseSource, INotifyPropertyChanged
    {
        private IDatabaseSource source;

        public event PropertyChangedEventHandler PropertyChanged;

        public DatabaseSourceErrorHandlingWrapper(IDatabaseSource source)
        {
            this.source = source;
            this.source.OnPropertyChanged += () =>
            {
                OnPropertyChanged?.Invoke();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DatabaseName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceDescription"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsReadOnly"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Availability"));
            };
        }

        private void Log(string method, Exception e)
        {
            LoggingManager.Logger.LogError("[external_datasource] [" + method + "] " + e.ToString());
        }

        public string DatabaseName {
            get
            {
                try
                {
                    return source.DatabaseName;
                }
                catch (Exception ex)
                {
                    Log("DatabaseName", ex);
                    return "unknown";
                }
            }
        }

        public string SourceDescription
        {
            get
            {
                try
                {
                    return source.SourceDescription;
                }
                catch (Exception ex)
                {
                    Log("SourceDescription", ex);
                    UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                    return "Source Description Unavailable";
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                try
                {
                    return source.IsReadOnly;
                }
                catch (Exception ex)
                {
                    Log("IsReadOnly", ex);
                    UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                    return true;
                }
            }
        }

        public Action OnPropertyChanged { get; set; }

        public IDatabaseSource.DatabaseAvailability Availability
        {
            get
            {
                try
                {
                    return source.Availability;
                }
                catch (Exception ex)
                {
                    Log("Availability", ex);
                    UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                    return IDatabaseSource.DatabaseAvailability.UnknownState;
                }
            }
        }

        public DateTime LastTimeModified
        {
            get
            {
                try
                {
                    return source.LastTimeModified;
                }
                catch (Exception ex)
                {
                    Log("LastTimeModified", ex);
                    UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                    return DateTime.MinValue;
                }
            }
        }

        public Task<byte[]> GetDatabaseFileBytes()
        {
            try
            {
                return source.GetDatabaseFileBytes();
            }
            catch (Exception ex)
            {
                Log("GetDatabaseFileBytes", ex);
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                return null;
            }
        }

        public Task<bool> SaveDatabaseFileBytes(byte[] databaseFileBytes)
        {
            try
            {
                return source.SaveDatabaseFileBytes(databaseFileBytes);
            }
            catch (Exception ex)
            {
                Log("SaveDatabaseFileBytes", ex);
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                return Task.FromResult(false);
            }
        }

        public void Login()
        {
            try
            {
                source.Login();
            }
            catch (Exception ex)
            {
                Log("Login", ex);
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
            }
        }

        public void Logout()
        {
            try
            {
                source.Logout();
            }
            catch (Exception ex)
            {
                Log("Logout", ex);
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
            }
        }
    }
}
