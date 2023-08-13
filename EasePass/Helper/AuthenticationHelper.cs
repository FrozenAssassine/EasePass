using EasePass.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace EasePass.Helper
{
    internal class AuthenticationHelper
    {
        public static bool VerifyPassword(string password)
        {
            return VerifyPassword(AppSettings.GetSettings(AppSettingsValues.pHash, ""), password);
        }
        public static bool VerifyPassword(string hash, string password)
        {
            if (hash.Length == 0)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public static (string hash, string salt) HashPassword(string password)
        {
            // Generate a random salt
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Hash the password using the salt
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return (hash, salt);
        }

        public static void StorePassword(string password)
        {
            var (hash, salt)= HashPassword(password);
            AppSettings.SaveSettings(AppSettingsValues.pHash, hash);
            AppSettings.SaveSettings(AppSettingsValues.pSalt, salt);
        }
    }
}
