using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Helper.Security.Generator
{
    internal static class TokenHelper
    {
        /// <summary>
        /// Generates a Token with the given <paramref name="length"/>
        /// </summary>
        /// <param name="length">The Length of the Token</param>
        /// <returns>Returns a generated Token</returns>
        public static string Generate(int length)
        {
            if (length <= 0)
            {
                length = 1;
            }

            int maxNumOccurence = length / 3;
            if (maxNumOccurence <= 0)
            {
                maxNumOccurence = 1;
            }

            StringBuilder token = new StringBuilder(length);
            do
            {
                token.Clear();
                for (int i = 0; i < length; i++)
                {
                    token.Append(RandomNumberGenerator.GetInt32(0, 10));
                }
            }
            while (!IsSecure(token, length, maxNumOccurence));

            return token.ToString();
        }

        /// <summary>
        /// Validates the Token if it is Secure
        /// </summary>
        /// <param name="token">The Token, which should be validated</param>
        /// <param name="length">The Length of the Token, which it should have at least</param>
        /// <param name="maxNumOccurence">The Amount of the Maximum occurence of a Number</param>
        /// <returns>Returns <see langword="true"/> if the Token is Secure</returns>
        public static bool IsSecure(StringBuilder token, int length, int maxNumOccurence)
        {
            if (token.Length < length)
                return false;

            int tLength = token.Length;
            Span<char> chars = length < 1025 ? stackalloc char[tLength] : new char[tLength];
            token.CopyTo(0, chars, tLength);

            Dictionary<int, int> counters = new Dictionary<int, int>();
            for (int i = 0; i < tLength; i++)
            {
                if (counters.ContainsKey(token[i]))
                {
                    counters[token[i]]++;
                }
                else
                {
                    counters.Add(token[i], 1);
                }

                if (counters[token[i]] > maxNumOccurence)
                    return false;
            }
            return true;
        }
    }
}