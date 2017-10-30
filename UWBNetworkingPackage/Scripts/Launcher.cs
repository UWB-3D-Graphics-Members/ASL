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
using HoloToolkit.Unity.SpatialMapping;
#endif

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Launcher is an abstract class that contains the base functionality needed for any device to 
    /// connect to a Game Room via Photon Unity Networking. 
    /// </summary>
    public abstract class Launcher : Photon.PunBehaviour
    {
        #region Private Properties

        private static string _version = "1";   // Should be set to the current version of your application 
        private DateTime lastRoomUpdate = DateTime.MinValue;
        //private NodeType lastNodeType = Config.AssetBundle.Current.NodeType;
        private NodeType lastNodeType = Config.Current.NodeType;
        #endregion

        #region Public Properties

        // Needed for Room Mesh sending
        [Tooltip("A port number for devices to communicate through. The port number should be the same for each set of projects that need to connect to each other and share the same Room Mesh.")]
        public int Port;

        // Needed for Photon 
        [Tooltip("The name of the room that this project will attempt to connect to. This room must be created by a \"Master Client\".")]
        public string RoomName;

        // Used for SendMesh/ReceiveMesh compatibility
        // Locally stores version of assetbundle received
        public AssetBundle networkAssets;

        #endregion

        /// <summary>
        /// Sets the Photon Network settings on awake
        /// </summary>
        public virtual void Awake()
        {
            //PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.MaxResendsBeforeDisconnect = 5;
            
            /// Prevent GameObjects from being cleaned up after their original owner leaves the room
            PhotonNetwork.autoCleanUpPlayerObjects = false;
            Debug.Log("Disabled autoCleanUpPlayerObjects");

            Port = gameObject.GetComponent<NetworkManager>().Port;
            RoomName = gameObject.GetComponent<NetworkManager>().RoomName;

            Debug.Log("Launcher awaken");
        }

        /// <summary>
        /// Attempts to connect to the specified Room Name on start
        /// </summary>
        public virtual void Start()
        {
            Connect();
            // Adds mesh display component for displaying RoomMesh (?)
            // gameObject.AddComponent<MeshDisplay>();
        }

        public virtual void Update()
        {
            //bool isMasterClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().MasterClient;


            // ERROR TESTING - Aiming to make it so that all things in Config update appropriately, but the code has been previously optimized to say if it's a master client or not.
            // Any way to get whether a node is a certain kind of thing, like Oculus?

            // Need to set custom properties PhotonPlayer.SetCustomProperties({{"nodeType", NodeType.MasterClient}})
            //http://forum.photonengine.com/discussion/1590/how-to-use-custom-player-properties
        }

        /// <summary>
        /// Joins the specified Room Name if already connected to the Photon Network; otherwise, connect to the master server using the current version
        /// number
        /// </summary>
        private void Connect()
        {
            if (PhotonNetwork.connected)
            {
                //bool roomFound = false;
                ////RoomInfo[] roomList = PhotonNetwork.GetRoomList();
                ////for (int i = 0; i < roomList.Length; i++)
                ////{
                ////    if (RoomName.Equals(roomList[i]))
                ////    {
                ////        PhotonNetwork.JoinRoom(RoomName);
                ////        roomFound = true;
                ////        break;
                ////    }
                ////    UnityEngine.Debug.LogError("Room found: " + roomList[i]);
                ////}

                //roomFound = PhotonNetwork.JoinRoom(RoomName);

                //UnityEngine.Debug.LogError("Photon network was connected, and room found status = " + roomFound);

                ////if (!roomFound)
                ////{
                ////    Debug.Error("PUN room not found. Please verify name and determine if master client has initialized room.");
                ////    PhotonNetwork.Disconnect();
                ////}
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_version);
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            ASL.Adapters.PUN.PUNEventCascader.Join();
        }

        [PunRPC]
        public virtual void RequestRoomModel()
        {
            string roomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            //string roomBundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
            string roomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            int roomBundlePort = Config.Ports.RoomBundle;
#if !UNITY_WSA_10_0
            SocketClient_PC.RequestFiles(roomBundlePort, roomBundleDirectory);
#elif !UNITY_EDITOR && UNITY_WSA_10_0
            SocketClient_Hololens.RequestFiles(roomBundlePort, roomBundleDirectory);
#endif
            RoomManager.UpdateRoomBundle(roomName);
        }



        //[PunRPC]
        //public virtual void SendAssetBundle(string path, int port)
        ////public static void SendAssetBundle(int id, string path, int port)
        //{
        //    //TcpListener listener = TCPManager.GetListener(port);
        //    //TCPManager.SendDataFromFile(listener, path);
        //    TCPManager.SendDataFromFile(port, path);

        //    //TcpListener bundleListener = new TcpListener(IPAddress.Any, port);

        //    //bundleListener.Start();
        //    //new Thread(() =>
        //    //{
        //    //    var client = bundleListener.AcceptTcpClient();

        //    //    using (var stream = client.GetStream())
        //    //    {
        //    //        //needs to be changed back
        //    //        byte[] data = File.ReadAllBytes(path);
        //    //        stream.Write(data, 0, data.Length);
        //    //        client.Close();
        //    //    }

        //    //    bundleListener.Stop();
        //    //}).Start();
        //}

        //[PunRPC]
        //public virtual void ReceiveAssetBundle(string networkConfig, string bundleName)
        //{
        //    string bundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));
        //    TCPManager.ReceiveDataToFile(networkConfig, bundlePath);

        //    //string bundlePath;
        //    //ReceiveAssetBundle(networkConfig, out bundlePath);
        //}

        //public static void ReceiveAssetBundle(string networkConfig, out string bundlePath)
        //{
        //    //var networkConfigArray = networkConfig.Split(':');
        //    //Debug.Log("Start receiving bundle.");
        //    //TcpClient client = new TcpClient();
        //    //Debug.Log("IP Address = " + IPAddress.Parse(networkConfigArray[0]).ToString());
        //    //Debug.Log("networkConfigArray[1] = " + Int32.Parse(networkConfigArray[1]));
        //    //client.Connect(IPAddress.Parse(networkConfigArray[0]), Int32.Parse(networkConfigArray[1]));


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

        //            AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
        //            string bundleName = bundle.name;

        //            // Save the asset bundle
        //            bundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));

        //            if (!Directory.Exists(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory())) { 
        //                Directory.CreateDirectory(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
        //            }

        //            File.WriteAllBytes(bundlePath, ms.ToArray());

        //            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/Resources/StreamingAssets/AssetBundlesPC/" + bundleName + ".asset"), ms.ToArray());
        //            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/Resources/StreamingAssets/AssetBundlesAndroid/" + bundleName + ".asset"), ms.ToArray());

        //            //AssetBundle newBundle = AssetBundle.LoadFromMemory(ms.ToArray());
        //            //bundles.Add(bundleName, newBundle);
        //            Debug.Log("You loaded the bundle successfully.");

        //            bundle.Unload(true);
        //        }
        //    }

        //    client.Close();
        //}

        ///// <summary>
        ///// Send mesh to a host specified by networkConfig.
        ///// Currently, only the HoloLens implements this method
        ///// </summary>
        ///// <param name="networkConfig">IP and port number of the host. Format: IP:Port</param>
        //[PunRPC]
        //public virtual void SendMesh(String networkConfig)
        //{
        //    Debug.Log("Callee is not HoloLens and this is a HoloLens only method");
        //}

        //[PunRPC]
        //public virtual void SendAssetBundles(int id)
        //{
        //    string directory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory(); ;

        //    //// ERROR TESTING - MUST GET NODETYPE OF PLAYER BASED OFF OF ID AND THE CORRESPONDING PLAYER'S CUSTOM SETTINGS
        //    //NodeType bundleType = Config.AssetBundle.Current.NodeType;

        //    //switch (bundleType)
        //    //{
        //    //    case NodeType.Android:
        //    //        directory = Config.AssetBundle.Android.CompileAbsoluteBundleDirectory();
        //    //        break;
        //    //    case NodeType.Hololens:
        //    //        directory = Config.AssetBundle.Hololens.CompileAbsoluteBundleDirectory();
        //    //        break;
        //    //    case NodeType.Kinect:
        //    //        directory = Config.AssetBundle.Kinect.CompileAbsoluteBundleDirectory();
        //    //        break;
        //    //    case NodeType.Oculus:
        //    //        directory = Config.AssetBundle.Oculus.CompileAbsoluteBundleDirectory();
        //    //        break;
        //    //    case NodeType.PC:
        //    //        directory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
        //    //        break;
        //    //    case NodeType.Vive:
        //    //        directory = Config.AssetBundle.Vive.CompileAbsoluteBundleDirectory();
        //    //        break;
        //    //}

        //    int bundlePort = Config.Ports.GetPort(Config.Ports.Types.Bundle);
        //    if (Directory.Exists(directory)) {
        //        foreach (string file in Directory.GetFiles(directory))
        //        {
        //            if (!file.Contains("manifest") && !file.Contains("meta"))
        //            {
        //                //TCPManager.SendDataFromFile(Config.Ports.Types.RoomBundle, file);
        //                SendAssetBundle(file, bundlePort);
        //                //SendAssetBundle(id, file, Config.Ports.Bundle);
        //                string bundleName = Path.GetFileName(file);
        //                photonView.RPC("ReceiveAssetBundle", PhotonPlayer.Find(id), IPManager.CompileNetworkConfigString(bundlePort), bundleName);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Send mesh to a host specified by PhotonNetwork.Player.ID.
        /// Currently, only the MasterClient implements this method
        /// </summary>
        /// <param name="id">The player id that will sent the mesh</param>
        //[PunRPC]
        //public virtual void SendRoomModel(int id)
        //{
        //    //Debug.Log("Callee is not MasterClient and this is a MasterClient only method");
        //    //string bundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
        //    string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
        //    string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(bundleName);
        //    RoomTextureManager.UpdateRoomBundle();
        //    int roomModelPort = Config.Ports.GetPort(Config.Ports.Types.RoomBundle);
        //    //SendAssetBundle(id, bundlePath, Config.Ports.RoomBundle);
        //    SendAssetBundle(ASLBundlePath, roomModelPort);
        //    photonView.RPC("ReceiveRoomModel", PhotonPlayer.Find(id), IPManager.CompileNetworkConfigString(roomModelPort), bundleName);
        //}

        //[PunRPC]
        //public virtual void ReceiveRoomModel(string networkConfig, string bundleName)
        //{
        //    //ReceiveAssetBundle(networkConfig);
        //    ReceiveAssetBundle(networkConfig, bundleName);
        //    //UWB_Texturing.BundleHandler.UnpackFinalRoomTextureBundle();

        //    //string bundlePath = Config.AssetBundle.Current.CompileAbsoluteAssetPath(Config.AssetBundle.Current.CompileFilename(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename()));
        //    string bundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename()));
        //    //string roomMatrixPath = Config.AssetBundle.Current.CompileAbsoluteAssetPath(UWB_Texturing.Config.MatrixArray.CompileFilename());
        //    string roomMatrixPath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.MatrixArray.CompileFilename());

        //    UWB_Texturing.BundleHandler.UnpackFinalRoomTextureBundle(bundlePath, roomMatrixPath);
        //}

        //[PunRPC]
        //public virtual void SendRawRoomModelInfo(int id)
        //{
        //    //SendAssetBundle(id, UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename()), Config.Ports.RawRoomBundle);

        //    // Pinpoint the bundle's location and copy it if 
        //    string bundleName = UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename();
        //    string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(bundleName);
        //    RoomTextureManager.UpdateRawRoomBundle();
        //    int rawRoomBundlePort = Config.Ports.RawRoomBundle;
        //    SendAssetBundle(ASLBundlePath, rawRoomBundlePort);
        //    photonView.RPC("ReceiveRawRoomModelInfo", PhotonPlayer.Find(id), IPManager.CompileNetworkConfigString(rawRoomBundlePort), bundleName);
        //}

        //[PunRPC]
        //public virtual void ReceiveRawRoomModelInfo(string networkConfig, string bundleName)
        //{
        //    string bundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
        //    string destinationDirectory = Config.AssetBundle.Current.CompileAbsoluteAssetDirectory();

        //    //ReceiveAssetBundle(networkConfig);
        //    ReceiveAssetBundle(networkConfig, bundleName);
        //    UWB_Texturing.BundleHandler.UnpackRoomTextureBundle(bundlePath, destinationDirectory, destinationDirectory, destinationDirectory, destinationDirectory);
        //}

        /// <summary>
        /// Receive room mesh from specified network configuration.
        /// Currently, only the ReveivingClient class implements this method
        /// </summary>
        /// <param name="networkConfig">The IP and port number that client can receive room mesh from. The format is IP:Port</param>
        //[PunRPC]
        //public virtual void ReceiveMesh(String networkConfig)
        //{
        //    Debug.Log("Callee is not a regular client and this is a regular client only method");
        //}

        /// <summary>
        /// Receive room mesh from specifed PhotonNetwork.Player.ID. 
        /// Currently, only the MasterClient implements this method
        /// </summary>
        /// <param name="id">The player id that will receive mesh</param>
        //[PunRPC]
        //public virtual void ReceiveMesh(int id)
        //{
        //    Debug.Log("Callee is not MasterClient and this is a MasterClient only method");
        //}

        ///// <summary>
        ///// This will send a call to delete all meshes held by the clients
        ///// This is a RPC method that will be called by ReceivingClient
        ///// </summary>
        //[PunRPC]
        //public virtual void DeleteRoomInfo()
        //{

        //}

        /// <summary>
        /// Deletes local copy of the mesh
        /// </summary>
        [PunRPC]
        public void DeleteLocalRoomModelInfo(string roomName)
        {
            // ERROR TESTING -> these might point to the wrong folders -> update to search through appropriate folders
            //string materialDirectory = Config.AssetBundle.Current.CompileAbsoluteAssetDirectory();
            string materialDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory();
            string meshesDirectory = materialDirectory;
            string texturesDirectory = materialDirectory;

            //UWB_Texturing.PrefabHandler.DeletePrefabs();
            UWB_Texturing.BundleHandler.RemoveRoomObject(roomName);
            UWB_Texturing.BundleHandler.RemoveRawInfo(roomName);
            UWB_Texturing.BundleHandler.RemoveRoomResources(roomName);
        }

        //// ERROR TESTING - REMOVE if not needed
        //[PunRPC]
        //public virtual void SendNetworkConfig(int id, int port)
        //{
        //    string networkConfig = IPManager.CompileNetworkConfigString(port);
        //    int networkConfigPort = Config.Ports.GetPort(Config.Ports.Types.NetworkConfig);
        //    photonView.RPC("ReceiveNetworkConfig", PhotonPlayer.Find(id), IPManager.CompileNetworkConfigString(networkConfigPort));
        //}

        //// ERROR TESTING - REMOVE if not needed
        //[PunRPC]
        //public virtual string ReceiveNetworkConfig(string networkConfig, string networkConfigToUse)
        //{
        //    return networkConfigToUse;
        //}

        // ERROR TESTING - REWORK AND REIMPLEMENT

        ///// <summary>
        ///// Needs to be called in order to instantiate an object from asset bundle.
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <param name="rot"></param>
        ///// <param name="id1"></param>
        ///// <param name="np"></param>
        //[PunRPC]
        //public void SpawnNetworkObject(Vector3 pos, Quaternion rot, int id1, String assetName)
        //{
        //    GameObject networkObject = networkAssets.LoadAsset(assetName) as GameObject;
        //    Instantiate(networkObject, pos, rot);
        //    PhotonView[] nViews = networkObject.GetComponentsInChildren<PhotonView>();
        //    nViews[0].viewID = id1;
        //}

#region TangoRPC
        [PunRPC]
        public virtual void SendTangoMesh()
        {

        }

        [PunRPC]
        public virtual void SendTangoMesh(int id)
        {

        }

        [PunRPC]
        public virtual void SendTangoMesh(string networkConfig)
        {

        }

        [PunRPC]
        public virtual void ReceiveTangoMesh()
        {

        }

        [PunRPC]
        public virtual void ReceiveTangoMesh(int id)
        {

        }

        [PunRPC]
        public virtual void ReceiveTangoMesh(string networkConfig)
        {

        }

#endregion

        #region Byte stream SendMesh/ReceiveMesh (i.e. uses Database class)
        /// <summary>
        /// Send mesh to a host specified by networkConfig.
        /// Currently, only the HoloLens implements this method
        /// </summary>
        /// <param name="networkConfig">IP and port number of the host. Format: IP:Port</param>
        [PunRPC]
        public virtual void SendMesh(String networkConfig)
        {
            Debug.Log("Callee is not HoloLens and this is a HoloLens only method");
        }

        /// <summary>
        /// Send mesh to a host specified by PhotonNetwork.Player.ID.
        /// Currently, only the MasterClient implements this method
        /// </summary>
        /// <param name="id">The player id that will sent the mesh</param>
        [PunRPC]
        public virtual void SendMesh(int id)
        {
            Debug.Log("Callee is not MasterClient and this is a MasterClient only method");
        }

        //Virtual client method for sending mesh
        public virtual void SendMesh()
        {

        }

        //Virtual client method for sending addMesh
        public virtual void SendAddMesh()
        {

        }

        /// <summary>
        /// Receive room mesh from specified network configuration.
        /// Currently, only the ReveivingClient class implements this method
        /// </summary>
        /// <param name="networkConfig">The IP and port number that client can reveice room mesh from. The format is IP:Port</param>
        [PunRPC]
        public virtual void ReceiveMesh(String networkConfig)
        {
            Debug.Log("Callee is not a regular client and this is a regular client only method");
        }

        /// <summary>
        /// Receive room mesh from specifed PhotonNetwork.Player.ID. 
        /// Currently, only the MasterClient implements this method
        /// </summary>
        /// <param name="id">The player id that will receive mesh</param>
        [PunRPC]
        public virtual void ReceiveMesh(int id)
        {
            Debug.Log("Callee is not MasterClient and this is a MasterClient only method");
        }

        /// <summary>
        /// This will send a call to delete all meshes held by the clients
        /// This is a RPC method that will be called by ReceivingClient
        /// </summary>
        [PunRPC]
        public virtual void DeleteMesh()
        {

        }

        /// <summary>
        /// Deletes local copy of the mesh
        /// </summary>
        [PunRPC]
        public void DeleteLocalMesh()
        {
            if (Database.GetMeshAsBytes() != null)
            {
                Database.DeleteMesh();
            }
            foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject)))
            {
                if (gameObject.name == "mesh")
                {
                    Destroy(gameObject);
                }
            }

        }

        /// <summary>
        /// Needs to be called in order to instatite an object from asset bundle.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="id1"></param>
        /// <param name="np"></param>
        [PunRPC]
        public void SpawnNetworkObject(Vector3 pos, Quaternion rot, int id1, String assetName)
        {
            GameObject networkObject = networkAssets.LoadAsset(assetName) as GameObject;
            Instantiate(networkObject, pos, rot);
            PhotonView[] nViews = networkObject.GetComponentsInChildren<PhotonView>();
            nViews[0].viewID = id1;
        }

        /// <summary>
        /// Initiates the sending of a Mesh from ReceivingClient
        /// </summary>
        /// <param name="networkConfig"></param>
        [PunRPC]
        public virtual void SendAddMesh(string networkConfig)
        {

        }

        /// <summary>
        /// Receive room mesh to add to total mesh (MasterClient)
        /// and add it to the total roommesh
        /// </summary>
        /// <param name="id">The player id that will receive mesh</param>
        [PunRPC]
        public virtual void ReceiveAddMesh(int id)
        {

        }

        /// <summary>
        /// Receive room mesh to add to total mesh (ReceiveClientLauncher)
        /// and add it to the total roommesh
        /// </summary>
        /// <param name="networkConfig">The player id that will receive mesh</param>
        [PunRPC]
        public virtual void ReceiveAddMesh(string networkConfig)
        {

        }

        public static Launcher GetLauncherInstance()
        {
            Launcher launcher = null;
            launcher = GameObject.Find("NetworkManager").GetComponent<MasterClientLauncher>();
            if (launcher == null)
            {
                launcher = GameObject.Find("NetworkManager").GetComponent<ReceivingClientLauncher>();
            }

            return launcher;
        }

        //NEEDED FOR HOLOLENS BUT NOTHING ELSE-------------------------------------------------------------------------------------
        //WILL NOT COMPILE TO ANYTHING ELSE BUT HOLOLENS SO LEAVE THIS SECTION OF CODE COMMENTED OUT IF NOT HOLOLENS
        //
        /// <summary>
        /// MeshDisplay extends SpatialMappingSource (provided by Holotoolkit) to implement the
        /// functionality needed to display a mesh created via a HoloLens
        /// </summary>
