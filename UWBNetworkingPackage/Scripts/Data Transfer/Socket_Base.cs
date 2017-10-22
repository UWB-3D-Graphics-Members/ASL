using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

namespace UWBNetworkingPackage
{
    public abstract class Socket_Base
    {

        public static void SendFile(string filepath, Socket socket)
        {

        }

        public static void SendFiles(string[] filepaths, Socket socket)
        {
        }

        public static void PrepSocketData(string[] filepaths, ref MemoryStream ms)
        {
        }

        public static string BuildSocketHeader(string[] filepaths)
        {
            return string.Empty;
        }

        public static void ReceiveFiles(Socket socket, string receiveDirectory)
        {
        }

        public static string BytesToString(byte[] bytes)
        {
            return string.Empty;
        }

        public static byte[] StringToBytes(string str)
        {
            return null;
        }
    }
}