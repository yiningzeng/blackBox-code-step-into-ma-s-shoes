using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DeviceServer
{
    public static class Methoded
    {
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            try
            {
                IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
                if (ips != null && ips.Length > 0)
                {
                    foreach (IPAddress ip in ips)
                    {
                        if (ip.AddressFamily.ToString().Equals("InterNetwork"))
                            return ip.ToString();
                    }
                }
            }
            catch { }
            return string.Empty;
        }

    }
}
