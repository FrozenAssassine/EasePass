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

using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Models
{
    public partial class PasswordManagerItem : ObservableObject
    {
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _notes;
        [ObservableProperty] private string _secret;
        [ObservableProperty] private string[] _tags;
        public string Digits { get; set; } = "6";
        public string Interval { get; set; } = "30";
        public string Algorithm { get; set; } = "SHA1";
        public List<string> Clicks { get; } = new List<string>();

        [ObservableProperty]
        private string _displayName;

        // Automatically called when DisplayName changes
        partial void OnDisplayNameChanged(string value)
        {
            FirstChar = string.IsNullOrEmpty(value) ? "" : value.Substring(0, 1);
            OnPropertyChanged(nameof(FirstChar));
            OnPropertyChanged(nameof(BackColor));
            OnPropertyChanged(nameof(ForeColor));
        }

        [JsonIgnore]
        private string _website = "";
        public string Website
        {
            get => _website;
            set
            {
                // Logic check: don't re-run if the value hasn't actually changed
                var normalized = value?.Trim() ?? "";
                if (_website == normalized) return;

                if (AppSettings.ShowIcons)
                {
                    _website = WebsiteIconHelper.NormalizeWebsite(normalized);
                    // Fire and forget the icon update
                    _ = UpdateWebsiteIconAsync();
                }
                else
                {
                    _website = normalized;
                    Icon = null;
                }

                SetProperty(ref _website, value, nameof(Website));
                OnPropertyChanged(nameof(Icon));
            }
        }

        [JsonIgnore][ObservableProperty] private Bitmap? _icon = null;

        [JsonIgnore] public SolidColorBrush BackColor => DisplayName.HashToSolidColorBrush();
        [JsonIgnore] public SolidColorBrush ForeColor => BackColor.MakeFittedTextColor();
        [JsonIgnore] public string FirstChar { get; private set; }
        [JsonIgnore] public bool ShowIcon => AppSettings.ShowIcons;

        public PasswordManagerItem()
        {
            SettingsManager.RegisterSettingsChangedEvent(AppSettingsValues.showIcons, (o, setting) =>
            {
                Website = _website;
                OnPropertyChanged(nameof(ShowIcon));
            });
        }

        private async Task UpdateWebsiteIconAsync()
        {
            if (!ShowIcon || string.IsNullOrEmpty(_website))
            {
                Icon = null;
                return;
            }

            //Icon = await WebsiteIconHelper.GetOrDownloadIconAsync(_website);
        }

        public static ObservableCollection<PasswordManagerItem>? DeserializeItems(string json)
        {
            try { return System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<PasswordManagerItem>>(json); }
            catch { return null; }
        }

        public static string SerializeItems(ObservableCollection<PasswordManagerItem> items)
        {
            try { return JsonConvert.SerializeObject(items, Formatting.Indented); }
            catch { return string.Empty; }
        }
    }
}