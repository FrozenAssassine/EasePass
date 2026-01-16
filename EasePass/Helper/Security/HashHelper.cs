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
using Konscious.Security.Cryptography;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EasePass.Helper.Security
{
    internal class HashHelper
    {
        /// <summary>
        /// Hashes the given <paramref name="password"/> with the Argon2id Algorithm
        /// </summary>
        /// <param name="password">The Password, which should be hashed</param>
        /// <param name="salt">The Salt, which should be used for the hash</param>
        /// <param name="associatedData">The Associated Data for the Hash</param>
        /// <param name="degreeOfParallelism">The degree of Parallelism</param>
        /// <param name="iterations">The amount of iterations for the hash</param>
        /// <param name="memorySize">The amount of Memory, which should be uused</param>
        /// <param name="hashLength">The length of the Hash in bytes (32 bytes = 256 bits)</param>
        /// <returns>Returns the Hash of the <paramref name="password"/> with the given <paramref name="hashLength"/>.
        /// If the <paramref name="password"/> is equal to <see langword="null"/> <see cref="Array.Empty{T}"/> will be returned.</returns>
        public static byte[] HashPasswordWithArgon2id(SecureString password, byte[] salt, byte[] associatedData = null, int degreeOfParallelism = 10, int iterations = 10, int memorySize = 256_000, int hashLength = 32)
        {
            if (password == null)
                return Array.Empty<byte>();

            return HashPasswordWithArgon2id(password.ToBytes(), salt, associatedData, degreeOfParallelism, iterations, memorySize, hashLength);
        }
        /// <summary>
        /// Hashes the given <paramref name="password"/> with the Argon2id Algorithm
        /// </summary>
        /// <param name="password">The Password, which should be hashed</param>
        /// <param name="salt">The Salt, which should be used for the hash</param>
        /// <param name="associatedData">The Associated Data for the Hash</param>
        /// <param name="degreeOfParallelism">The degree of Parallelism</param>
        /// <param name="iterations">The amount of iterations for the hash</param>
        /// <param name="memorySize">The amount of Memory, which should be uused</param>
        /// <param name="hashLength">The length of the Hash in bytes (32 bytes = 256 bits)</param>
        /// <returns>Returns the Hash of the <paramref name="password"/> with the given <paramref name="hashLength"/>.
        /// If the <paramref name="password"/> is equal to <see langword="null"/> <see cref="Array.Empty{T}"/> will be returned.</returns>
        public static byte[] HashPasswordWithArgon2id(char[] password, byte[] salt, byte[] associatedData = null, int degreeOfParallelism = 10, int iterations = 10, int memorySize = 256_000, int hashLength = 32)
        {
            if (password == null)
                return Array.Empty<byte>();

            return HashPasswordWithArgon2id(Encoding.UTF8.GetBytes(password), salt, associatedData, degreeOfParallelism, iterations, memorySize, hashLength);
        }
        /// <summary>
        /// Hashes the given <paramref name="password"/> with the Argon2id Algorithm
        /// </summary>
        /// <param name="password">The Password, which should be hashed</param>
        /// <param name="salt">The Salt, which should be used for the hash</param>
        /// <param name="associatedData">The Associated Data for the Hash</param>
        /// <param name="degreeOfParallelism">The degree of Parallelism</param>
        /// <param name="iterations">The amount of iterations for the hash</param>
        /// <param name="memorySize">The amount of Memory, which should be uused</param>
        /// <param name="hashLength">The length of the Hash in bytes (32 bytes = 256 bits)</param>
        /// <returns>Returns the Hash of the <paramref name="password"/> with the given <paramref name="hashLength"/>.
        /// If the <paramref name="password"/> is equal to <see langword="null"/> <see cref="Array.Empty{T}"/> will be returned.</returns>
        public static byte[] HashPasswordWithArgon2id(byte[] password, byte[] salt, byte[] associatedData = null, int degreeOfParallelism = 10, int iterations = 10, int memorySize = 256_000, int hashLength = 32)
        {
            if (password == null)
                return Array.Empty<byte>();

            byte[] hash;
            using (Argon2id Argon2id = new Argon2id(password))
            {
                Argon2id.Salt = salt;
                Argon2id.DegreeOfParallelism = degreeOfParallelism;
                Argon2id.Iterations = iterations;
                Argon2id.MemorySize = memorySize;

                if (associatedData != null)
                {
                    Argon2id.AssociatedData = associatedData;
                }
                hash = Argon2id.GetBytes(hashLength);
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
            return hash;
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