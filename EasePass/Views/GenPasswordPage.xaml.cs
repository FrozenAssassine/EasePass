using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public sealed partial class GenPasswordPage : Page
    {
        public GenPasswordPage()
        {
            this.InitializeComponent();
        }

        public async void GeneratePassword()
        {
            await _GeneratePassword();
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public async Task _GeneratePassword()
        {
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

            passwordTB.Text = password;

            info.Text =
                "Contains:" + Environment.NewLine +
                (chars.Any(char.IsDigit) ? "- digits" + Environment.NewLine : "") +
                (chars.Any(char.IsLower) ? "- lowercase characters" + Environment.NewLine : "") +
                (chars.Any(char.IsUpper) ? "- uppercase characters" + Environment.NewLine : "") +
                (chars.Any(char.IsPunctuation) ? "- punctuation" + Environment.NewLine : "") + Environment.NewLine +
                "Length: " + password.Length + Environment.NewLine + Environment.NewLine +
                "This password is not known from leaks.";
        }

        public string GetPassword()
        {
            return passwordTB.Text;
        }

        private async Task<bool> IsSecure(string chars, int length, string password)
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
            if (!await IsPwned(password)) securepoints++;
            return securepoints == Math.Min(maxpoints, length);
        }

        private async Task<bool> IsPwned(string passwort)
        {
            try
            {
                string hash = GetSha1Hash(passwort);
                string hashPrefix = hash.Substring(0, 5);
                string hashSuffix = hash.Substring(5);

                using (var client = new HttpClient())
                using (var data = await client.GetAsync("https://api.pwnedpasswords.com/range/" + hashPrefix))
                {
                    if (data == null)
                        return false;

                    using (var reader = new StreamReader(data.Content.ReadAsStream()))
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
    }
}
