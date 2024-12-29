namespace EasePass.Extensions
{
    public static class CharExtension
    {
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
    }
}
