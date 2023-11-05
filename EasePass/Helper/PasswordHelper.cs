using EasePass.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    public static class PasswordHelper
    {
        private static bool disableLeakedPasswords = false;

        public static async Task<string> GeneratePassword()
        {
            disableLeakedPasswords = AppSettings.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords);
            string password = "";
            int length = AppSettings.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength);
            string chars = AppSettings.GetSettings(AppSettingsValues.passwordChars, DefaultSettingsValues.PasswordChars);
            Random r = new Random();
            while (!await IsSecure(chars, length, password))
            {
                if (password.Length > length)
                    password = "";
                password += chars[r.Next(chars.Length)];
            }
            return password;
        }

        private static async Task<bool> IsSecure(string chars, int length, string password)
        {
            int maxpoints = 1; // 1 because of length
            if (!disableLeakedPasswords) maxpoints++;
            if (chars.Any(char.IsDigit)) maxpoints++;
            if (chars.Any(char.IsLower)) maxpoints++;
            if (chars.Any(char.IsUpper)) maxpoints++;
            if (chars.Any(char.IsPunctuation)) maxpoints++;
            int securepoints = 0;
            if (password.Any(char.IsDigit)) securepoints++;
            if (password.Any(char.IsLower)) securepoints++;
            if (password.Any(char.IsUpper)) securepoints++;
            if (password.Any(char.IsPunctuation) || password.Any(char.IsWhiteSpace)) securepoints++;
            if (password.Length >= length) securepoints++;
            if (securepoints + 1 < Math.Min(maxpoints, length)) return false; // Skip Request if not necessary
            if (!disableLeakedPasswords)
            {
                bool? r = await IsPwned(password);
                if (r != true) securepoints++;
            }
            return securepoints == Math.Min(maxpoints, length);
        }

        private static async Task<bool> IsSecure2(string password)
        {
            bool?[] res = EvaluatePassword(password);
            for(int i = 0; i < res.Length; i++)
            {
                if (res[i] == false) return false;
            }

            if(!AppSettings.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords))
            {
                if (await IsPwned(password) == true) return false;
                else return true;
            }
            else
            {
                return true;
            }
        }

        public static bool?[] EvaluatePassword(string password)
        {
            bool?[] checks = new bool?[5];

            checks[0] = password.Any(char.IsLower);
            checks[1] = password.Any(char.IsUpper);
            checks[2] = password.Length >= AppSettings.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength);
            checks[3] = password.Any(char.IsPunctuation);
            checks[4] = password.Any(char.IsDigit);

            return checks;
        }

        public static async Task<bool?> IsPwned(string password)
        {
            try
            {
                string hash = GetSha1Hash(password);
                string hashPrefix = hash.Substring(0, 5);
                string hashSuffix = hash.Substring(5);

                using (var client = new HttpClient())
                using (var data = await client.GetAsync("https://api.pwnedpasswords.com/range/" + hashPrefix))
                {
                    if (data == null)
                    {
                        return null;
                    }

                    using (var reader = new System.IO.StreamReader(data.Content.ReadAsStream()))
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();

                            if (line == null)
                                continue;

                            // Each line of the returned hash list has the following form "hash:numberOfTimesFound"
                            // The first element is the hash
                            // The second element is the numberOfTimesFound
                            var splitLIne = line.Split(':');

                            var lineHashedSuffix = splitLIne[0];
                            var numberOfTimesPasswordPwned = int.Parse(splitLIne[1]);

                            if (lineHashedSuffix == hashSuffix)
                            {
                                return true;
                            }
                        }

                }

                return false;
            }
            catch
            {
                return null;
            }
        }

        private static string GetSha1Hash(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                string hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashHex;
            }
        }
    }
}
