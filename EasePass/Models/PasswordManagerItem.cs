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
                FirstChar = value?.Length == 0 ? "" : value.Substring(0, 1);
                NotifyPropertyChanged("DisplayName");
                NotifyPropertyChanged("Website");
                NotifyPropertyChanged("FirstChar");
            }
        }
        [JsonIgnore]
        private string _Website = "";
        public string Website
        {
            get => _Website;
            set
            {
                _Website = value;
                if (string.IsNullOrEmpty(_Website))
                {
                    Icon = null;
                    NotifyPropertyChanged("Icon");
                    return;
                }

                if (Website.ToLower().StartsWith("http"))
                    _Website = "http://" + _Website;

                try
                {
                    Icon = new BitmapImage(new Uri(_Website + "/favicon.ico"));
                    Icon.ImageFailed += (object sender, ExceptionRoutedEventArgs e) => { Icon = null; };
                }
                catch { /*Invalid URI format*/ }

                NotifyPropertyChanged("Icon");
            }
        }
        
        [JsonIgnore]
        public BitmapImage Icon = null;
        [JsonIgnore]
        public Brush BackColor
        {
            get
            {
                MD5 md5Hasher = MD5.Create();
                byte[] bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(DisplayName));
                return new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));
            }
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
        public string FirstChar = "";
        [JsonIgnore]
        public bool ShowIcon => AppSettings.GetSettingsAsBool(AppSettingsValues.showIcons, DefaultSettingsValues.showIcons);

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
