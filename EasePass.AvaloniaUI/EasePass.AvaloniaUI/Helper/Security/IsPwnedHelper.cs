using EasePass.AvaloniaUI.Settings;
using System;
using System.IO;

namespace EasePass.Helper.Security
{
    internal class IsPwnedHelper
    {
        private static string filePath = Path.Combine(ApplicationData.InstalledLocation, "Assets", "pwned.txt");

        private static PwnedResult ContainsString(string filePath, string password)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch
            {
                //could not read file:
                return PwnedResult.Error;
            }

            for(int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Equals(password, StringComparison.OrdinalIgnoreCase))
                {
                    return PwnedResult.Leaked;
                }
            }
            return PwnedResult.NotLeaked;
        }

        public static PwnedResult IsPwned(string password)
        {
            return ContainsString(filePath, password);
        }
    }

    public enum PwnedResult
    {
        Error, Leaked, NotLeaked
    }
}
