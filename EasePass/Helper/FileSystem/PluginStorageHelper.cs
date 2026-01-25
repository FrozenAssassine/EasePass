using System;
using System.IO;
using System.Text;
using EasePass.Settings;
using EasePassExtensibility;
using Windows.Storage;

namespace EasePass.Helper.FileSystem
{
    internal static class PluginStorageHelper
    {
        const string AppsettingsPrefix = "pluginstorage_";
        private static readonly string path = ApplicationData.Current.LocalFolder.Path + "\\extensions\\";

        public static void Initialize(IStorageInjectable storageInjectable, string pluginID)
        {
            storageInjectable.SaveString = (key, value) => _SaveString(pluginID, key, value);
            storageInjectable.LoadString = (key) => _LoadString(pluginID, key);
            storageInjectable.SaveFile = (filename, data) => _SaveFile(pluginID, filename, data);
            storageInjectable.LoadFile = (filename) => _LoadFile(pluginID, filename);
        }

        private static void _SaveString(string pluginID, string key, string value)
        {
            string newKey = AppsettingsPrefix + pluginID + "_" + Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            SettingsManager.SaveSettings(newKey, value);
        }

        private static string _LoadString(string pluginID, string key)
        {
            string newKey = AppsettingsPrefix + pluginID + "_" + Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            return SettingsManager.GetSettings(newKey, null);
        }

        private static void _SaveFile(string pluginID, string filename, byte[] data)
        {
            if (!Directory.Exists(path + pluginID))
                Directory.CreateDirectory(path + pluginID);
            File.WriteAllBytes(path + pluginID + "\\" + Convert.ToBase64String(Encoding.UTF8.GetBytes(filename)), data);
        }

        private static byte[] _LoadFile(string pluginID, string filename)
        {
            if(!Directory.Exists(path + pluginID))
                return null;
            string p = path + pluginID + "\\" + Convert.ToBase64String(Encoding.UTF8.GetBytes(filename));
            if (!File.Exists(p))
                return null;
            return File.ReadAllBytes(p);
        }

        public static void Clean(string pluginID)
        {
            if(Directory.Exists(path + pluginID))
                Directory.Delete(path + pluginID, true);
            SettingsManager.DeleteSettings(AppsettingsPrefix + pluginID + "_");
        }
    }
}
