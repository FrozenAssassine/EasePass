using System.Collections.Generic;
using System;
using System.Linq;

namespace EasePass.Extensions
{
    internal static class ByteArrayExtension
    {
        private readonly static char[] base64ByteTo = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();

        /// <summary>
        /// Converts the <paramref name="input"/> to a Base64 <see cref="char"/>[]
        /// </summary>
        /// <param name="input">The <see cref="byte"/>[], which should be converted to Base64</param>
        /// <returns>Returns the <paramref name="input"/> as Base64 <see cref="char"/>[]</returns>
        public static char[] ToBase64(this byte[] input)
        {
            // Base64 encodes in blocks of 3 bytes. Each block becomes 4 base64 characters.
            int nChars = (int)Math.Ceiling((double)input.Length / 3) * 4;
            char[] outBase64Chars = new char[nChars];

            int iByte = 0, iBit = 7, iBase64Bits = 0, iBase64Chars = 0;
            byte base64Byte = 0;

            // Walk byte by byte on the input, traversing bits from 7 to 0. Along the way, convert every group of 6 into base64
            int length = input.Length;
            while (iByte < length)
            {
                base64Byte = (byte)(
                    (byte)(base64Byte << 1) |
                    (Convert.ToBoolean(input[iByte] & (byte)(1 << iBit)) ? (byte)1 : (byte)0));

                iBase64Bits++;
                iBit--;
                // If we have gotten 6 base64 bits, convert them into a char.
                if (iBase64Bits % 6 == 0)
                {
                    outBase64Chars[iBase64Chars] = base64ByteTo[base64Byte];
                    base64Byte = 0;
                    iBase64Bits = 0;
                    iBase64Chars++;
                }
                // If we are done with bits in current byte, advance to next byte
                if (iBit < 0)
                {
                    iByte++;
                    iBit = 7;
                }
            }

            // Padding if last block has less than 3 bytes.
            // if block has only 1 byte, give 1 char padding.
            // if block has 2 bytes, give one more char padding
            if (iByte % 3 > 0 || iByte % 3 > 1)
            {
                outBase64Chars[iBase64Chars] = '=';
                iBase64Chars++;
            }
            return outBase64Chars;
        }
    }
}