using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;

namespace EasePass.Models
{
    public class PasswordManagerItem : INotifyPropertyChanged
    {
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string Secret { get; set; }
        public string Digits { get; set; } = "6";
        public string Interval { get; set; } = "30";
        public string Algorithm { get; set; } = "SHA1";
        private string _DisplayName;
        public string DisplayName
        {
            get => _DisplayName;
            set
            {
                _DisplayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }
        [JsonIgnore]
        private string _Website { get; set; }
        public string Website
        {
            get => _Website;
            set
            {
                _Website = value;
                if (!AppSettings.GetSettingsAsBool(AppSettingsValues.showIcons, DefaultSettingsValues.showIcons)) return;
                if (!string.IsNullOrEmpty(WebsiteFix))
                {
                    try
                    {
                        Icon = new BitmapImage(new Uri(WebsiteFix + "/favicon.ico"));
                        Icon.ImageFailed += Icon_ImageFailed;
                        return;
                    }
                    catch { }
                }
                Icon = null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;



        // Probably wrong place for this code but needed for showing the icon in the ListBox.
        [JsonIgnore]
        public string WebsiteFix
        {
            get
            {
                if (string.IsNullOrEmpty(Website)) return "";
                if (Website.ToLower().StartsWith("http")) return Website; else return "http://" + Website;
            }
        }

        [JsonIgnore]
        public BitmapImage Icon = null;

        private void Icon_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Icon = null;
            NotifyPropertyChanged("Icon");
            NotifyPropertyChanged("ImgVisibility");
            NotifyPropertyChanged("CharVisibility");
        }

        [JsonIgnore]
        public Visibility ImgVisibility
        {
            get
            {
                return AppSettings.GetSettingsAsBool(AppSettingsValues.showIcons, DefaultSettingsValues.showIcons) && Icon != null ? Visibility.Visible : Visibility.Collapsed;
            }
            set { }
        }

        [JsonIgnore]
        public Visibility CharVisibility
        {
            get
            {
                return ImgVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
            set { }
        }

        [JsonIgnore]
        public Brush BackColor
        {
            get
            {
                MD5 md5Hasher = MD5.Create();
                byte[] bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(DisplayName));
                return new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));
            }
            set { }
        }

        [JsonIgnore]
        public Brush ForeColor
        {
            get
            {
                SolidColorBrush c = BackColor as SolidColorBrush;
                byte average = (byte)((c.Color.R + c.Color.G + c.Color.B) / 3);
                return average > 127 ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)) : new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
        }

        [JsonIgnore]
        public string FirstChar
        {
            get
            {
                return DisplayName.Substring(0, 1);
            }
            set { }
        }

        [JsonIgnore]
        public Visibility ContainsTOTP
        {
            get
            {
                return ConvertHelper.BoolToVisibility(!string.IsNullOrEmpty(Secret));
            }
            set { }
        }
    }
}
