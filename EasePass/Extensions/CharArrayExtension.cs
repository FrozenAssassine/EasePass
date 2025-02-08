namespace EasePass.Extensions
{
    /// <summary>
    /// Includes every Extension for the <see cref="char"/>[]
    /// </summary>
    internal static class CharArrayExtension
    {
        #region ZeroOut
        /// <summary>
        /// Zeros out the given <paramref name="chars"/>
        /// </summary>
        /// <param name="chars">The <see cref="char"/>[], which should be zero out</param>
        /// <returns>Returns the Zero out Array</returns>
        public static char[] ZeroOut(this char[] chars)
        {
            int length = chars.Length;
            for (int i = 0; i < length; i++)
            {
                chars[i] = '\0';
            }
            return chars;
        }
        #endregion
    }
}
