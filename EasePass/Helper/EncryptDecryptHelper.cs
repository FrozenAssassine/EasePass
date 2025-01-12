/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Extensions;
using EasePass.Settings;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Helper
{
    internal class EncryptDecryptHelper
    {
        #region DecryptStringAES
        public static (string decryptedString, bool correctPassword) DecryptStringAES(byte[] cipherText, string password, string salt)
        {
            return DecryptStringAES(cipherText, password.ConvertToSecureString(), salt);
        }
        public static (string decryptedString, bool correctPassword) DecryptStringAES(byte[] cipherText, SecureString password, string salt = "")
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetCryptionKey(password, salt);
                aesAlg.IV = cipherText.Take(16).ToArray();

                return DecryptInternal(aesAlg, cipherText);
            }
        }
        public static (string decryptedString, bool correctPassword) DecryptStringAES(byte[] cipherText, byte[] password, string salt = "")
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetCryptionKey(password, salt);
                aesAlg.IV = cipherText.Take(16).ToArray();

                return DecryptInternal(aesAlg, cipherText);
            }
        }
        private static (string decryptedString, bool correctPassword) DecryptInternal(Aes aesAlg, byte[] cipherText)
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText.Skip(16).ToArray()))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
            {
                try
                {
                    string reader = srDecrypt.ReadToEnd();
                    return (reader, true);
                }
                catch (CryptographicException)
                {
                    return (null, false);
                }
            }
        }
        #endregion

        #region Encrypt
        public static byte[] EncryptStringAES(string plainText, string password, string salt)
        {
            return EncryptStringAES(plainText, password.ConvertToSecureString(), salt);
        }
        public static byte[] EncryptStringAES(string plainText, SecureString password, string salt = "")
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetCryptionKey(password, salt);
                aesAlg.GenerateIV();

                return EncryptInternal(aesAlg, plainText);
            }
        }
        public static byte[] EncryptStringAES(string plainText, byte[] password, string salt = "")
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetCryptionKey(password, salt);
                aesAlg.GenerateIV();

                return EncryptInternal(aesAlg, plainText);
            }
        }
        private static byte[] EncryptInternal(Aes aesAlg, string plainText)
        {
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                {
                    swEncrypt.Write(plainText);
                }
                return msEncrypt.ToArray();
            }
        }
        #endregion

        #region Derive
        public static byte[] DeriveEncryptionKey(byte[] password, byte[] salt, int keySizeInBytes, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(keySizeInBytes);
            }
        }
        public static byte[] DeriveEncryptionKey(SecureString password, byte[] salt, int keySizeInBytes, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password.ToBytes(), salt, iterations))
            {
                return pbkdf2.GetBytes(keySizeInBytes);
            }
        }
        #endregion

        #region GetCryptionKey
        private static byte[] GetCryptionKey(byte[] pw, string salt = "")
        {
            byte[] saltFromDatabase = Encoding.UTF8.GetBytes(salt.Length == 0 ? SettingsManager.GetSettings(AppSettingsValues.pSalt) : "");
            int keySizeInBytes = 32;
            int iterations = 10000;

            return DeriveEncryptionKey(pw, saltFromDatabase, keySizeInBytes, iterations);
        }
        private static byte[] GetCryptionKey(SecureString pw, string salt = "")
        {
            byte[] saltFromDatabase = Encoding.UTF8.GetBytes(salt.Length == 0 ? SettingsManager.GetSettings(AppSettingsValues.pSalt) : "");
            int keySizeInBytes = 32;
            int iterations = 10000;

            return DeriveEncryptionKey(pw, saltFromDatabase, keySizeInBytes, iterations);
        }
        #endregion
    }
}