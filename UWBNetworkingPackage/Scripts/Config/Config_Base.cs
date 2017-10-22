using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public enum NodeType
    {
        Android,
        Hololens,
        Kinect,
        Oculus,
        Vive,
        PC,
        Tango
    };

    public class Config_Base
    {
        #region Fields/Properties

        public static class Messages
        {
            public static string PlatformNotFound = "Platform not found. Please reference NodeType enum in Config_Base file.";
        }

        private static NodeType nodeType = NodeType.PC;
        public static NodeType NodeType
        {
            get
            {
                return nodeType;
            }
            set
            {
                nodeType = value;
            }
        }

        #endregion

        public static void Start(NodeType platform)
        {
            NodeType = platform;
        }


        public static class Ports
        {
            public enum Types
            {
                Bundle,
                Bundle_ClientToServer,
                RoomResourceBundle,
                RoomResourceBundle_ClientToServer,
                RoomBundle,
                RoomBundle_ClientToServer,
                AndroidBundle,
                AndroidBundle_ClientToServer,
                AndroidRoomResourceBundle,
                AndroidRoomResourceBundle_ClientToServer,
                AndroidRoomBundle,
                AndroidRoomBundle_ClientToServer,
                ClientServerConnection,
                FindServer
            }

            public static int GetPort(Types portType)
            {
                switch (portType)
                {
                    case Types.Bundle:
                        return Bundle;
                    case Types.Bundle_ClientToServer:
                        return Bundle_ClientToServer;
                    case Types.RoomResourceBundle:
                        return RoomResourceBundle;
                    case Types.RoomResourceBundle_ClientToServer:
                        return RoomResourceBundle_ClientToServer;
                    case Types.RoomBundle:
                        return RoomBundle;
                    case Types.RoomBundle_ClientToServer:
                        return RoomBundle_ClientToServer;
                    case Types.ClientServerConnection:
                        return ClientServerConnection;
                    case Types.FindServer:
                        return FindServer;
                    case Types.AndroidBundle:
                        return AndroidBundle;
                    case Types.AndroidBundle_ClientToServer:
                        return AndroidBundle_ClientToServer;
                    case Types.AndroidRoomResourceBundle:
                        return AndroidRoomResourceBundle;
                    case Types.AndroidRoomResourceBundle_ClientToServer:
                        return AndroidRoomResourceBundle_ClientToServer;
                    case Types.AndroidRoomBundle:
                        return AndroidRoomBundle;
                    case Types.AndroidRoomBundle_ClientToServer:
                        return AndroidRoomBundle_ClientToServer;
                }

                return Base;
            }
            public static Types GetPortType(int port)
            {
                if (port == Bundle)
                    return Types.Bundle;
                else if (port == Bundle_ClientToServer)
                    return Types.Bundle_ClientToServer;
                else if (port == RoomResourceBundle)
                    return Types.RoomResourceBundle;
                else if (port == RoomResourceBundle_ClientToServer)
                    return Types.RoomResourceBundle_ClientToServer;
                else if (port == RoomBundle)
                    return Types.RoomBundle;
                else if (port == RoomBundle_ClientToServer)
                    return Types.RoomBundle_ClientToServer;
                else if (port == ClientServerConnection)
                    return Types.ClientServerConnection;
                else if (port == FindServer)
                    return Types.FindServer;
                else if (port == AndroidBundle)
                    return Types.AndroidBundle;
                else if (port == AndroidBundle_ClientToServer)
                    return Types.AndroidBundle_ClientToServer;
                else if (port == AndroidRoomResourceBundle)
                    return Types.AndroidRoomResourceBundle;
                else if (port == AndroidRoomResourceBundle_ClientToServer)
                    return Types.AndroidRoomResourceBundle_ClientToServer;
                else if (port == AndroidRoomBundle)
                    return Types.AndroidRoomBundle;
                else if (port == AndroidRoomBundle_ClientToServer)
                    return Types.AndroidRoomBundle_ClientToServer;
                else
                {
                    Debug.LogError("Port type not found for port " + port);
                    throw new System.Exception();
                }
            }

            private static int port = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().Port;
            public static int Base
            {
                get
                {
                    return port;
                }
                set
                {
                    if (value < 64000 && value > 20000)
                    {
                        port = value;
                    }
                    else
                    {
                        Debug.Log("Invalid port chosen. Please select a port between 20000 and 64000");
                    }
                }
            }
            public static int FindServer
            {
                get
                {
                    return Base + 1;
                }
            }
            public static int ClientServerConnection
            {
                get
                {
                    return Base + 2;
                }
            }
            public static int Bundle
            {
                get
                {
                    return Base + 3;
                }
            }
            public static int Bundle_ClientToServer
            {
                get
                {
                    return Base + 4;
                }
            }
            public static int RoomResourceBundle
            {
                get
                {
                    return Base + 5;
                }
            }
            public static int RoomResourceBundle_ClientToServer
            {
                get
                {
                    return Base + 6;
                }
            }
            public static int RoomBundle
            {
                get
                {
                    return Base + 7;
                }
            }
            public static int RoomBundle_ClientToServer
            {
                get
                {
                    return Base + 8;
                }
            }
            public static int AndroidBundle
            {
                get { return Base + 9; }
            }
            public static int AndroidBundle_ClientToServer
            {
                get { return Base + 10; }
            }
            public static int AndroidRoomResourceBundle
            {
                get { return Base + 11; }
            }
            public static int AndroidRoomResourceBundle_ClientToServer
            {
                get { return Base + 12; }
            }
            public static int AndroidRoomBundle
            {
                get { return Base + 13; }
            }
            public static int AndroidRoomBundle_ClientToServer
            {
                get { return Base + 14; }
            }
        }
    }
}