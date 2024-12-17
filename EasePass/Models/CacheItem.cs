using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EasePass.Models
{
    internal class CacheItem
    {
        private string Category = "";
        private string Id = "";

        private CacheItem() { }

        public static CacheItem FindInCache(string category, string id)
        {
            CacheItem item = new CacheItem();
            item.Category = category;
            item.Id = CalculateHash(id);
            if (File.Exists(item.GetPath()))
                return item;
            return null;
        }

        public static CacheItem Create(string category, string id, bool reserve = false)
        {
            CacheItem item = new CacheItem();
            item.Category = category;
            item.Id = CalculateHash(id);
            if (File.Exists(item.GetPath()))
                return null;
            if(!Directory.Exists(GetCacheFolder()))
                Directory.CreateDirectory(GetCacheFolder());
            if(!Directory.Exists(GetCacheFolder() + category))
                Directory.CreateDirectory(GetCacheFolder() + category);
            if (reserve)
                File.WriteAllText(item.GetPath(), "");
            return item;
        }

        // https://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
        private static string CalculateHash(string read)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return Convert.ToString(hashedValue);
        }

        public string GetPath()
        {
            return GetCacheFolder() + Category + "\\" + Id + ".cache";
        }

        private static string GetCacheFolder()
        {
            return ApplicationData.Current.LocalFolder.Path + "\\cache\\";
        }

        public long GetCacheSize()
        {
            return new FileInfo(GetPath()).Length;
        }

        public void Remove()
        {
            if(File.Exists(GetPath()))
                File.Delete(GetPath());
        }
    }
}
