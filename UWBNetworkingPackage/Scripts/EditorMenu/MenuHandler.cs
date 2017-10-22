using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage
{
    public class MenuHandler : MonoBehaviour
    {
//        private static PhotonView photonView;

//        public void Start()
//        {
//            //photonView = PhotonView.Get(GameObject.Find("NetworkManager").GetComponent<PhotonView>());
//            photonView = PhotonView.Find(1);
//            if (photonView == null)
//                Debug.Log("Wrong photon view id given");
//            // ERROR TESTING
//        }

//#if UNITY_EDITOR
//        public static void PackRawResourcesBundle()
//        {
//            //string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
//            //UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
//            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.StandaloneWindows);
//        }

//        public static void PackRoomBundle()
//        {
//            //string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
//            //UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
//            UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.StandaloneWindows);
//        }
//#endif

//        public static void ExportRawResources(int targetID)
//        {
//            //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
//            string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
//            int rawRoomPort = Config.Ports.RoomResourceBundle_ClientToServer;
//#if !UNITY_WSA_10_0
//            SocketClient_PC.SendFile(ServerFinder.serverIP, rawRoomPort, filepath);
//            Debug.Log("Exporting raw room resources not currently implemented correctly! Doesn't consider target ID and just sends to master");
//#elif !UNITY_EDITOR && UNITY_WSA_10_0
//            SocketClient_Hololens.SendFile(ServerFinder_Hololens.serverIP, rawRoomPort, filepath);
//#endif
//        }


        //public static void ProcessRoomResources()
        //{
        //    string roomName = UWB_Texturing.Config.RoomObject.GameObjectName;
        //    //string customOrientationFilepath = Config.AssetBundle.Current.CompileAbsoluteAssetPath(UWB_Texturing.Config.CustomOrientation.CompileFilename());
        //    string customOrientationFilepath = UWB_Texturing.Config.CustomOrientation.CompileAbsoluteAssetPath(UWB_Texturing.Config.CustomOrientation.CompileFilename(), roomName);
        //    string unityMeshesRelativeDirectory = Config.AssetBundle.Current.AssetSubFolder;
        //    string materialsRelativeDirectory = Config.AssetBundle.Current.AssetSubFolder;
        //    if (File.Exists(customOrientationFilepath))
        //    {
        //        // Build room object
        //        string[] customOrientationFileLines = File.ReadAllLines(customOrientationFilepath);
        //        UWB_Texturing.RoomModel.BuildRoomObject(roomName, customOrientationFileLines, unityMeshesRelativeDirectory, materialsRelativeDirectory);
        //    }
        //    else
        //    {
        //        Debug.Log("Unable to build room!");
        //    }
        //    //UWB_Texturing.RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())));
        //}


        #region Instantiate
        public static void InstantiateRoom()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            string rawResourceBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename(), roomName);

            UWB_Texturing.BundleHandler.InstantiateRoom(rawResourceBundlePath);
        }

        public static void InstantiateAllRooms()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.InstantiateAllRooms();
        }
        #endregion

        #region Prefabs
        public static void InstantiateRoomPrefab()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            GameObject room = GameObject.Find(roomName);
            UWB_Texturing.PrefabHandler.CreateRoomPrefab(room);
        }

        public static void InstantiateAllRoomPrefabs()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.PrefabHandler.CreateAllRoomPrefabs();
        }
        #endregion

        #region Delete
        public static void RemoveRoomObject()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRoomObject(roomName);
        }

        public static void RemoveAllRoomObjects()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRoomObjects();
        }


        public static void RemoveRoomPrefab()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.PrefabHandler.DeletePrefab(roomName);
        }

        public static void RemoveAllRoomPrefabs()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.PrefabHandler.DeleteAllPrefabs();
        }

        public static void RemoveRoomResources()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRoomResources(roomName);
        }

        public static void RemoveAllRoomResources()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRoomResources();
        }

        public static void RemoveRoomRawInfo()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRawInfo(roomName);
        }

        public static void RemoveAllRoomRawInfo()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRawInfo();
        }

        public static void RemoveEverything()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRoomObject(roomName);
            UWB_Texturing.BundleHandler.RemoveRoomResources(roomName);
            UWB_Texturing.BundleHandler.RemoveRawInfo(roomName);
        }

        public static void RemoveAllEverything()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRoomObjects();
            UWB_Texturing.BundleHandler.RemoveAllRoomResources();
            UWB_Texturing.BundleHandler.RemoveAllRawInfo();
        }
        #endregion

        #region Bundle
#if UNITY_EDITOR
        public static void PackRoomResourceBundle()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.StandaloneWindows);
            RoomHandler.PackRawRoomResourceBundle(UWB_Texturing.Config.RoomObject.GameObjectName);
        }

        public static void PackAllRoomResourceBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackAllRawRoomTextureBundles(BuildTarget.StandaloneWindows);
            RoomHandler.PackAllRawRoomResourceBundles();
        }

        public static void PackRoomBundle()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.StandaloneWindows);
            RoomHandler.PackRoomBundle(UWB_Texturing.Config.RoomObject.GameObjectName);
        }

        public static void PackAllRoomBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackAllFinalRoomBundles(BuildTarget.StandaloneWindows);
            RoomHandler.PackAllRoomBundles();
        }
#endif

        public static void UnpackRoomResourceBundle()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            string rawRoomBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename(), roomName);

            UWB_Texturing.BundleHandler.UnpackRawResourceTextureBundle(rawRoomBundlePath);
        }

        public static void UnpackAllRoomResourceBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.UnpackAllRawResourceTextureBundles();
        }

        public static void UnpackRoomBundle()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            string roomBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename(), roomName);

            UWB_Texturing.BundleHandler.UnpackFinalRoomTextureBundle(roomBundlePath);
        }

        public static void UnpackAllRoomBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.UnpackAllFinalRoomTextureBundles();
        }
        #endregion

        #region Miscellaneous
        public static void ExportRoom(int targetID)
        {
            //Debug.Log("Export Room entered");
            ////string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();

            ////UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
            //string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
            //string bundlePath = Config.AssetBundle.PC.CompileAbsoluteBundlePath(Config.AssetBundle.PC.CompileFilename(bundleName)); // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE

            //Debug.Log("bundlename = " + bundleName);
            //Debug.Log("bundle path = " + bundlePath);

            //int finalRoomBundlePort = Config.Ports.RoomBundle;
            //////Launcher.SendAssetBundle(targetID, bundlePath, finalRoomBundlePort);
            ////Launcher launcher = Launcher.GetLauncherInstance();
            ////launcher.SendRoomModel(targetID);

            //////Debug.Log("bundle sent");

            //////PhotonPlayer.Find(targetID);

            //////Debug.Log("Photon Player found");

            //////photonView.RPC("ReceiveRoomModel", PhotonPlayer.Find(targetID), IPManager.CompileNetworkConfigString(finalRoomBundlePort));


            //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            int roomBundlePort = Config.Ports.RoomBundle_ClientToServer;
#if !UNITY_WSA_10_0
            SocketClient_PC.SendFile(ServerFinder.serverIP, roomBundlePort, filepath);
            Debug.Log("Exporting raw room resources not currently implemented correctly! Doesn't consider target ID and just sends to master");
#elif !UNITY_EDITOR && UNITY_WSA_10_0
            SocketClient_Hololens.SendFile(ServerFinder.serverIP, roomBundlePort, filepath);
#endif
        }
        
        public static void ProcessAllRooms()
        {
            RoomHandler.ProcessAllRooms();
        }
#endregion
    }
}