#if !UNITY_EDITOR && UNITY_WSA_10_0
//    public class MeshDisplay : SpatialMappingSource
//    {
//        /// <summary>
//        /// Display the mesh currently saved in Database
//        /// </summary>
//        public void DisplayMesh()
//        {
//            var meshes = (List<Mesh>)Database.GetMeshAsList();
//            Debug.Log(meshes.Count);
//            foreach (var mesh in meshes)
//            {
//                GameObject surface = AddSurfaceObject(mesh, string.Format("Beamed-{0}", SurfaceObjects.Count), transform);
//                surface.transform.parent = SpatialMappingManager.Instance.transform;
//                surface.GetComponent<MeshRenderer>().enabled = true;
//                surface.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//            }
//        }

//        /// <summary>
//        /// Scan the room and return the scaned room as a RoomMesh (serialized as a byte array)
//        /// This method can ONLY be used by HoloLens
//        /// </summary>
//        /// <returns>Serialized Room Mesh</returns>
//        public byte[] LoadMesh()
//        {
//            SpatialMappingManager mappingManager = GetComponent<SpatialMappingManager>();
//            List<MeshFilter> meshFilters = mappingManager.GetMeshFilters();
//            List<Mesh> meshes = new List<Mesh>();

//            foreach (var meshFilter in meshFilters)
//            {
//                Mesh mesh = meshFilter.sharedMesh;
//                Mesh clone = new Mesh();
//                List<Vector3> verts = new List<Vector3>();
//                verts.AddRange(mesh.vertices);

