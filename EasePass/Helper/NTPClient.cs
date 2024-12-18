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
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    public static class NTPClient // https://github.com/finn-freitag/Authenticator/blob/main/Authenticator/NTPClient.cs
    {
        public static readonly string DEFAULT_SERVER = "pool.ntp.org";

        public static async Task<DateTime> GetTime(string NTP_Server = "", int timeout = 3000)
        {
            //return DateTime.Now; // I disabled online time to prevent flickering through fast switching between online time and local time.

            if (NTP_Server.Length == 0)
                NTP_Server = DEFAULT_SERVER;

            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            try
            {
                var addresses = (await Dns.GetHostEntryAsync(NTP_Server)).AddressList;
                var ipEndPoint = new IPEndPoint(addresses[0], 123);

                using (var udpClient = new UdpClient())
                {
                    udpClient.Client.ReceiveTimeout = timeout;
                    await udpClient.SendAsync(ntpData, ntpData.Length, ipEndPoint);

                    var result = await udpClient.ReceiveAsync();
                    ntpData = result.Buffer;
                }
            }
            catch
            {
                //return the current time when no network available or the server is not available
                return DateTime.Now;
            }

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

            DateTime onlineTime = networkDateTime.ToLocalTime();
            TimeSpan ts = DateTime.Now - onlineTime;
            if (ts.TotalSeconds < 4) return onlineTime;
            return DateTime.Now;
        }
    }
}
