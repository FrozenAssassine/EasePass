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

using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace EasePass.Models
{
    public class PasswordManagerItem : INotifyPropertyChanged
    {
        private string _Password;
        public string Password { get => _Password; set { _Password = value; NotifyPropertyChanged("Password"); } }
        private string _Username;
        public string Username { get => _Username; set { _Username = value; NotifyPropertyChanged("Username"); } }

        private string _Email;
        public string Email { get => _Email; set { _Email = value; NotifyPropertyChanged("Email"); } }

        private string _Notes;
        public string Notes { get => _Notes; set { _Notes = value; NotifyPropertyChanged("Notes"); } }

        private string _Secret;
        public string Secret { get => _Secret; set { _Secret = value; NotifyPropertyChanged("Secret"); } }
        public string Digits { get; set; } = "6";
        public string Interval { get; set; } = "30";
        public string Algorithm { get; set; } = "SHA1";
        public List<string> Clicks { get; } = new List<string>();
        [JsonIgnore]
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
                const string iconCache = "icons";
                _Website = value.Trim();
                if (!ShowIcon)
                {
                    Icon = null;
                    NotifyPropertyChanged("Icon");
                    NotifyPropertyChanged("Website");
                    return;
                }

                if (string.IsNullOrEmpty(_Website))
                {
                    Icon = null;
                    NotifyPropertyChanged("Icon");
                    return;
                }

                if (!_Website.ToLower().StartsWith("http"))
                    _Website = "http://" + _Website;

                try
                {
                    CacheItem item = CacheItem.FindInCache(iconCache, _Website);
                    if (item != null)
                    {
                        Icon = new BitmapImage(new Uri(item.GetPath()));
                        Icon.ImageFailed += (object sender, ExceptionRoutedEventArgs e) => { Icon = null; NotifyPropertyChanged("Icon"); };

                        NotifyPropertyChanged("Icon");
                        NotifyPropertyChanged("Website");
                        return;
                    }

                    IconDownloadImage(item, iconCache);
                }
                catch { /*Invalid URI format*/ }

                NotifyPropertyChanged("Icon");
                NotifyPropertyChanged("Website");
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
        public bool ShowIcon =>  AppSettings.ShowIcons;

        public event PropertyChangedEventHandler PropertyChanged;

        public PasswordManagerItem()
        {
            SettingsManager.RegisterSettingsChangedEvent(AppSettingsValues.showIcons, (object o, string setting) =>
            {
                Website = _Website;
                NotifyPropertyChanged("Website");
                NotifyPropertyChanged("ShowIcon");
                NotifyPropertyChanged("Icon");
            });
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void IconDownloadImage(CacheItem item, string iconCache)
        {
            item = CacheItem.Create(iconCache, _Website);
            if (item == null)
            {
                Icon = null;

                NotifyPropertyChanged("Icon");
                NotifyPropertyChanged("Website");
                return;
            }

            if (await RequestsHelper.DownloadFileAsync(_Website + "/favicon.ico", item.GetPath(), 30000))
            {
                Icon = new BitmapImage(new Uri(item.GetPath()));
                Icon.ImageFailed += (object sender, ExceptionRoutedEventArgs e) => { Icon = null; NotifyPropertyChanged("Icon"); };
            }
            else
                Icon = null;

            //check for valid item
            if (item.GetCacheSize() < 10)
            {
                item.Remove();
            }
        }
    }
}
