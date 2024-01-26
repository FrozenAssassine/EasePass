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

            if (events.ContainsKey(Value) && events[Value] != null) events[Value](null, Value);
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
