using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace EasePass.Tests.Core
{
    public static class DatabaseTestHelper
    {
        public static SecureString ToSecureString(string str)
        {
            SecureString secureString = new SecureString();
            foreach (char c in str)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }

        public static string ConvertToString(SecureString secureString)
        {
            if (secureString == null) return null;
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        
        public static string GetTempDatabasePath()
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".epdb");
        }
    }
}
