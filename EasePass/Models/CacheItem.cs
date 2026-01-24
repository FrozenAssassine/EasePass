using System;
using System.IO;
using Windows.Storage;

namespace EasePass.Models
{
    public class CacheItem
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

        /// <summary>
        /// Gets the Path of the Cache
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return GetCacheFolder() + Category + Path.DirectorySeparatorChar + Id + ".cache";
        }


        private static string GetCacheFolder()
        {
            return ApplicationData.Current.LocalFolder.Path + Path.DirectorySeparatorChar + "cache" + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Gets the disk size of the cache item
        /// Too small cache items are invalid
        /// </summary>
        /// <returns></returns>
        public long GetCacheSize()
        {
            var path = GetPath();
            if (!File.Exists(path))
                return 0;
            return new FileInfo(path).Length;
        }

        /// <summary>
        /// Clears the Cache
        /// </summary>
        public void Remove()
        {
            string path = GetPath();
            if(File.Exists(path))
                File.Delete(path);
        }
    }
}
