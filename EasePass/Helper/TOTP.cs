using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EasyCodeClass
{
    public class TOTP // https://github.com/finn-freitag/Authenticator/blob/main/Authenticator/TOTP.cs
    {
        public static string GenerateTOTPToken(DateTime time, string secretKey, int digits = 6, int interval = 30, HashMode algorithm = HashMode.SHA1)
        {
            double currentTime = (int)(time.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            double timeInterval = currentTime / interval;
            byte[] timeBytes = BitConverter.GetBytes((ulong)timeInterval);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timeBytes);
            }
            byte[] secretKeyBytes = B32ToBytes(secretKey);
            HMAC hmac;
            if (algorithm == HashMode.SHA1)
            {
                hmac = new HMACSHA1();
                //hmac = HMAC.Create("sha1");
            }
            else if (algorithm == HashMode.SHA256)
            {
                hmac = new HMACSHA256(secretKeyBytes);
                //hmac = HMAC.Create("sha256");
            }
            else if (algorithm == HashMode.SHA512)
            {
                hmac = new HMACSHA512(secretKeyBytes);
                //hmac = HMAC.Create("sha512");
            }
            else throw new InvalidOperationException();
            hmac.Key = secretKeyBytes;
            byte[] hmacHash = hmac.ComputeHash(timeBytes);
            int offset = hmacHash[hmacHash.Length - 1] & 0x0f;
            byte[] fourBytes = new byte[4];
            Array.Copy(hmacHash, offset, fourBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(fourBytes);
            }
            uint value = BitConverter.ToUInt32(fourBytes, 0) & 0x7fffffff;
            string str = value.ToString();
            if (str.Length > digits) str = str.Substring(str.Length - digits, digits);
            //uint token = value % Convert.ToUInt32(str);
            return /*token.ToString()*/str.PadLeft(digits, '0');
        }

        public static (string Issuer, string Username, string Secret, HashMode Algorithm, int Digits, int Period) DecodeUrl(string url)
        {
            try
            {
                if (!url.ToLower().StartsWith("otpauth://totp/")) throw new InvalidCastException("Unknown url format!");
                url = url.Substring("otpauth://totp/".Length);
                int sepInd = url.IndexOf(':');
                string issuerpart = url.Substring(0, (sepInd < 0 ? url.ToLower().IndexOf("%3a") : sepInd));
                string Issuer = DecodeUrlStr(issuerpart);
                url = url.Substring(issuerpart.Length + (sepInd < 0 ? 2 : 0) + 1);
                string usernamepart = url.Substring(0, url.IndexOf('?'));
                string Username = DecodeUrlStr(usernamepart);
                url = url.Substring(usernamepart.Length + 1);
                string Secret = "";
                HashMode Algorithm = HashMode.SHA1;
                int Digits = 6;
                int Period = 30;
                while (true)
                {
                    if (url.Contains("&"))
                    {
                        string part = url.Substring(0, url.IndexOf('&'));
                        url = url.Substring(part.Length + 1);
                        if (part.Contains("="))
                        {
                            string[] parts = part.Split('=');
                            switch(parts[0].ToLower())
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
                    }
                    else
                    {
                        if(url.Length > 3)
                        {
                            url = url + '&';
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return (Issuer, Username, Secret, Algorithm, Digits, Period);
            }
            catch { }
            return ("", "", "", HashMode.SHA1, 0, 0);
        }

        public static string EncodeUrl(string Issuer, string Username, string Secret, HashMode Algorithm = HashMode.SHA1, int Digits = 6, int Period = 30)
        {
            return "otpauth://totp/" + Uri.EscapeDataString(Issuer) + ':' + Uri.EscapeDataString(Username) + "?secret=" + Uri.EscapeDataString(Secret) + "&issuer=" + Uri.EscapeDataString(Issuer) + "&algorithm=" + HashModeToString(Algorithm) + "&digits=" + Convert.ToString(Digits) + "&period=" + Convert.ToString(Period);
        }

        public static HashMode StringToHashMode(string str)
        {
            str = str.ToLower();
            if (str == "sha1") return HashMode.SHA1;
            if (str == "sha256") return HashMode.SHA256;
            if (str == "sha512") return HashMode.SHA512;
            throw new InvalidCastException();
        }

        public static string HashModeToString(HashMode hashMode)
        {
            if (hashMode == HashMode.SHA1) return "SHA1";
            if (hashMode == HashMode.SHA256) return "SHA256";
            if (hashMode == HashMode.SHA512) return "SHA512";
            throw new InvalidCastException();
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
            {
                throw new ArgumentNullException(nameof(input));
            }

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
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
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

        private static string B32ToString(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
            var returnArray = new char[charCount];

            byte nextChar = 0, bitsRemaining = 5;
            var arrayIndex = 0;

            foreach (byte b in input)
            {
                nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ValueToChar(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ValueToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((b << bitsRemaining) & 31);
            }

            // if we didn't end with a full char
            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ValueToChar(nextChar);
                // padding
                while (arrayIndex != charCount)
                {
                    returnArray[arrayIndex++] = '=';
                }
            }

            return new string(returnArray);
        }

        private static int CharToValue(char c)
        {
            int value = c;

            // 65-90 == uppercase letters
            if (value < 91 && value > 64)
            {
                return value - 65;
            }

            // 50-55 == numbers 2-7
            if (value < 56 && value > 49)
            {
                return value - 24;
            }

            // 97-122 == lowercase letters
            if (value < 123 && value > 96)
            {
                return value - 97;
            }

            throw new ArgumentException("Character is not a Base32 character.", nameof(c));
        }

        private static char ValueToChar(byte b)
        {
            if (b < 26)
            {
                return (char)(b + 65);
            }

            if (b < 32)
            {
                return (char)(b + 24);
            }

            throw new ArgumentException("Byte is not a Base32 value.", nameof(b));
        }
    }

    public enum HashMode : byte
    {
        SHA1 = 0,
        SHA256 = 1,
        SHA512 = 2
    }
}
