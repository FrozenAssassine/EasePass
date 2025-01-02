using EasePass.Core.Database.Format.Serialization;
using EasePass.Models;
using System.Security;

namespace EasePass.Core.Database.Format.epeb
{
    internal class MainDatabaseLoader : IDatabaseLoader
    {
        public static double Version => 1.0;

        public static (PasswordValidationResult result, DatabaseFile database) Load(string path, SecureString password, bool showWrongPasswordError)
        {
            throw new System.NotImplementedException();
        }
    }
}
