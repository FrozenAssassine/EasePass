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
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace EasePass.Settings
{
    internal class SettingsManager
    {
        private static Dictionary<string, EventHandler<string>> events = new Dictionary<string, EventHandler<string>>();
        private static Dictionary<string, object> localSettingsCache = new Dictionary<string, object>();
        private static bool? isAppPackaged = null;

        //required for testing, since appsettings only work in packaged app
        private static bool IsAppPackaged
        {
            get
            {
                if (isAppPackaged == null)
                {
                    try { var x = ApplicationData.Current.LocalSettings; isAppPackaged = true; }
                    catch { isAppPackaged = false; }
                }
                return isAppPackaged.Value;
            }
        }

        public static void RegisterSettingsChangedEvent(string Value, EventHandler<string> eventHandler)
        {
            if (events.ContainsKey(Value)) events[Value] += eventHandler;
            else events.Add(Value, eventHandler);
        }

        public static void SaveSettings(string Value, object data)
        {
            if (data == null)
                return;

            //cancel if data is a type
            if (data.ToString() == data.GetType().Name)
                return;

            if (IsAppPackaged)
                ApplicationData.Current.LocalSettings.Values[Value] = data.ToString();
            else
                localSettingsCache[Value] = data.ToString();

            if (events.TryGetValue(Value, out EventHandler<string> value) && value != null) value(null, Value);
        }
        public static string GetSettings(string value, string defaultValue = "")
        {
            object val = null;
            if (IsAppPackaged)
                val = ApplicationData.Current.LocalSettings.Values[value];
            else
                localSettingsCache.TryGetValue(value, out val);

            return val as string ?? defaultValue;
        }
        public static bool DeleteSettings(string value)
        {
            bool result = false;
            if (IsAppPackaged)
                result = ApplicationData.Current.LocalSettings.Values.Remove(value);
            else
                result = localSettingsCache.Remove(value);
            if (events.TryGetValue(value, out EventHandler<string> eventHandler) && eventHandler != null) eventHandler(null, value);
            return result;
        }
        public static bool DeleteSettingsStartsWith(string prefix)
        {
            List<string> keysToDelete = new List<string>();
            if (IsAppPackaged)
            {
                foreach (var key in ApplicationData.Current.LocalSettings.Values.Keys)
                {
                    if (key.StartsWith(prefix))
                        keysToDelete.Add(key);
                }
                foreach (var key in keysToDelete)
                {
                    ApplicationData.Current.LocalSettings.Values.Remove(key);
                    if (events.TryGetValue(key, out EventHandler<string> eventHandler) && eventHandler != null) eventHandler(null, key);
                }
            }
            else
            {
                foreach (var key in localSettingsCache.Keys)
                {
                    if (key.StartsWith(prefix))
                        keysToDelete.Add(key);
                }
                foreach (var key in keysToDelete)
                {
                    localSettingsCache.Remove(key);
                    if (events.TryGetValue(key, out EventHandler<string> eventHandler) && eventHandler != null) eventHandler(null, key);
                }
            }
            return keysToDelete.Count > 0;
        }
        public static int GetSettingsAsInt(string value, int defaultValue = 0)
        {
            return ConvertHelper.ToInt(GetSettings(value, null), defaultValue);
        }
        public static bool GetSettingsAsBool(string value, bool defaultValue = false)
        {
            return ConvertHelper.ToBoolean(GetSettings(value, null), defaultValue);
        }
    }
}
