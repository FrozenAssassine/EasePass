using EasePass.Extensions;
using EasePass.Settings;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Helper
{
    internal class EncryptDecryptHelper
    {
        public static byte[] EncryptStringAES(string plainText, string password, string salt)
        {
            SecureString pw = new SecureString();
            for (var i = 0; i < password.Length; i++)
            {
                pw.AppendChar(password[i]);
            }

            return EncryptStringAES(plainText, pw, salt);
        }

        public static string DecryptStringAES(byte[] cipherText, string password, string salt)
        {
            return DecryptStringAES(cipherText, password.ConvertToSecureString(), salt);
        }


        public static byte[] EncryptStringAES(string plainText, SecureString password, string salt = "")
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetCryptionKey(password, salt);
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        public static string DecryptStringAES(byte[] cipherText, SecureString password, string salt = "")
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetCryptionKey(password, salt);
                aesAlg.IV = cipherText.Take(16).ToArray();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText.Skip(16).ToArray()))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private static byte[] ToBytes(SecureString secureString)
        {
            var pUnicodeBytes = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            try
            {
                byte[] unicodeBytes = new byte[secureString.Length * 2];
                byte[] bytes = new byte[unicodeBytes.Length];

                for (var idx = 0; idx < unicodeBytes.Length; ++idx)
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

        public static byte[] DeriveEncryptionKey(SecureString password, byte[] salt, int keySizeInBytes, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(ToBytes(password), salt, iterations))
            {
                return pbkdf2.GetBytes(keySizeInBytes);
            }
        }

        private static byte[] GetCryptionKey(SecureString pw, string salt = "")
        {
            byte[] saltFromDatabase = Encoding.UTF8.GetBytes(salt.Length == 0 ? AppSettings.GetSettings(AppSettingsValues.pSalt) : "");
            int keySizeInBytes = 32;
            int iterations = 10000;

            return DeriveEncryptionKey(pw, saltFromDatabase, keySizeInBytes, iterations);
        }
    }
}
