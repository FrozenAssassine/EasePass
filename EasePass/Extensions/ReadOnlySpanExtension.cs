using System;

namespace EasePass.Extensions
{
    internal static class ReadOnlySpanExtension
    {
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
    }
}
