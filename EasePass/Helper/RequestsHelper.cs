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

using EasePass.Dialogs;
using System;
using System.Net.Http;
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


        public static async Task<bool> DownloadFileAsync(string url, string path)
        {
            return await DownloadFileAsync(new Uri(url), path);
        }

        public static async Task<bool> DownloadFileAsync(Uri url, string path)
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();
                        using (HttpContent content = response.Content)
                        {
                            using (var fileStream = System.IO.File.Create(path))
                            {
                                await content.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                InfoMessages.CouldNotGetExtensions(ex.Message);
            }
            return false;
        }
    }
}
