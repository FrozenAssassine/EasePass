using EasePass.Dialogs;
using EasePass.Helper;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Models
{
    internal class DatabaseSourceErrorHandlingWrapper : IDatabaseSource
    {
        private IDatabaseSource source;

        public DatabaseSourceErrorHandlingWrapper(IDatabaseSource source)
        {
            this.source = source;
        }

        public string DatabaseName {
            get
            {
                try
                {
                    return source.DatabaseName;
                }
                catch
                {
                    return "Database Name Unavailable";
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
                catch
                {
                    UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                    return "Source Description Unavailable";
                }
            }
        }

        public IDatabaseSource.DatabaseAvailability GetAvailability()
        {
            try
            {
                return source.GetAvailability();
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                return IDatabaseSource.DatabaseAvailability.UnknownState;
            }
        }

        public byte[] GetDatabaseFileBytes()
        {
            try
            {
                return source.GetDatabaseFileBytes();
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                return null;
            }
        }

        public bool SaveDatabaseFileBytes(byte[] databaseFileBytes)
        {
            try
            {
                return source.SaveDatabaseFileBytes(databaseFileBytes);
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                return false;
            }
        }

        public DateTime GetLastTimeModified()
        {
            try
            {
                return source.GetLastTimeModified();
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
                return DateTime.MinValue;
            }
        }

        public void Login()
        {
            try
            {
                source.Login();
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
            }
        }

        public void Logout()
        {
            try
            {
                source.Logout();
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.UnknownDatabaseSourceError(DatabaseName));
            }
        }
    }
}
