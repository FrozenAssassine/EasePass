using Avalonia.Media;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Extensions;

public static class BrushExtension
{
    public static SolidColorBrush HashToSolidColorBrush(this string str, byte alpha = 255)
    {
        str ??= "";
        using MD5 md5Hasher = MD5.Create();
        byte[] bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
        return new SolidColorBrush(Color.FromArgb(alpha, bytes[0], bytes[1], bytes[2]));
    }

    public static SolidColorBrush MakeFittedTextColor(this SolidColorBrush c)
    {
        if (c == null) return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        
        byte average = (byte)((c.Color.R + c.Color.G + c.Color.B) / 3);
        return average > 127 ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)) : new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
    }
}
