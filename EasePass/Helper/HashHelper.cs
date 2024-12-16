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
