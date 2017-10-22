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

#if !UNITY_EDITOR && UNITY_WSA_10_0
using Windows.System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
#endif

namespace UWBNetworkingPackage
{
    public class SocketServer_Hololens : Socket_Base_Hololens
    {
        #if !UNITY_EDITOR && UNITY_WSA_10_0
        public static int numListeners = 15;
        protected static StreamSocketListener listener;

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

        public static async void StartAsync()
        {
            int port = Config.Ports.ClientServerConnection;
            listener = new StreamSocketListener();
            SetSocketSettings(listener.Control);
            listener.ConnectionReceived += OnConnection;

            // Bind TCP to the server endpoint
            HostName serverHostName = new HostName(IPManager.GetLocalIpAddress());
            int serverPort = Config.Ports.ClientServerConnection;
            await listener.BindEndpointAsync(serverHostName, serverPort.ToString());
        }

        public static void SetSocketSettings(StreamSocketListenerControl socketControl)
        {
            socketControl.QualityOfService = SocketQualityOfService.Normal;
        }

        // ERROR TESTING - Have to revisit after updating sendfiles, sendfile, and receivefiles for Hololens
        public static void OnConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            new Task(() =>
            {
                // Determine which logic to follow based off of client port
                StreamSocket clientSocket = args.Socket;
                string clientIP = args.Socket.Information.RemoteAddress.ToString();
                int clientPort = Int32.Parse(args.Socket.Information.RemotePort);

                if (clientPort == Config.Ports.Bundle)
                {
                    string[] allFilepaths = Directory.GetFiles(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
                    List<string> fileList = new List<string>();
                    foreach (string filepath in allFilepaths)
                    {
                        if (!filepath.Contains(".meta"))
                        {
                            fileList.Add(filepath);
                        }
                    }

                    SendFilesAsync(fileList.ToArray(), clientSocket);
                }
                else if (clientPort == Config.Ports.Bundle_ClientToServer)
                {
                    string bundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                    ReceiveFilesAsync(clientSocket, bundleDirectory);
                }
                else if (clientPort == Config.Ports.RoomResourceBundle)
                {
                    string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                    SendFileAsync(filepath, clientSocket);
                }
                else if (clientPort == Config.Ports.RoomResourceBundle_ClientToServer)
                {
                    string roomResourceBundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                    ReceiveFilesAsync(clientSocket, roomResourceBundleDirectory);
                }
                else if (clientPort == Config.Ports.RoomBundle)
                {
                    string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
                    SendFileAsync(filepath, clientSocket);
                }
                else if (clientPort == Config.Ports.RoomBundle_ClientToServer)
                {
                    string roomBundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                    ReceiveFilesAsync(clientSocket, roomBundleDirectory);
                }
                else if (clientPort == Config.Ports.AndroidBundle)
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
                else if (clientPort == Config.Ports.AndroidBundle_ClientToServer)
                {
                    //string bundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
                    string bundleDirectory = Config.Android.AssetBundle.CompileAbsoluteAssetDirectory();
                    ReceiveFiles(clientSocket, bundleDirectory);
                }
                else if (clientPort == Config.Ports.AndroidRoomResourceBundle)
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
                else if (clientPort == Config.Ports.AndroidRoomResourceBundle_ClientToServer)
                {
                    string roomResourceBundleDirectory = Config.Android.AssetBundle.CompileAbsoluteAssetDirectory();
                    ReceiveFiles(clientSocket, roomResourceBundleDirectory);
                }
                else if (clientPort == Config.Ports.AndroidRoomBundle)
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
                else if (clientPort == Config.Ports.AndroidRoomBundle_ClientToServer)
                {
                    string roomBundleDirectory = Config.Android.AssetBundle.CompileAbsoluteAssetDirectory();
                    ReceiveFiles(clientSocket, roomBundleDirectory);
                }
                else
                {
                    Debug.Log("Port not found");
                }
            }).Start();
        }

        ~SocketServer_Hololens()
        {
            listener.Dispose();
        }
#endif
    }
}