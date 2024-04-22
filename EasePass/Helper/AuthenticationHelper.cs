namespace EasePass.Helper
{
    internal class AuthenticationHelper
    {
        public static bool VerifyPassword(string hash, string password)
        {
            if (hash.Length == 0)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
