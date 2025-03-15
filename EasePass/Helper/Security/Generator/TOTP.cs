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
using System.Security.Cryptography;

namespace EasePass.Helper.Security.Generator
{
    public class TOTP // https://github.com/finn-freitag/Authenticator/blob/main/Authenticator/TOTP.cs
    {
        public static string GenerateTOTPToken(DateTime time, string secretKey, int digits = 6, int interval = 30, HashMode algorithm = HashMode.SHA1)
        {
            double currentTime = (int)(time.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            double timeInterval = currentTime / interval;
            byte[] timeBytes = BitConverter.GetBytes((ulong)timeInterval);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeBytes);

            byte[] secretKeyBytes = B32ToBytes(secretKey);

            HMAC hmac = algorithm switch
            {
                HashMode.SHA1 => new HMACSHA1(),
                HashMode.SHA256 => new HMACSHA256(secretKeyBytes),
                HashMode.SHA512 => new HMACSHA512(secretKeyBytes),
                _ => throw new InvalidOperationException()
            };

            hmac.Key = secretKeyBytes;
            byte[] hmacHash = hmac.ComputeHash(timeBytes);
            int offset = hmacHash[hmacHash.Length - 1] & 0x0f;
            byte[] fourBytes = new byte[4];
            Array.Copy(hmacHash, offset, fourBytes, 0, 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(fourBytes);

            uint value = BitConverter.ToUInt32(fourBytes, 0) & 0x7fffffff;
            string str = value.ToString();
            if (str.Length > digits)
                str = str.Substring(str.Length - digits, digits);

            return str.PadLeft(digits, '0');
        }

        public static (string Issuer, string Username, string Secret, HashMode Algorithm, int Digits, int Period) DecodeUrl(string url)
        {
            try
            {
                if (!url.ToLower().StartsWith("otpauth://totp/")) throw new InvalidCastException("Unknown url format!");
                url = url.Substring("otpauth://totp/".Length);
                string usernamepart = url.Substring(0, url.IndexOf('?'));
                string[] unparts = usernamepart.Split(new string[] { ":", "%3a" }, StringSplitOptions.RemoveEmptyEntries);
                string Issuer = DecodeUrlStr(unparts[0]);
                string Username = unparts.Length > 1 ? DecodeUrlStr(usernamepart) : "";
                url = url.Substring(usernamepart.Length + 1);
                string Secret = "";
                HashMode Algorithm = HashMode.SHA1;
                int Digits = 6;
                int Period = 30;
                while (true)
                {
                    if (!url.Contains("&"))
                    {
                        if (url.Length <= 3)
                            break;

                        url = url + '&';
                        continue;
                    }

                    string part = url.Substring(0, url.IndexOf('&'));
                    url = url.Substring(part.Length + 1);
                    if (!part.Contains("="))
                        continue;

                    string[] parts = part.Split('=');
                    switch (parts[0].ToLower())
                    {
                        case "secret":
                            Secret = DecodeUrlStr(parts[1]);
                            break;
                        case "issuer":
                            Issuer = DecodeUrlStr(parts[1]);
                            break;
                        case "algorithm":
                            Algorithm = StringToHashMode(DecodeUrlStr(parts[1]));
                            break;
                        case "digits":
                            Digits = Convert.ToInt32(DecodeUrlStr(parts[1]));
                            break;
                        case "period":
                            Period = Convert.ToInt32(DecodeUrlStr(parts[1]));
                            break;
                    }
                }
                return (Issuer, Username, Secret, Algorithm, Digits, Period);
            }
            catch { }
            return ("", "", "", HashMode.SHA1, 0, 0);
        }

        public static string EncodeUrl(string Issuer, string Username, string Secret, HashMode Algorithm = HashMode.SHA1, int Digits = 6, int Period = 30)
        {
            return "otpauth://totp/" +
                Uri.EscapeDataString(Issuer) + ':' +
                Uri.EscapeDataString(Username) + "?secret=" +
                Uri.EscapeDataString(Secret) + "&issuer=" +
                Uri.EscapeDataString(Issuer) + "&algorithm=" +
                HashModeToString(Algorithm) + "&digits=" +
                Convert.ToString(Digits) + "&period=" +
                Convert.ToString(Period);
        }

        public static HashMode StringToHashMode(string str)
        {
            return str.ToLower() switch
            {
                "sha1" => HashMode.SHA1,
                "sha256" => HashMode.SHA256,
                "sha512" => HashMode.SHA512,
                _ => throw new InvalidCastException()
            };
        }

        public static string HashModeToString(HashMode hashMode)
        {
            return hashMode switch
            {
                HashMode.SHA1 => "sha1",
                HashMode.SHA256 => "sha256",
                HashMode.SHA512 => "sha512",
                _ => throw new InvalidCastException()
            };
        }

        private static string DecodeUrlStr(string url) // https://stackoverflow.com/questions/1405048/how-do-i-decode-a-url-parameter-using-c
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }

        private static byte[] B32ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            // remove padding characters
            input = input.TrimEnd('=');
            // this must be TRUNCATED
            var byteCount = input.Length * 5 / 8;
            var returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;
            var arrayIndex = 0;

            foreach (var c in input)
            {
                var cValue = CharToValue(c);

                int mask;
                if (bitsRemaining > 5)
                {
                    mask = cValue << bitsRemaining - 5;
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> 5 - bitsRemaining;
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << 3 + bitsRemaining);
                    bitsRemaining += 3;
                }
            }

            // if we didn't end with a full byte
            if (arrayIndex != byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        private static int CharToValue(char c)
        {
            int value = c;

            // 65-90 == uppercase letters
            if (value < 91 && value > 64)
                return value - 65;

            // 50-55 == numbers 2-7
            if (value < 56 && value > 49)
                return value - 24;

            // 97-122 == lowercase letters
            if (value < 123 && value > 96)
                return value - 97;

            throw new ArgumentException("Character is not a Base32 character.", nameof(c));
        }
    }

    public enum HashMode : byte
    {
        SHA1 = 0,
        SHA256 = 1,
        SHA512 = 2
    }
}
