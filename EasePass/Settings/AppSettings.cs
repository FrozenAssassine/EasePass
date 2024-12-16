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
    internal class AppSettings
    {
        private static Dictionary<string, EventHandler<string>> events = new Dictionary<string, EventHandler<string>>();
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

            ApplicationData.Current.LocalSettings.Values[Value] = data.ToString();

            if (events.TryGetValue(Value, out EventHandler<string> value) && value != null) value(null, Value);
        }
        public static string GetSettings(string value, string defaultValue = "")
        {
            return ApplicationData.Current.LocalSettings.Values[value] as string ?? defaultValue;
        }
        public static int GetSettingsAsInt(string value, int defaultValue = 0)
        {
            return ConvertHelper.ToInt(ApplicationData.Current.LocalSettings.Values[value] as string, defaultValue);
        }
        public static bool GetSettingsAsBool(string value, bool defaultValue = false)
        {
            return ConvertHelper.ToBoolean(ApplicationData.Current.LocalSettings.Values[value] as string, defaultValue);
        }
    }
}
