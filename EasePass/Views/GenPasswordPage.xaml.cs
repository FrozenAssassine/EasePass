using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
            while (!isSecure(password))
            {
                if (password.Length > length) password = "";
                password += chars[r.Next(chars.Length)];
            }
            passwordTB.Text = password;

            info.Text =
                "Contains:" + Environment.NewLine +
                (chars.Any(char.IsDigit) ? "- digits" + Environment.NewLine : "") +
                (chars.Any(char.IsLower) ? "- lowercase characters" + Environment.NewLine : "") +
                (chars.Any(char.IsUpper) ? "- uppercase characters" + Environment.NewLine : "") +
                (chars.Any(char.IsPunctuation) ? "- punctuation" + Environment.NewLine : "") + Environment.NewLine +
                "Length: " + password.Length;
        }

        public string GetPassword()
        {
            return passwordTB.Text;
        }

        private bool isSecure(string password)
        {
            int securepoints = 0;
            if (password.Any(char.IsDigit)) securepoints++;
            if (password.Any(char.IsLower)) securepoints++;
            if (password.Any(char.IsUpper)) securepoints++;
            if (password.Any(char.IsPunctuation) || password.Any(char.IsWhiteSpace)) securepoints++;
            if (password.Length >= length) securepoints++;
            //if (!isPwned(password)) securepoints++;
            return securepoints == 5; //6;
        }

        // Fix "GetSha1Hash" for checking password leaks online

        /*private bool isPwned(string passwort)
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
