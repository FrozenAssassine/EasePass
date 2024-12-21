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

using EasePass.Models;
using EasePass.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasePass.Helper
{
    public static class PasswordHelper
    {
        private static bool disableLeakedPasswords = false;

        private const string ABC = "abcdefghijklmnopqrstuvwxyz";
        private const int StringMinRepeat = 5;
        private const int IntegerMinRepeat = 4;

        private static List<CommonPasswordSequence> CommonSequences = new List<CommonPasswordSequence>()
        {
            new CommonPasswordSequence("012345678909876543210", IntegerMinRepeat), // forward and backward
            new CommonPasswordSequence(ABC + ABC, StringMinRepeat), // double abc
            new CommonPasswordSequence("qwertyuiopasdfghjklzxcvbnm", StringMinRepeat), // English keyboard layout
            new CommonPasswordSequence("qwertzuiopasdfghjklyxcvbnm", StringMinRepeat), // German keyboard layout
            new CommonPasswordSequence("hello"), // the following words are based on some stats from Wikipedia
            new CommonPasswordSequence("test"),
            new CommonPasswordSequence("password"),
            new CommonPasswordSequence("service"),
            new CommonPasswordSequence("monkey"),
            new CommonPasswordSequence("letmein"),
            new CommonPasswordSequence("football"),
            new CommonPasswordSequence("baseball"),
            new CommonPasswordSequence("princess"),
            new CommonPasswordSequence("sunshine"),
            new CommonPasswordSequence("iloveyou"),
            new CommonPasswordSequence("admin"),
            new CommonPasswordSequence("starwars"),
            new CommonPasswordSequence("master"),
            new CommonPasswordSequence("lovely"),
            new CommonPasswordSequence("welcome"),
            new CommonPasswordSequence("dragon"),
            new CommonPasswordSequence("superman"),
        };

        public static void Init()
        {
            string[] username = Environment.UserName.Split(" "); // User should not use his username in passwords

            for (int i = 0; i < username.Length; i++)
            {
                if (!int.TryParse(username[i], out int value)) 
                    CommonSequences.Add(new CommonPasswordSequence(username[i].ToLower()));
            }

            for (int i = 0; i < ABC.Length; i++) // repeating character, i.e. 'aaaaaaa'
            {
                string repeated = new string(ABC[i], StringMinRepeat);
                CommonSequences.Add(new CommonPasswordSequence(repeated, StringMinRepeat));
            }

            for (int i = 0; i < 10; i++) // repeating number, i.e. '5555555'
            {
                string repeated = new string(i.ToString()[0], StringMinRepeat);
                CommonSequences.Add(new CommonPasswordSequence(repeated, IntegerMinRepeat));
            }
        }

        public static async Task<string> GeneratePassword()
        {
            disableLeakedPasswords = AppSettings.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords);
            int length = AppSettings.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength);
            string chars = AppSettings.GetSettings(AppSettingsValues.passwordChars, DefaultSettingsValues.PasswordChars);         
            Random r = new Random();

            StringBuilder password = new StringBuilder();
            while (!await IsSecure(chars, length, password.ToString()))
            {
                if (password.Length > length)
                    password.Clear();

                password.Append(chars[r.Next(chars.Length)]);
            }
            return password.ToString();
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

        public static bool[] EvaluatePassword(string password)
        {
            bool[] checks = new bool[6];

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
            int length = CommonSequences.Count;
            for (int i = 0; i < length; i++)
            {
                if (CommonSequences[i].ContainsSequence(password)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the Password has been leaked already
        /// </summary>
        /// <param name="password">The Password which will be checked</param>
        /// <returns>Returns <see langword="true"/> if the Password has been leaked</returns>
        public static async Task<bool?> IsPwned(string password)
        {
            try
            {
                string hash = GetSha1Hash(password);
                string hashPrefix = hash.Substring(0, 5);
                string hashSuffix = hash.Substring(5);

                using (var client = new HttpClient())
                // Checks if the Password has been leaked
                using (var data = await client.GetAsync("https://api.pwnedpasswords.com/range/" + hashPrefix))
                {
                    if (data == null)
                    {
                        return null;
                    }
                    return ReadIsPwnedData(data, hashSuffix);
                }
            }
            catch
            {
                return null;
            }
        }
        private static bool ReadIsPwnedData(HttpResponseMessage data, string hashSuffix)
        {
            using (var reader = new System.IO.StreamReader(data.Content.ReadAsStream()))
            {
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