//                for (int i = 0; i < verts.Count; i++)
//                {
//                    verts[i] = meshFilter.transform.TransformPoint(verts[i]);
//                }

//                clone.SetVertices(verts);
//                clone.SetTriangles(mesh.triangles, 0);
//                meshes.Add(clone);
//            }

//            return SimpleMeshSerializer.Serialize(meshes);
//        }
//    }
////   END OF NEEDED FOR HOLOLENS BUT NOTHING ELSE-------------------------------------------------------------------------------
//    /// <summary>
#endif
#endregion

        //NEEDED FOR HOLOLENS BUT NOTHING ELSE-------------------------------------------------------------------------------------
        //WILL NOT COMPILE TO ANYTHING ELSE BUT HOLOLENS SO LEAVE THIS SECTION OF CODE COMMENTED OUT IF NOT HOLOLENS
        //
        /// <summary>
        /// MeshDisplay extends SpatialMappingSource (provided by Holotoolkit) to implement the
        /// functionality needed to display a mesh created via a HoloLens
        /// </summary>
#if !UNITY_EDITOR && UNITY_WSA_10_0
    public class MeshDisplay : SpatialMappingSource
    {
        /// <summary>
        /// Display the mesh currently saved in Database
        /// </summary>
        public void DisplayMesh()
        {
            var meshes = (List<Mesh>)Database.GetMeshAsList();
            Debug.Log(meshes.Count.ToString());
            foreach (var mesh in meshes)
            {
                GameObject surface = AddSurfaceObject(mesh, string.Format("Beamed-{0}", SurfaceObjects.Count), transform);
                surface.transform.parent = SpatialMappingManager.Instance.transform;
                surface.GetComponent<MeshRenderer>().enabled = true;
                surface.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

        /// <summary>
        /// Scan the room and return the scaned room as a RoomMesh (serialized as a byte array)
        /// This method can ONLY be used by HoloLens
        /// </summary>
        /// <returns>Serialized Room Mesh</returns>
        public byte[] LoadMesh()
        {
            SpatialMappingManager mappingManager = GetComponent<SpatialMappingManager>();
            List<MeshFilter> meshFilters = mappingManager.GetMeshFilters();
            List<Mesh> meshes = new List<Mesh>();

            foreach (var meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Mesh clone = new Mesh();
                List<Vector3> verts = new List<Vector3>();
                verts.AddRange(mesh.vertices);

                for (int i = 0; i < verts.Count; i++)
                {
                    verts[i] = meshFilter.transform.TransformPoint(verts[i]);
                }

                clone.SetVertices(verts);
                clone.SetTriangles(mesh.triangles, 0);
                meshes.Add(clone);
            }

            return SimpleMeshSerializer.Serialize(meshes);
        }
    }
//   END OF NEEDED FOR HOLOLENS BUT NOTHING ELSE-------------------------------------------------------------------------------
    /// <summary>
#endif
    }
}


