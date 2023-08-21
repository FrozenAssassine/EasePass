using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Views
{
    public sealed partial class GenPasswordPage : Page
    {
        private int length = 15;

        public GenPasswordPage()
        {
            this.InitializeComponent();
            string password = "";
            length = AppSettings.GetSettingsAsInt(AppSettingsValues.passwordLength, DefaultSettingsValues.PasswordLength);
            string chars = AppSettings.GetSettings(AppSettingsValues.passwordChars, DefaultSettingsValues.PasswordChars);
            Random r = new Random();
            while (!isSecure(chars, length, password))
            {
                if (password.Length > length) 
                    password = "";
                password += chars[r.Next(chars.Length)];
            }
            passwordTB.Text = password;

            info.Text =
                "Contains:" + Environment.NewLine +
                (chars.Any(char.IsDigit) ? "- digits" + Environment.NewLine : "") +
                (chars.Any(char.IsLower) ? "- lowercase characters" + Environment.NewLine : "") +
                (chars.Any(char.IsUpper) ? "- uppercase characters" + Environment.NewLine : "") +
                (chars.Any(char.IsPunctuation) ? "- punctuation" + Environment.NewLine : "") + Environment.NewLine +
                "Length: " + password.Length + Environment.NewLine + Environment.NewLine+
                "This password is not known from leaks.";
        }

        public string GetPassword()
        {
            return passwordTB.Text;
        }

        private bool isSecure(string chars, int length, string password)
        {
            int maxpoints = 2; // 2 because of length and pwned passwords
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
            if (!isPwned(password)) securepoints++;
            return securepoints == Math.Min(maxpoints, length);
        }

        private bool isPwned(string passwort)
        {
            try
            {
                string hash = GetSha1Hash(passwort);
                string hashPrefix = hash.Substring(0, 5);
                string hashSufix = hash.Substring(5);

                using (var client = new WebClient())
                using (var data = client.OpenRead("https://api.pwnedpasswords.com/range/" + hashPrefix))
                {
                    if (data == null)
                        return false;

                    using (var reader = new StreamReader(data))
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

                            if (lineHashedSuffix == hashSufix)
                                return true;
                        }

                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private string GetSha1Hash(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                string hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashHex;
            }
        }

        /*private string GetSha1Hash(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var stringBuilder = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    // can be "x2" if you want lowercase
                    stringBuilder.Append(b.ToString("X2"));
                }

                return stringBuilder.ToString();
            }
        }*/
    }
}
