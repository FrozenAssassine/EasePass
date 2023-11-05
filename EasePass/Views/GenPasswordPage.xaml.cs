using EasePass.Helper;
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
            safetyChart.EvaluatePassword(passwordTB.Text);
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public async Task _GeneratePassword()
        {
            passwordTB.Text = await PasswordHelper.GeneratePassword();

            //string chars = AppSettings.GetSettings(AppSettingsValues.passwordChars, DefaultSettingsValues.PasswordChars);
            //bool disableLeakedPasswords = AppSettings.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords);
            //info.Text =
            //    "Contains:" + Environment.NewLine +
            //    (chars.Any(char.IsDigit) ? "- digits" + Environment.NewLine : "") +
            //    (chars.Any(char.IsLower) ? "- lowercase characters" + Environment.NewLine : "") +
            //    (chars.Any(char.IsUpper) ? "- uppercase characters" + Environment.NewLine : "") +
            //    (chars.Any(char.IsPunctuation) ? "- punctuation" + Environment.NewLine : "") + Environment.NewLine +
            //    "Length: " + passwordTB.Text.Length + Environment.NewLine + Environment.NewLine +
            //    (disableLeakedPasswords ? "" : "This password is not known from leaks.");
        }

        public string GetPassword()
        {
            return passwordTB.Text;
        }
    }
}
