using EasePass.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private const string ABC = "abcdefghijklmnopqrstuvwxyz";
        private const int StringMinRepeat = 5;
        private const int IntegerMinRepeat = 4;

        private static List<CommonSequence> CommonSequences = new List<CommonSequence>()
        {
            new CommonSequence("012345678909876543210", IntegerMinRepeat), // forward and backward
            new CommonSequence(ABC + ABC, StringMinRepeat), // double abc
            new CommonSequence("qwertyuiopasdfghjklzxcvbnm", StringMinRepeat), // English keyboard layout
            new CommonSequence("qwertzuiopasdfghjklyxcvbnm", StringMinRepeat), // German keyboard layout
            new CommonSequence("hello"), // the following words are based on some stats from Wikipedia
            new CommonSequence("test"),
            new CommonSequence("password"),
            new CommonSequence("service"),
            new CommonSequence("monkey"),
            new CommonSequence("letmein"),
            new CommonSequence("football"),
            new CommonSequence("baseball"),
            new CommonSequence("princess"),
            new CommonSequence("sunshine"),
            new CommonSequence("iloveyou"),
            new CommonSequence("admin"),
            new CommonSequence("starwars"),
            new CommonSequence("master"),
            new CommonSequence("lovely"),
            new CommonSequence("welcome"),
            new CommonSequence("dragon"),
            new CommonSequence("superman"),
        };

        public static void Init()
        {
            string[] username = Environment.UserName.Split(" "); // User should not use his username in passwords
            for(int i = 0; i < username.Length; i++)
            {
                bool isNumber = int.TryParse(username[i], out int value);
                if (!isNumber) CommonSequences.Add(new CommonSequence(username[i].ToLower()));
            }
            for(int i = 0; i < ABC.Length; i++) // repeating character, i.e. 'aaaaaaa'
            {
                StringBuilder sb = new StringBuilder();
                for(int j = 0; j < StringMinRepeat; j++)
                {
                    sb.Append(ABC[i]);
                }
                CommonSequences.Add(new CommonSequence(sb.ToString(), StringMinRepeat));
            }
            for(int i = 0; i < 10; i++) // repeating number, i.e. '5555555'
            {
                StringBuilder sb = new StringBuilder();
                for(int j = 0; j < StringMinRepeat; j++)
                {
                    sb.Append(Convert.ToString(i));
                }
                CommonSequences.Add(new CommonSequence(sb.ToString(), IntegerMinRepeat));
            }
        }

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
            int maxpoints = 2; // 1 because of length + common sequences
            if (chars.Any(char.IsDigit)) maxpoints++;
            if (chars.Any(char.IsLower)) maxpoints++;
            if (chars.Any(char.IsUpper)) maxpoints++;
            if (chars.Any(char.IsPunctuation)) maxpoints++;
            int securepoints = 0;
            if (password.Any(char.IsDigit)) securepoints++;
            if (password.Any(char.IsLower)) securepoints++;
            if (password.Any(char.IsUpper)) securepoints++;
            if (password.Any(char.IsPunctuation)) securepoints++;
            if (password.Length >= length) securepoints++;
            if (!ContainsCommonSequences(password)) securepoints++;
            if (securepoints + 1 < Math.Min(maxpoints, length)) return false; // Skip Request if not necessary
            if (!disableLeakedPasswords)
            {
                maxpoints++;
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
            bool?[] checks = new bool?[6];

            checks[0] = password.Any(char.IsLower);
            checks[1] = password.Any(char.IsUpper);
            checks[2] = password.Length >= AppSettings.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength);
            checks[3] = password.Any(char.IsPunctuation);
            checks[4] = password.Any(char.IsDigit);
            checks[5] = !ContainsCommonSequences(password);

            return checks;
        }

        private static bool ContainsCommonSequences(string password)
        {
            for(int i = 0; i < CommonSequences.Count; i++)
            {
                if (CommonSequences[i].ContainsSequence(password)) return true;
            }
            return false;
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

        private class CommonSequence
        {
            private string sequence = "";
            private int minLength = 5;
            private bool entireSequence = false;

            public CommonSequence(string sequence, int minLength)
            {
                this.sequence = sequence;
                this.minLength = minLength;
                entireSequence = false;
            }

            public CommonSequence(string sequence)
            {
                this.sequence = sequence;
                entireSequence = true;
            }

            public bool ContainsSequence(string str)
            {
                string lower = str.ToLower();
                if (entireSequence || sequence.Length <= minLength) return lower.Contains(sequence);
                for (int i = 0; i < sequence.Length - minLength; i++)
                {
                    string subsequence = sequence.Substring(i, minLength);
                    if (lower.Contains(subsequence)) return true;
                }
                return false;
            }
        }
    }
}
