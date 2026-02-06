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

using EasePass.Settings;
using EasePass.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasePass.Settings
{
    internal static class SettingsManager
    {
        private static readonly string SettingsFile = Path.Combine(ApplicationData.LocalFolder, "appsettings.json");
        private static Dictionary<string, string> _settingsCache = new();
        private static readonly Dictionary<string, EventHandler<string>> _events = new();

        static SettingsManager()
        {
            LoadSettings();
        }

        private static void LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFile);
                    _settingsCache = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                                     ?? new Dictionary<string, string>();
                }
                catch
                {
                    _settingsCache = new Dictionary<string, string>();
                }
            }
        }

        private static void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settingsCache, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch
            {
                // ignore errors
            }
        }

        public static void RegisterSettingsChangedEvent(string key, EventHandler<string> handler)
        {
            if (_events.ContainsKey(key)) _events[key] += handler;
            else _events.Add(key, handler);
        }

        public static void SaveSettings(string key, object data)
        {
            if (data == null) return;
            if (data.ToString() == data.GetType().Name) return;

            _settingsCache[key] = data.ToString();
            SaveToFile();

            if (_events.TryGetValue(key, out var handler)) handler?.Invoke(null, key);
        }

        public static string GetSettings(string key, string defaultValue = "")
        {
            return _settingsCache.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static bool DeleteSettings(string key)
        {
            if (_settingsCache.Remove(key))
            {
                SaveToFile();
                if (_events.TryGetValue(key, out var handler)) handler?.Invoke(null, key);
                return true;
            }
            return false;
        }

        public static bool DeleteSettingsStartsWith(string prefix)
        {
            var keysToDelete = new List<string>();
            foreach (var key in _settingsCache.Keys)
                if (key.StartsWith(prefix)) keysToDelete.Add(key);

            foreach (var key in keysToDelete)
            {
                _settingsCache.Remove(key);
                if (_events.TryGetValue(key, out var handler)) handler?.Invoke(null, key);
            }

            if (keysToDelete.Count > 0) SaveToFile();
            return keysToDelete.Count > 0;
        }

        public static int GetSettingsAsInt(string key, int defaultValue = 0)
        {
            return int.TryParse(GetSettings(key, null), out var result) ? result : defaultValue;
        }

        public static bool GetSettingsAsBool(string key, bool defaultValue = false)
        {
            return bool.TryParse(GetSettings(key, null), out var result) ? result : defaultValue;
        }
    }
}
