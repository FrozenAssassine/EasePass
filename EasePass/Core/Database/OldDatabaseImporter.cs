using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;

namespace EasePass.Core.Database;

internal class OldDatabaseImporter
{
    public static string GetDecryptedString(string path, SecureString password)
    {
        if (FileHelper.HasExtension(path, ".eped"))
        {
            string pw = new System.Net.NetworkCredential(string.Empty, password).Password;
            EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(path));
            if (!AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, pw))
            {
                InfoMessages.ImportDBWrongPassword();
                return null;
            }
            var res = EncryptDecryptHelper.DecryptStringAES(decryptedJson.Data, pw, decryptedJson.Salt);
            if (!res.correctPassword)
            {
                InfoMessages.ImportDBWrongPassword();
                return null;
            }

            return res.decryptedString;
        }
        return null;

    }

    public static (PasswordValidationResult result, string decryptedData) CheckValidPassword(string path, SecureString enteredPassword, bool showWrongPasswordError = false)
    {
        if (FileHelper.HasExtension(path, ".epeb"))
        {
            string pw = new System.Net.NetworkCredential(string.Empty, enteredPassword).Password;
            EncryptedDatabaseItem decryptedJson = JsonConvert.DeserializeObject<EncryptedDatabaseItem>(File.ReadAllText(path));
            if (!AuthenticationHelper.VerifyPassword(decryptedJson.PasswordHash, pw))
            {
                if (showWrongPasswordError)
                    InfoMessages.ImportDBWrongPassword();
                return (PasswordValidationResult.WrongPassword, null);
            }
            var res = EncryptDecryptHelper.DecryptStringAES(decryptedJson.Data, pw, decryptedJson.Salt);
            if (!res.correctPassword)
            {
                if (showWrongPasswordError)
                    InfoMessages.ImportDBWrongPassword();
                return (PasswordValidationResult.WrongPassword, null);
            }
            return (PasswordValidationResult.Success, res.decryptedString);
        }
        return (PasswordValidationResult.DatabaseNotFound, null);
    }

    public static void CheckAndFixFile(DatabaseItem dbItem)
    {
        if (FileHelper.HasExtension(dbItem.Path, ".eped"))
        {
            dbItem.Path = Path.ChangeExtension(dbItem.Path, "epdb");
            dbItem.Save();
        }
    }
}