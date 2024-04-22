using System.Security;

namespace EasePass.Extensions;

public static class SecureStringExtension
{
    public static SecureString ConvertToSecureString(this string plainString)
    {
        SecureString secureString = new SecureString();

        foreach (char c in plainString)
        {
            secureString.AppendChar(c);
        }
        secureString.MakeReadOnly();
        return secureString;
    }
}
