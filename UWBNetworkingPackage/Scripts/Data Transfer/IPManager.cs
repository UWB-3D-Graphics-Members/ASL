using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
#endif

namespace UWBNetworkingPackage
{
    public static class IPManager
    {
        //public static IPAddress GetLocalIpAddress()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily.ToString() == "InterNetwork")
        //        {
        //            return ip;
        //        }
        //    }
        //    return null;
        //}

        public static string GetLocalIpAddress()
        {
#if !UNITY_EDITOR && UNITY_WSA_10_0
            string ip = null;

            // IPInformation is null if not HostName is not a local IPv4 or
            // IPv6 address retrieved from GetHostNames()
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        localHostName.ToString(); // Get the IP address from the host name
                        break;
                    }
                }
            }

            return ip;
#else
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    return ip.ToString();
                }
            }
            return null;
#endif
        }

        public static string CompileNetworkConfigString(int port)
        {
            return GetLocalIpAddress() + ":" + port;
        }

        public static string ExtractIPAddress(string networkConfigString)
        {
            return networkConfigString.Split(':')[0];
        }

        public static string ExtractPort(string networkConfigString)
        {
            return networkConfigString.Split(':')[1];
        }

        public static string BroadcastIP
        {
            get
            {
                return "255.255.255.255";
            }
        }
    }
}