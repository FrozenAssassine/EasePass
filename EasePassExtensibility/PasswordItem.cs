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

namespace EasePassExtensibility
{
    /// <summary>
    /// IPasswordImporter uses this class to store a password.
    /// </summary>
    public class PasswordItem
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Website { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public string TOTPSecret { get; set; }
        public int TOTPDigits { get; set; }
        public int TOTPInterval { get; set; }
        public Algorithm TOTPAlgorithm { get; set; }

        public enum Algorithm
        {
            SHA1,
            SHA256,
            SHA512
        }
    }
}
