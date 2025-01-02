﻿using EasePass.Core.Database.Format.Serialization;
using EasePass.Models;
using System.Collections.ObjectModel;
using System.Security;

namespace EasePass.Core.Database.Format.epdb.v1
{
    internal class DatabaseLoader : IDatabaseLoader
    {
        public static double Version => 1.0;

        public static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            byte[] content = IDatabaseLoader.ReadFile(path);
            if (!IDatabaseLoader.DecryptData(content, password, showWrongPasswordError, out string data))
                return (PasswordValidationResult.WrongPassword, default);

            ObservableCollection<PasswordManagerItem> items = IDatabaseLoader.DeserializePasswordManagerItems(data);
            if (items == default)
                return (PasswordValidationResult.WrongFormat, default);

            DatabaseSettings settings = new DatabaseSettings();
            settings.SecondFactorType = Enums.SecondFactorType.None;
            settings.UseSecondFactor = false;

            DatabaseFile database = new DatabaseFile();
            database.DatabaseFileType = Enums.DatabaseFileType.epdb;
            database.Version = Version;
            database.Settings = settings;
            database.Items = items;

            return (PasswordValidationResult.Success, database);
        }
    }
}