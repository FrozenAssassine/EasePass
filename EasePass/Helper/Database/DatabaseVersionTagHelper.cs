using System;
using System.Text;

namespace EasePass.Helper.Database;

public enum DatabaseVersionTag
{
    /// <summary>
    /// No Version Tag found
    /// </summary>
    Undefined = -1,

    /// <summary>
    /// The Core.Database.Format.epeb.MainDatabaseLoader
    /// </summary>
    EPEB = 1,

    /// <summary>
    /// The Core.Database.Format.epdb.v1.DatabaseLoader
    /// </summary>
    EpdbV1DbVersion = 2,

    /// <summary>
    /// The Core.Database.Format.epdb.MainDatabaseLoader class
    /// </summary>
    EpdbV2DbVersion = 3,
    
}

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

    public static (byte[] data, DatabaseVersionTag versionTag) GetVersionTag(byte[] dataWithVersiontag)
    {
        byte[] identbytes = Encoding.UTF8.GetBytes(ident);
        if (dataWithVersiontag.Length < identbytes.Length)
            return (dataWithVersiontag, DatabaseVersionTag.Undefined);
        for (int i = 0; i < identbytes.Length; i++)
            if (dataWithVersiontag[i] != identbytes[i])
                return (dataWithVersiontag, DatabaseVersionTag.Undefined);
        int v = BitConverter.ToInt32(dataWithVersiontag, identbytes.Length);
        int vLength = BitConverter.GetBytes(v).Length;
        byte[] data = new byte[dataWithVersiontag.Length - identbytes.Length - vLength];
        for (int i = identbytes.Length + vLength; i < dataWithVersiontag.Length; i++)
            data[i - identbytes.Length - vLength] = dataWithVersiontag[i];

        return (data, (DatabaseVersionTag)v);
    }
}
