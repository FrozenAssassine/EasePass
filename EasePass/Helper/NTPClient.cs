using System;
using System.Net.Sockets;
using System.Net;

namespace EasePass.Helper
{
    public static class NTPClient // https://github.com/finn-freitag/Authenticator/blob/main/Authenticator/NTPClient.cs
    {
        public static readonly string DEFAULT_SERVER = "pool.ntp.org";

        public static DateTime GetTime(string NTP_Server = "", int timeout = 3000)
        {
            return DateTime.Now; // I disabled online time to prevent flickering through fast switching between online time and local time.

            if (NTP_Server.Length == 0) 
                NTP_Server = DEFAULT_SERVER;

            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            try
            {
                var addresses = Dns.GetHostEntry(NTP_Server).AddressList;
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                socket.ReceiveTimeout = timeout;
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
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
            if (ts.TotalSeconds < 5) return onlineTime;
            return DateTime.Now;
        }
    }
}
