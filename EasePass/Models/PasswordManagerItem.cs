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
using System.Text;
using System.Threading.Tasks;

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
        public string Website { get; set; }
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
        public ImageSource Icon
        {
            get
            {
                if (!string.IsNullOrEmpty(WebsiteFix) && Visibility == Visibility.Visible)
                {
                    try
                    {
                        return new BitmapImage(new Uri(WebsiteFix + "/favicon.ico"));
                    }
                    catch { }
                }
                return null;
            }
        }

        [JsonIgnore]
        public Visibility Visibility
        {
            get
            {
                return AppSettings.GetSettingsAsBool(AppSettingsValues.showIcons, DefaultSettingsValues.showIcons) ? Visibility.Visible : Visibility.Collapsed;
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
