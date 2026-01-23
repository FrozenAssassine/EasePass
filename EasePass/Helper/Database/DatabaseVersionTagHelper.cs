using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Helper.Database
{
    internal static class DatabaseVersionTagHelper
    {
        const string ident = "epdbversion";

        public static byte[] AddVersionTag(byte[] data, int version)
        {
            byte[] identbytes = Encoding.UTF8.GetBytes(ident);
            byte[] v = BitConverter.GetBytes(version);
            byte[] res = new byte[identbytes.Length + data.Length + v.Length];
            for (int i = 0; i < identbytes.Length; i++)
                res[i] = identbytes[i];
            for (int i = identbytes.Length; i < identbytes.Length + v.Length; i++)
                res[i] = v[i - identbytes.Length];
            for (int i = identbytes.Length + v.Length; i < res.Length; i++)
                res[i] = data[i - identbytes.Length - v.Length];
            return res;
        }

        public static (byte[] data, int version) GetVersionTag(byte[] dataWithVersiontag)
        {
            byte[] identbytes = Encoding.UTF8.GetBytes(ident);
            if (dataWithVersiontag.Length < identbytes.Length)
                return (dataWithVersiontag, -1);
            for (int i = 0; i < identbytes.Length; i++)
                if (dataWithVersiontag[i] != identbytes[i])
                    return (dataWithVersiontag, -1);
            int v = BitConverter.ToInt32(dataWithVersiontag, identbytes.Length);
            int vLength = BitConverter.GetBytes(v).Length;
            byte[] data = new byte[dataWithVersiontag.Length - identbytes.Length - vLength];
            for (int i = identbytes.Length + vLength; i < dataWithVersiontag.Length; i++)
                data[i - identbytes.Length - vLength] = dataWithVersiontag[i];
            return (data, v);
        }
    }
}
