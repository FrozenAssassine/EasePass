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
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal static class DailyTipHelper
    {
        const string TipURL = "https://github.com/FrozenAssassine/EasePass/raw/master/resources/DailyTips_LANGTAG.txt";

        public static async Task<string> GetTodaysTip(string langTag = "en-US")
        {
            var res = await RequestsHelper.MakeRequest(TipURL.Replace("LANGTAG", langTag));
            if (!res.success) return "";
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(DateTime.Now.ToString("d")));
            Random random = new Random(BitConverter.ToInt32(hash, 0));
            string[] tips = res.result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return tips[random.Next(tips.Length)];
        }
    }
}
