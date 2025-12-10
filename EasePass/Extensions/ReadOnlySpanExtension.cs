using System;
using System.Security;

namespace EasePass.Extensions
{
    /// <summary>
    /// Includes all Extension Methods for the <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    internal static class ReadOnlySpanExtension
    {
        #region Convert
        /// <summary>
        /// Converts the given <paramref name="value"/> to a <see cref="byte"/>[]
        /// </summary>
        /// <param name="value">The Value, which should be converted</param>
        /// <returns>Returns the <paramref name="value"/> as <see cref="byte"/>[]</returns>
        public static byte[] ConvertToBytes(this ReadOnlySpan<char> value)
        {
            byte[] bytes = new byte[value.Length];
            
            for (int i = 0; i < value.Length; i++)
            {
                bytes[i] = (byte)value[i];
            }
            return bytes;
        }
        /// <summary>
        /// Converts the given <paramref name="value"/> to a <see cref="byte"/>[]
        /// </summary>
        /// <param name="value">The Value, which should be converted</param>
        /// <returns>Returns the <paramref name="value"/> as <see cref="byte"/>[]</returns>
        public static SecureString ConvertToSecureString(this ReadOnlySpan<char> value)
        {
            SecureString secureString = new SecureString();

            for (int i = 0; value.Length > i; i++)
            {
                secureString.AppendChar(value[i]);
            }
            secureString.MakeReadOnly();
            return secureString;
        }
        #endregion

        #region Count
        /// <summary>
        /// Counts every occurence of the given <paramref name="c"/> in the <paramref name="span"/>
        /// </summary>
        /// <param name="span">The Span which chars will be checked</param>
        /// <param name="c">The Character which should be counted</param>
        /// <returns>Returns the amount of <paramref name="c"/> occurences in <paramref name="span"/></returns>
        public static int Count(this ReadOnlySpan<byte> span, char c)
        {
            int count = 0;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == c)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion
    }
}