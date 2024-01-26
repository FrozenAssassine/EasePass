using System;
using System.IO;
using System.Security.Cryptography;

namespace EasePass.Helper
{
    internal class HashHelper
    {
        public static string HashFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            byte[] hash = MD5.Create().ComputeHash(file);
            return Convert.ToBase64String(hash).Replace("+", "").Replace("=", "");
        }
    }
}
