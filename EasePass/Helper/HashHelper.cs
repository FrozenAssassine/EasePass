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
using EasePass.Helper;
using Konscious.Security.Cryptography;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Helper
{
    internal class HashHelper
    {
        /// <summary>
        /// Hashes the given <paramref name="password"/> with the Argon2id Algorithm
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="degreeOfParallelism"></param>
        /// <param name="iterations"></param>
        /// <param name="memorySize"></param>
        /// <param name="hashLength"></param>
        /// <returns></returns>
        public static byte[] HashPasswordWithArgon2id(SecureString password, byte[] salt, int degreeOfParallelism, int iterations, int memorySize, int hashLength)
        {

            if (password == null)
                return Array.Empty<byte>();

            using Argon2id Argon2id = new Argon2id(password.ToBytes());
            Argon2id.Salt = salt;
            Argon2id.DegreeOfParallelism = degreeOfParallelism;
            Argon2id.Iterations = iterations;
            Argon2id.MemorySize = memorySize;

            return Argon2id.GetBytes(hashLength);
        }

        public static string HashFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            byte[] hash = MD5.Create().ComputeHash(file);
            string base64 = Convert.ToBase64String(hash);
            StringBuilder sb = new StringBuilder();
            int length = base64.Length;
            for (int i = 0; i < length; i++)
            {
                if (char.IsLetter(base64[i]))
                {
                    sb.Append(base64[i]);
                }
            }
            return sb.ToString();
        }
    }
}