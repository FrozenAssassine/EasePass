using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Helper
{
    internal class HashHelper
    {
        public static string HashFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            byte[] hash = MD5.Create().ComputeHash(file);
            string base64 = Convert.ToBase64String(hash);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < base64.Length; i++)
                if (char.IsLetter(base64[i]))
                    sb.Append(base64[i]);
            return sb.ToString();
        }
    }
}
