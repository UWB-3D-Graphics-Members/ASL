using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.Security.Cryptography;
using Windows.Networking; // HostName
using Windows.Networking.Sockets;
using System.Runtime.InteropServices.WindowsRuntime; // Used for ToArray extension method for IBuffers (to convert them to byte arrays)
using Windows.Storage.Streams; // DataWriter/DataReader
#endif

namespace UWBNetworkingPackage
{
    public class ServerFinder_Hololens
    {

#if !UNITY_EDITOR && UNITY_WSA_10_0
    public static string serverIP;
    public static DatagramSocket listener;

    public static async void ServerStartAsync()
    {
        // Get information for connecting or for later reference
        int listenerPort = Config.Ports.FindServer;
        serverIP = IPManager.GetLocalIpAddress();

        // Generate the listener server socket
        listener = new DatagramSocket();
        listener.MessageReceived += AcceptClient;
        // Assumes default inbound buffer size
        await listener.BindEndpointAsync(new HostName(IPManager.GetLocalIpAddress()), Config.Ports.FindServer.ToString());
    }

    public static void AcceptClient(DatagramSocket listener, DatagramSocketMessageReceivedEventArgs args)
    {
        new Task(async () =>
        {
            // Retrieve client IP info
            byte[] serverIPBytes = CryptographicBuffer.ConvertStringToBinary(serverIP, BinaryStringEncoding.Utf8).ToArray();
            //HostName clientIP = args.RemoteAddress;
            //string clientPort = args.RemotePort;
            DataReader reader = args.GetDataReader();
            uint numBytesLoaded = await reader.LoadAsync(1024);
            //byte[] buffer = new byte[reader.UnconsumedBufferLength];
            //reader.ReadBytes(buffer);

            string clientIP = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBuffer(reader.UnconsumedBufferLength));

            DataWriter writer = new DataWriter(await listener.GetOutputStreamAsync(new HostName(clientIP), Config.Ports.FindServer.ToString()));

            writer.WriteBytes(serverIPBytes);
            await writer.StoreAsync(); // necessary??

            // Reset listening status
            await listener.ConnectAsync(new HostName("0.0.0.0"), Config.Ports.FindServer.ToString());

        }).Start();
    }

    // IPAddress string
    public static async void FindServerAsync()
    {
        string IPString = string.Empty;

        DatagramSocket clientSocket = new DatagramSocket();
        string clientIP = IPManager.GetLocalIpAddress();

        clientSocket.MessageReceived += FoundServer;

        //await clientSocket.BindEndpointAsync(new HostName(IPManager.GetLocalIpAddress()), Config.Ports.FindServer.ToString());
        await clientSocket.ConnectAsync(new HostName(IPManager.BroadcastIP), Config.Ports.FindServer.ToString());
        DataWriter writer = new DataWriter(await clientSocket.GetOutputStreamAsync(new HostName(IPManager.BroadcastIP), Config.Ports.FindServer.ToString()));
        byte[] clientIPBytes = CryptographicBuffer.ConvertStringToBinary(clientIP, BinaryStringEncoding.Utf8).ToArray();
        writer.WriteBytes(clientIPBytes);
    }

    public static void FoundServer(DatagramSocket clientSocket, DatagramSocketMessageReceivedEventArgs args)
    {
        new Task(async () =>
        {
            DataReader reader = args.GetDataReader();
            await reader.LoadAsync(1024);
            serverIP = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBuffer(reader.UnconsumedBufferLength));
        }).Start();
    }

    public static bool KillThreads()
        {
            listener.Dispose();
        }
#endif
    }
}