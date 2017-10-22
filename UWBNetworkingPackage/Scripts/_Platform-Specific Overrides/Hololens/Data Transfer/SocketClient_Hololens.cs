using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using Windows.System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
using Windows.Security.Cryptography; // Convert string to bytes
#endif

namespace UWBNetworkingPackage
{
    public class SocketClient_Hololens : Socket_Base_Hololens
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        public static void RequestFiles(int port, string receiveDirectory)
        {
            RequestFiles(ServerFinder.serverIP, port, receiveDirectory);
        }

        public static void RequestFiles(string serverIP, int port, string receiveDirectory)
        {
            new Task(async () =>
            {
                // Generate the socket and connect to the server
                StreamSocket socket = new StreamSocket();
                
                int serverPort = Config.Ports.ClientServerConnection;
                EndpointPair endpointPair = new EndpointPair(new HostName(IPManager.GetLocalIpAddress()), port.ToString(), new HostName(serverIP), serverPort.ToString());
                await socket.ConnectAsync(endpointPair);

                // After awaiting the connection, receive data appropriately
                ReceiveFilesAsync(socket, receiveDirectory);

                socket.Dispose();
            }).Start();
        }

        public static void SendFile(string remoteIP, int port, string filepath)
        {
            SendFiles(remoteIP, port, new string[1] { filepath });
        }

        public static void SendFiles(string remoteIP, int port, string[] filepaths)
        {
            new Task(async () =>
            {
                int serverPort = Config.Ports.ClientServerConnection;
                EndpointPair endpointPair = new EndpointPair(new HostName(IPManager.GetLocalIpAddress()), port.ToString(), new HostName(remoteIP), serverPort.ToString());
                StreamSocket socket = new StreamSocket();

                await socket.ConnectAsync(endpointPair);
                Socket_Base_Hololens.SendFilesAsync(filepaths, socket);

                socket.Dispose();
            }).Start();
        }
#endif
    }
}