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

namespace EasePass.Models
{
    internal class EncryptedDatabaseItem
    {
        public EncryptedDatabaseItem(string hash, string salt, byte[] data)
        {
            this.PasswordHash = hash;
            this.Salt = salt;
            this.Data = data;
        }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public byte[] Data { get; set; }
    }
}
