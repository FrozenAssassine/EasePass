using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class HashHelper
    {
        public static string HashFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            byte[] hash = MD5.Create().ComputeHash(file);
            return Convert.ToBase64String(hash).Replace("+","").Replace("=","");
        }
    }
}
