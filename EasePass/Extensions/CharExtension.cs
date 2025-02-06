namespace EasePass.Extensions
{
    /// <summary>
    /// Includes every Extension for the <see cref="char"/>
    /// </summary>
    internal static class CharExtension
    {
        #region IsSpecialChar
        /// <summary>
        /// Checks if the given <paramref name="c"/> is a Special Character
        /// </summary>
        /// <param name="c">The Character, which will be checked</param>
        /// <returns>Returns <see langword="true"/> if the given <paramref name="c"/> is a Special Character</returns>
        public static bool IsSpecialChar(this char c)
        {
            switch (c)
            {
                case '!':
                case '"':
                case '§':
                case '$':
                case '%':
                case '&':
                case '/':
                case '{':
                case '(':
                case '[':
                case ')':
                case ']':
                case '=':
                case '}':
                case '?':
                case '\\':
                case '`':
                case '´':
                case '*':
                case '+':
                case '~':
                case '³':
                case '\'':
                case '#':
                case '-':
                case '_':
                case '.':
                case ':':
                case ',':
                case ';':
                case '<':
                case '>':
                case '|':
                case '²':
                    return true;
                
                default:
                    return false;
            }
        }
        #endregion
    }
}