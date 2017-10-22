using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UWBNetworkingPackage
{
    public class SocketServer_PC : Socket_Base_PC
    {
#if !UNITY_WSA_10_0
        public static int numListeners = 15;
        protected static TcpListener listener;

        // Thread signal for client connection
        //public static ManualResetEvent clientConnected = new ManualResetEvent(false);
        
        public static class Messages
        {
            public static class Errors
            {
                public static string ListenerNotPending = "TCPListener is uninitialized";//"TCPListener is either uninitialized or has no clients waiting to send/request messages.";
                public static string SendFileFailed = "Sending of file data failed. File not found.";
                public static string SendDataFailed = "Sending of data failed. Byte array is zero length or null.";
                public static string ReceiveDataFailed = "Data stream was empty.";
            }
        }

        public static void Start()
        {
            int port = Config.Ports.ClientServerConnection;
            listener = new TcpListener(IPAddress.Any, port);
            // Bind listener
            EndPoint localEndpoint = new IPEndPoint(IPAddress.Any, port);
            listener.Server.Bind(localEndpoint);
            // Begin listening
            listener.Server.Listen(numListeners);
            // Start accepting the socket
            listener.BeginAcceptSocket(new AsyncCallback(AcceptSocketCallback), listener);
        }

        public static void AcceptSocketCallback(IAsyncResult ar)
        {
            Debug.Log("Socket accepted! Attempting to process...");

            // Retrieve the listener
            TcpListener listener = (TcpListener)ar.AsyncState;
            listener.BeginAcceptSocket(new AsyncCallback(AcceptSocketCallback), listener);
            
            // Accept the socket
            Socket clientSocket = listener.EndAcceptSocket(ar);
            Debug.Log("Socket finalized and accepted!");
            int clientPort = ((IPEndPoint)clientSocket.RemoteEndPoint).Port;
            Debug.Log("client port is " + clientPort);

            if(clientPort == Config.Ports.Bundle)
            {
                //string[] allFilepaths = Directory.GetFiles(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
                string[] allFilepaths = Directory.GetFiles(Config.Current.AssetBundle.CompileAbsoluteAssetDirectory());
                List<string> fileList = new List<string>();
                foreach(string filepath in allFilepaths)
                {
                    if (!filepath.Contains(".meta"))
                    {
                        fileList.Add(filepath);
                    }
                }


                SendFiles(fileList.ToArray(), clientSocket);
            }
            else if(clientPort == Config.Ports.Bundle_ClientToServer)
            {
                //string bundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                string bundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
                ReceiveFiles(clientSocket, bundleDirectory);
            }
            else if(clientPort == Config.Ports.RoomResourceBundle)
            {
                //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

                string[] roomNames = RoomManager.GetAllRoomNames();
                List<string> filepaths = new List<string>();

                foreach(string roomName in roomNames)
                {
                    UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                    string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                    filepaths.Add(filepath);
                }

                //string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                //SendFile(filepath, clientSocket);

                SendFiles(filepaths.ToArray(), clientSocket);

                UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
            }
            else if(clientPort == Config.Ports.RoomResourceBundle_ClientToServer)
            {
                //string roomResourceBundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                string roomResourceBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
                ReceiveFiles(clientSocket, roomResourceBundleDirectory);
            }
            else if(clientPort == Config.Ports.RoomBundle)
            {
                //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
                string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

                string[] roomNames = RoomManager.GetAllRoomNames();
                List<string> filepaths = new List<string>();

                foreach (string roomName in roomNames)
                {
                    UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                    string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
                    filepaths.Add(filepath);
                    //SendFile(filepath, clientSocket);
                }

                SendFiles(filepaths.ToArray(), clientSocket);

                UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
            }
            else if(clientPort == Config.Ports.RoomBundle_ClientToServer)
            {
                //string roomBundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                string roomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
                ReceiveFiles(clientSocket, roomBundleDirectory);
            }
            else if(clientPort == Config.Ports.AndroidBundle)
            {
                string[] allFilepaths = Directory.GetFiles(Config.Android.AssetBundle.CompileAbsoluteAssetDirectory());
                List<string> fileList = new List<string>();
                foreach (string filepath in allFilepaths)
                {
                    if (!filepath.Contains(".meta"))
                    {
                        fileList.Add(filepath);
                    }
                }
                
                SendFiles(fileList.ToArray(), clientSocket);
            }
            else if(clientPort == Config.Ports.AndroidBundle_ClientToServer)
            {
                //string bundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                string bundleDirectory = Config.Android.AssetBundle.CompileAbsoluteAssetDirectory();
                ReceiveFiles(clientSocket, bundleDirectory);
            }
            else if(clientPort == Config.Ports.AndroidRoomResourceBundle)
            {
                //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

                string[] roomNames = RoomManager.GetAllRoomNames();
                List<string> filepaths = new List<string>();

                foreach (string roomName in roomNames)
                {
                    UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                    string filepath = Config.Android.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                    filepaths.Add(filepath);
                }

                //string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                //SendFile(filepath, clientSocket);

                SendFiles(filepaths.ToArray(), clientSocket);

                UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
            }
            else if(clientPort == Config.Ports.AndroidRoomResourceBundle_ClientToServer)
            {
                string roomResourceBundleDirectory = Config.Android.AssetBundle.CompileAbsoluteAssetDirectory();
                ReceiveFiles(clientSocket, roomResourceBundleDirectory);
            }
            else if(clientPort == Config.Ports.AndroidRoomBundle)
            {
                string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

                string[] roomNames = RoomManager.GetAllRoomNames();
                List<string> filepaths = new List<string>();

                foreach (string roomName in roomNames)
                {
                    UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                    string filepath = Config.Android.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
                    filepaths.Add(filepath);
                    //SendFile(filepath, clientSocket);
                }

                SendFiles(filepaths.ToArray(), clientSocket);

                UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
            }
            else if(clientPort == Config.Ports.AndroidRoomBundle_ClientToServer)
            {
                string roomBundleDirectory = Config.Android.AssetBundle.CompileAbsoluteAssetDirectory();
                ReceiveFiles(clientSocket, roomBundleDirectory);
            }
            else
            {
                Debug.Log("Port not found");
            }
        }

        //public static void AcceptSocketCallback(IAsyncResult ar)
        //{
        //    // Retrieve the listener
        //    TcpListener listener = (TcpListener)ar.AsyncState;

        //    Debug.Log("Listener socket accept started");

        //    // Accept the socket
        //    Socket clientSocket = listener.EndAcceptSocket(ar);

        //    Debug.Log("Socket found");

        //    // Needs to tell the client socket what the server's ip is
        //    string configString = IPManager.CompileNetworkConfigString(Config.Ports.ClientServerConnection);
        //    //byte[] serverIPData = System.Text.Encoding.ASCII.GetBytes(IPManager.ExtractIPAddress(configString));

        //    //byte[] serverIPData = System.Text.Encoding.UTF8.GetBytes(IPManager.ExtractIPAddress(configString));
        //    //clientSocket.Send(serverIPData);

        //    Debug.Log("Client IP address = " + ((IPEndPoint)clientSocket.RemoteEndPoint).Address);
        //    Debug.Log("Client IP address = " + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);

        //    //Debug.Log("Sending server ip data to client socket; IP = " + IPManager.ExtractIPAddress(configString) + "; port = " + IPManager.ExtractPort(configString));
        //    Debug.Log("Sending files to client socket");
        //    Debug.Log("Filenames = ");
        //    string directory = "C:\\Users\\Thomas\\Documents\\tempwritefile";
        //    System.Text.StringBuilder headerBuilder = new System.Text.StringBuilder();
        //    foreach(string filepath in Directory.GetFiles(directory))
        //    {
        //        Debug.Log(Path.GetFileName(filepath));
        //        headerBuilder.Append(Path.GetFileName(filepath));
        //        headerBuilder.Append(";");
        //    }
        //    headerBuilder.Remove(headerBuilder.Length - 1, 1); // Remove the last separator (';')
        //    Debug.Log("Header being sent over is " + headerBuilder.ToString());
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        byte[] headerData = System.Text.Encoding.UTF8.GetBytes(headerBuilder.ToString());
        //        //clientSocket.Send(System.BitConverter.GetBytes(headerData.Length));
        //        ms.Write(System.BitConverter.GetBytes(headerData.Length), 0, System.BitConverter.GetBytes(headerData.Length).Length);
        //        //clientSocket.Send(headerData);
        //        ms.Write(headerData, 0, headerData.Length);

        //        // ERROR TESTING - REMOVE
        //        Debug.Log("int length = " + System.BitConverter.GetBytes(headerData.Length).Length);

        //        string[] filenames = headerBuilder.ToString().Split(';');
        //        foreach (string filename in filenames)
        //        {
        //            string filepath = Path.Combine(directory, filename);
        //            Debug.Log("Sending file at path " + filepath);
        //            byte[] fileData = File.ReadAllBytes(filepath);
        //            //clientSocket.Send(System.BitConverter.GetBytes(fileData.Length));
        //            ms.Write(System.BitConverter.GetBytes(fileData.Length), 0, System.BitConverter.GetBytes(fileData.Length).Length);
        //            //clientSocket.Send(fileData);
        //            ms.Write(fileData, 0, fileData.Length);
        //        }

        //        clientSocket.Send(ms.ToArray());

        //        ms.Close();
        //        clientSocket.Shutdown(SocketShutdown.Both);
        //    }


        //    // Save the socket to the map after determining the port
        //    int clientPort = ((IPEndPoint)clientSocket.RemoteEndPoint).Port;
        //    //if (!socketMap.ContainsKey(clientPort))
        //    //{
        //    //    socketMap.Add(clientPort, new Queue<Socket>());
        //    //    Debug.Log("socket queue generated");
        //    //}
        //    //socketMap[clientPort].Enqueue(clientSocket);

        //    //if(socketMap[clientPort].Count > 0)
        //    //    Debug.Log("socket added to queue");

        //    // Raise the appropriate event saying that a client of the appropriate port type has been found
        //    // ERROR TESTING - NOT YET IMPLEMENTED

        //    Debug.Log("Socket is closed?" + clientSocket.Connected);

        //    Debug.Log("Resetting socket");

        //    // Reset the listener and enqueue it again
        //    //listener.BeginAcceptSocket(new AsyncCallback(AcceptSocketCallback), listener);
        //    //listenerQueue.Enqueue(listener);

        //    Debug.Log("Socket reset for additional clients");
        //    Debug.Log("Socket is closed?" + clientSocket.Connected);
        //}

        //public static void SendServerIP(Socket socket)
        //{
        //    // Needs to tell the client socket what the server's ip is
        //    string configString = IPManager.CompileNetworkConfigString(Config.Ports.ClientServerConnection);
        //    byte[] serverIPData = System.Text.Encoding.UTF8.GetBytes(IPManager.ExtractIPAddress(configString));
        //    socket.Send(serverIPData);
        //    socket.Shutdown(SocketShutdown.Both);
        //}

        //public static void CloseSocket(Socket socket)
        //{
        //    socket.Shutdown(SocketShutdown.Both);
        //}














        //public static TcpListener GetListener(int port)
        //{
        //    if (PortListenerMap.ContainsKey(port))
        //    {
        //        return PortListenerMap[port];
        //    }
        //    else
        //    {
        //        TcpListener listener = new TcpListener(IPAddress.Any, port);
        //        listener.Start();
        //        PortListenerMap.Add(port, listener);
        //        return listener;
        //    }
        //}

        //public static bool CloseListener(int port)
        //{
        //    if (PortListenerMap.ContainsKey(port))
        //    {
        //        PortListenerMap[port].Stop();
        //        PortListenerMap.Remove(port);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool SendData(TcpListener listener, byte[] data)
        //{
        //    if (listener != null)
        //    {
        //        if (data != null && data.Length > 0)
        //        {
        //            new Thread(() =>
        //            {
        //                var client = listener.AcceptTcpClient();

        //                using (var stream = client.GetStream())
        //                {
        //                //needs to be changed back
        //                stream.Write(data, 0, data.Length);
        //                    client.Close();
        //                }
        //            }).Start();

        //            return true;
        //        }
        //        else
        //        {
        //            var client = listener.AcceptTcpClient();
        //            client.Close();
        //            Debug.Log(Messages.Errors.SendDataFailed);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log(Messages.Errors.ListenerNotPending);
        //    }

        //    return false;
        //}

        //public static bool SendData(Config.Ports.Types portType, byte[] data)
        //{
        //    int port = Config.Ports.GetPort(portType);
        //    TcpListener portListener = GetListener(port);
        //    return SendData(portListener, data);
        //}

        //public static bool SendDataFromFile(int port, string filepath)
        //{
        //    TcpListener portListener = GetListener(port);
        //    return SendDataFromFile(portListener, filepath);
        //}

        //private static bool SendDataFromFile(TcpListener listener, string filepath)
        //{
        //    if (listener != null)
        //    {
        //        if (File.Exists(filepath))
        //        {
        //            new Thread(() =>
        //            {
        //                var client = listener.AcceptTcpClient();

        //                using (var stream = client.GetStream())
        //                {
        //                //needs to be changed back
        //                byte[] data = File.ReadAllBytes(filepath);
        //                    stream.Write(data, 0, data.Length);
        //                    client.Close();
        //                }
        //            }).Start();

        //            return true;
        //        }
        //        else
        //        {
        //            var client = listener.AcceptTcpClient();
        //            client.Close();
        //            Debug.Log(Messages.Errors.SendFileFailed);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log(Messages.Errors.ListenerNotPending);
        //    }

        //    return false;
        //}

        //public static bool SendDataFromFile(Config.Ports.Types portType, string filepath)
        //{
        //    int port = Config.Ports.GetPort(portType);
        //    return SendDataFromFile(port, filepath);
        //}

        //public static byte[] ReceiveData(string networkConfig)
        //{
        //    TcpClient client = new TcpClient();
        //    client.Connect(IPAddress.Parse(IPManager.ExtractIPAddress(networkConfig)), Int32.Parse(IPManager.ExtractPort(networkConfig)));
        //    Debug.Log("Client connected to server!");
        //    using (var stream = client.GetStream())
        //    {
        //        byte[] buffer = new byte[1024];
        //        Debug.Log("Byte array allocated");

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            Debug.Log("MemoryStream created");
        //            int numBytesRead;
        //            while ((numBytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                ms.Write(buffer, 0, numBytesRead);
        //                Debug.Log("Data received! Size = " + numBytesRead);
        //            }

        //            Debug.Log("Finish receiving data: size = " + ms.Length);
        //            byte[] data = new byte[ms.Length];
        //            data = ms.ToArray();

        //            client.Close();

        //            return data;
        //        }
        //    }
        //}

        //public static bool ReceiveDataToFile(string networkConfig, string filepath)
        //{
        //    //bool fileOverwritten = File.Exists(filepath);

        //    TcpClient client = new TcpClient();
        //    client.Connect(IPAddress.Parse(IPManager.ExtractIPAddress(networkConfig)), Int32.Parse(IPManager.ExtractPort(networkConfig)));
        //    Debug.Log("Client connected to server!");
        //    using (var stream = client.GetStream())
        //    {
        //        byte[] data = new byte[1024];
        //        Debug.Log("Byte array allocated");

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            Debug.Log("MemoryStream created");
        //            int numBytesRead;
        //            while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
        //            {
        //                ms.Write(data, 0, numBytesRead);
        //                Debug.Log("Data received! Size = " + numBytesRead);
        //            }
        //            Debug.Log("Finish receiving bundle: size = " + ms.Length);
        //            client.Close();

        //            //AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
        //            //string bundleName = bundle.name;

        //            // Save the asset bundle
        //            //filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));

        //            //if (!Directory.Exists(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory()))
        //            //{
        //            //    Directory.CreateDirectory(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
        //            //}

        //            AbnormalDirectoryHandler.CreateDirectoryFromFile(filepath);

        //            if(ms.Length <= 0)
        //            {
        //                Debug.Log(Messages.Errors.ReceiveDataFailed);
        //                return false;
        //            }
        //            else
        //            {
        //                File.WriteAllBytes(filepath, ms.ToArray());
        //                return true;
        //            }

        //            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesPC/" + bundleName + ".asset"), ms.ToArray());
        //            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesAndroid/" + bundleName + ".asset"), ms.ToArray());

        //            //AssetBundle newBundle = AssetBundle.LoadFromMemory(ms.ToArray());
        //            //bundles.Add(bundleName, newBundle);
        //            //Debug.Log("You loaded the bundle successfully.");

        //            //bundle.Unload(true);
        //        }
        //    }

        //    //client.Close();
        //    return true;
        //}

        ////public static TCPHeader ConstructTCPHeader(string filepath)
        ////{
        ////    FileInfo info = new FileInfo(filepath);
        ////    long filesize = info.Length;
        ////    byte[] filepathBytes = System.Text.Encoding.UTF8.GetBytes(filepath);

        ////    TCPHeader header = new TCPHeader()
        ////    {
        ////        MessageSize = filesize,
        ////        FilepathLength = (short)filepathBytes.Length,
        ////        Filepath = filepath
        ////    };

        ////    return header;
        ////}

        ////public static void ConstructTCPHeaderBytes(string filepath)
        ////{

        ////}

        //public static void SendDataFromFile(string targetNetworkConfig, string filepath)
        //{
        //    //byte[] data = File.ReadAllBytes(filepath);
        //    //StreamSocket socket = new StreamSocket();

        //    //using (DataWriter writer = new DataWriter(socket.OutputStream))
        //    //{
        //    //    string port = IPManager.ExtractPort(targetNetworkConfig).ToString();
        //    //    string ip = IPManager.ExtractIPAddress(targetNetworkConfig);

        //    //    HostName localHostName = new HostName(ip);
        //    //}
        //}

        ////public static void ReceiveAssetBundle(string networkConfig, out string bundlePath)
        ////{
        ////    //var networkConfigArray = networkConfig.Split(':');
        ////    //Debug.Log("Start receiving bundle.");
        ////    //TcpClient client = new TcpClient();
        ////    //Debug.Log("IP Address = " + IPAddress.Parse(networkConfigArray[0]).ToString());
        ////    //Debug.Log("networkConfigArray[1] = " + Int32.Parse(networkConfigArray[1]));
        ////    //client.Connect(IPAddress.Parse(networkConfigArray[0]), Int32.Parse(networkConfigArray[1]));


        ////    TcpClient client = new TcpClient();
        ////    client.Connect(IPAddress.Parse(IPManager.ExtractIPAddress(networkConfig)), Int32.Parse(IPManager.ExtractPort(networkConfig)));
        ////    Debug.Log("Client connected to server!");
        ////    using (var stream = client.GetStream())
        ////    {
        ////        byte[] data = new byte[1024];
        ////        Debug.Log("Byte array allocated");

        ////        using (MemoryStream ms = new MemoryStream())
        ////        {
        ////            Debug.Log("MemoryStream created");
        ////            int numBytesRead;
        ////            while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
        ////            {
        ////                ms.Write(data, 0, numBytesRead);
        ////                Debug.Log("Data received! Size = " + numBytesRead);
        ////            }
        ////            Debug.Log("Finish receiving bundle: size = " + ms.Length);
        ////            client.Close();

        ////            AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
        ////            string bundleName = bundle.name;

        ////            // Save the asset bundle
        ////            bundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));

        ////            if (!Directory.Exists(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory()))
        ////            {
        ////                Directory.CreateDirectory(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
        ////            }

        ////            File.WriteAllBytes(bundlePath, ms.ToArray());

        ////            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesPC/" + bundleName + ".asset"), ms.ToArray());
        ////            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesAndroid/" + bundleName + ".asset"), ms.ToArray());

        ////            //AssetBundle newBundle = AssetBundle.LoadFromMemory(ms.ToArray());
        ////            //bundles.Add(bundleName, newBundle);
        ////            Debug.Log("You loaded the bundle successfully.");

        ////            bundle.Unload(true);
        ////        }
        ////    }

        ////    client.Close();
        ////}

        //public static int[] Ports
        //{
        //    get
        //    {
        //        var keys = PortListenerMap.Keys;
        //        int[] ports = new int[keys.Count];
        //        int index = 0;
        //        foreach (var key in keys)
        //        {
        //            ports[index++] = key;
        //        }
        //        return ports;
        //    }
        //}

        ~SocketServer_PC()
        {
            listener.Server.Disconnect(false);
            listener.Server.Close();
            listener.Stop();
        }
#endif
    }
}