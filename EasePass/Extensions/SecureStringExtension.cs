using System.Runtime.InteropServices;
using System.Security;

namespace EasePass.Extensions
{
    /// <summary>
    /// Includes every Extension for the <see cref="SecureString"/>
    /// </summary>
    internal static class SecureStringExtension
    {
        /// <summary>
        /// Converts the given <paramref name="secureString"/> to a <see cref="byte"/>[]
        /// </summary>
        /// <param name="secureString">The <see cref="SecureString"/>, which should be converted</param>
        /// <returns>Returns the <paramref name="secureString"/> as <see cref="byte"/>[]</returns>
        public static byte[] ToBytes(this SecureString secureString)
        {
            nint pUnicodeBytes = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            try
            {
                byte[] unicodeBytes = new byte[secureString.Length * 2];
                byte[] bytes = new byte[unicodeBytes.Length];

                for (int idx = 0; idx < unicodeBytes.Length; ++idx)
                {
                    bytes[idx] = Marshal.ReadByte(pUnicodeBytes, idx);
                }
                return bytes;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(pUnicodeBytes);
            }
        }
    }
}