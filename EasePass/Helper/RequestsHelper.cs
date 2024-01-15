using EasePass.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class RequestsHelper
    {
        public static async Task<(bool success, string result)> MakeRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return (true, result);
                    }
                }
                catch (Exception ex)
                {
                    InfoMessages.CouldNotGetExtensions(ex.Message);
                }
                return (false, "");
            }
        }

        [Obsolete]
        public static void DownloadFile(string url, string path) // WebClient is obsolete. Please find a way to use HttpClient
        {
            new WebClient().DownloadFile(url, path);
        }

        public static async Task<(bool success, Stream result)> MakeRequestRaw(string url) // Doesn't work
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        Stream result = await response.Content.ReadAsStreamAsync();
                        return (true, result);
                    }
                }
                catch (Exception ex)
                {
                    InfoMessages.CouldNotGetExtensions(ex.Message);
                }
                return (false, null);
            }
        }
    }
}
