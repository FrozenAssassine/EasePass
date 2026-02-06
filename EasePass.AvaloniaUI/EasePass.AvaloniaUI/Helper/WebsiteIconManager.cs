using Avalonia.Media.Imaging;
using EasePass.Models;
using System;
using System.Threading.Tasks;

namespace EasePass.Helper;

public class WebsiteIconHelper
{
    private const string IconCache = "icons";

    public static async Task<Bitmap?> DownloadFaviconAsync(string website)
    {
        if (string.IsNullOrWhiteSpace(website))
            return null;

        CacheItem item = CacheItem.Create(IconCache, website);
        if (item == null)
            return null;

        try
        {
            bool success = await RequestsHelper.DownloadFileAsync(
                website.TrimEnd('/') + "/favicon.ico",
                item.GetPath(),
                3000
            );

            if (!success)
            {
                item.Remove();
                return null;
            }

            // favicon too small = invalid
            if (item.GetCacheSize() < 10)
            {
                item.Remove();
                return null;
            }

            var image = new Bitmap(item.GetPath());
            //todo
            //image.ImageFailed += (_, __) =>
            //{
            //    try { item.Remove(); } catch { }
            //};

            return image;
        }
        catch
        {
            try { item.Remove(); } catch { }
            return null;
        }
    }

    public static string NormalizeWebsite(string website)
    {
        if (string.IsNullOrWhiteSpace(website))
            return null;

        website = website.Trim();

        if (!website.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            website = "http://" + website;

        return website;
    }

    public static Bitmap GetIcon(string website)
    {
        if (string.IsNullOrEmpty(website))
            return null;

        try
        {
            CacheItem item = CacheItem.FindInCache(IconCache, website);
            if (item == null)
                return null;

            var image = new Bitmap(item.GetPath());
            //todo: image.ImageFailed += (_, __) => {  };

            return image;
        }
        catch
        {
            return null;
        }
    }

    public static async Task<Bitmap?> GetOrDownloadIconAsync(string website)
    {
        try
        {
            var cached = CacheItem.FindInCache(IconCache, website);
            if (cached != null)
            {
                return new Bitmap(cached.GetPath());
            }
        }
        catch { }

        return await DownloadFaviconAsync(website);
    }
}
