using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal static class DailyTipHelper
    {
        const string TipURL = "https://github.com/FrozenAssassine/EasePass/raw/master/resources/DailyTips.txt";

        public static async Task<string> GetTodaysTip()
        {
            var res = await RequestsHelper.MakeRequest(TipURL);
            if (!res.success) return "";
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(DateTime.Now.ToString("d")));
            Random random = new Random(BitConverter.ToInt32(hash, 0));
            string[] tips = res.result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return tips[random.Next(tips.Length)];
        }
    }
}
