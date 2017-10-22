using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class ReceivingClientLauncher_PC : ReceivingClientLauncher
    {
#if !UNITY_WSA_10_0
        // Insert PC-specific code here
        public override void Start()
        {
            base.Start();
//#if !UNITY_ANDROID
//            ServerFinder.FindServer();
//#endif
//            SocketServer_PC.Start(); // For sending files to other non-master clients
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            //string roomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            //string roomBundleStorageDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory();
            //SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomBundle, roomBundleStorageDirectory);
            RequestRoomBundles();
            //string rawRoomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            //SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomResourceBundle, rawRoomBundleDirectory);
            string assetBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.Bundle, assetBundleDirectory);
        }

        public void RequestRoomBundles()
        {
            // Generate temp bundle storage directory
            string roomBundleStorageDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory() + "/temp";
            AbnormalDirectoryHandler.CreateDirectory(roomBundleStorageDirectory);
            //Directory.CreateDirectory(roomBundleStorageDirectory);

            Debug.debugging = true;
            Debug.Log("Testing whether room bundle was created at " + roomBundleStorageDirectory);
            if (Directory.Exists(roomBundleStorageDirectory))
            {
                Debug.Log("Room bundle storage directory exists!");
            }
            else
            {
                Debug.Log("Room bundle storage directory does NOT exist!");
            }
            Debug.debugging = false;

            Debug.Log("Room bundle storage directory = " + roomBundleStorageDirectory);
#if UNITY_ANDROID
            SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.AndroidRoomBundle, roomBundleStorageDirectory);
#else
            SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomBundle, roomBundleStorageDirectory);
#endif

            InvokeRepeating("CopyRoomBundles", 3, 1);
        }

        private void CopyRoomBundles()
        {
            string roomBundleStorageDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory() + "/temp";
            string[] tempRoomNames = Directory.GetFiles(roomBundleStorageDirectory);
            Debug.Log("# of Room names found = " + tempRoomNames.Length);
            List<string> roomBundleList = new List<string>();
            foreach (string tempRoomName in tempRoomNames)
            {
                if (!Path.HasExtension(tempRoomName))
                {
                    string[] pass = tempRoomName.Split('/');
                    string[] pass2 = pass[pass.Length - 1].Split('\\');
                    string bundleName = pass2[pass2.Length - 1];
                    //roomNameList.Add(UWB_Texturing.Config.AssetBundle.RoomPackage.ExtractRoomName(bundleName));
                    roomBundleList.Add(bundleName);
                }
            }
            
            // Make room directories
            string[] roomBundles = roomBundleList.ToArray();
            foreach (string roomBundle in roomBundles)
            {
                string roomName = UWB_Texturing.Config.AssetBundle.RoomPackage.ExtractRoomName(roomBundle);
                Debug.Log("Making directory for room named: " + roomBundle);
                
                string roomDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory(roomName);
                if (!Directory.Exists(roomDirectory))
                {
                    AbnormalDirectoryHandler.CreateDirectory(roomDirectory);
                }

                // Copy asset bundles to room directories
                string sourceFilePath;
                string destinationFilePath;
#if UNITY_EDITOR
                if (File.Exists(Config.Current.Room.CompileAbsoluteAssetPath(roomBundle)))
                {
                    Debug.LogError("Room asset bundle exists! File not copied to room directory to avoid potentially overwriting data. Manually copy from " + roomBundleStorageDirectory + " if you would like to update the room.");
                }
                else
                {
                    sourceFilePath = Path.Combine(roomBundleStorageDirectory, roomBundle);
                    destinationFilePath = Config.Current.Room.CompileAbsoluteAssetPath(roomName, roomBundle);
                    File.Copy(sourceFilePath, destinationFilePath);
                }
#else
                sourceFilePath = Path.Combine(roomBundleStorageDirectory, roomBundle);
                destinationFilePath = Config.Current.Room.CompileAbsoluteAssetPath(roomName, roomBundle);
                if(File.Exists(destinationFilePath)){
                    File.Delete(destinationFilePath);
                }
                File.Copy(sourceFilePath, destinationFilePath);
#endif

                // Copy asset bundles to asset bundle directories
                if (File.Exists(Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomBundle)))
                {
                    File.Delete(Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomBundle));
                }
                sourceFilePath = Path.Combine(roomBundleStorageDirectory, roomBundle);
                destinationFilePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomBundle);
                File.Copy(sourceFilePath, destinationFilePath);

                // Remove file from temp folder
                File.Delete(Path.Combine(roomBundleStorageDirectory, roomBundle));
            }

            if (Directory.GetFiles(roomBundleStorageDirectory).Length <= 0)
            {
                CancelInvoke("CopyRoomBundles");

                // Clean up temp storage folder
                string[] tempFiles = Directory.GetFiles(roomBundleStorageDirectory);
                foreach (string tempFile in tempFiles)
                {
                    File.Delete(tempFile);
                }

                Directory.Delete(roomBundleStorageDirectory);

                // Generate the rooms
                UWB_Texturing.BundleHandler.UnpackAllFinalRoomTextureBundles();
            }
        }

        //private void CopyRoomBundles()
        //{
        //    string roomBundleStorageDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory() + "/temp";
        //    string[] tempRoomNames = Directory.GetFiles(roomBundleStorageDirectory);
        //    Debug.Log("# of Room names found = " + tempRoomNames.Length);
        //    List<string> roomNameList = new List<string>();
        //    foreach (string tempRoomName in tempRoomNames)
        //    {
        //        if (!Path.HasExtension(tempRoomName))
        //        {
        //            string[] pass = tempRoomName.Split('/');
        //            string[] pass2 = pass[pass.Length - 1].Split('\\');
        //            string bundleName = pass2[pass2.Length - 1];
        //            roomNameList.Add(UWB_Texturing.Config.AssetBundle.RoomPackage.ExtractRoomName(bundleName));
        //        }
        //    }

        //    // Make room directories
        //    string[] roomNames = roomNameList.ToArray();
        //    foreach (string roomName in roomNames)
        //    {
        //        Debug.Log("Making directory for room named: " + roomName);

        //        string roomDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory(roomName);
        //        if (!Directory.Exists(roomDirectory))
        //        {
        //            AbnormalDirectoryHandler.CreateDirectory(roomDirectory);
        //        }

        //        // Copy asset bundles to room directories
        //        string sourceFilePath;
        //        string destinationFilePath;
        //        if (File.Exists(Config.Current.Room.CompileAbsoluteAssetPath(roomName)))
        //        {
        //            Debug.Error("Room asset bundle exists! File not copied to room directory to avoid potentially overwriting data. Manually copy from " + roomBundleStorageDirectory + " if you would like to update the room.");
        //        }
        //        else
        //        {
        //            sourceFilePath = Path.Combine(roomBundleStorageDirectory, UWB_Texturing.);
        //            destinationFilePath = Config.Current.Room.CompileAbsoluteAssetPath(roomName, roomName);
        //            File.Copy(sourceFilePath, destinationFilePath);
        //        }

        //        // Copy asset bundles to asset bundle directories
        //        if (File.Exists(Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomName)))
        //        {
        //            File.Delete(Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomName));
        //        }
        //        sourceFilePath = Path.Combine(roomBundleStorageDirectory, roomName);
        //        destinationFilePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomName);
        //        File.Copy(sourceFilePath, destinationFilePath);

        //        // Remove file from temp folder
        //        File.Delete(Path.Combine(roomBundleStorageDirectory, roomName));
        //    }

        //    if (Directory.GetFiles(roomBundleStorageDirectory).Length <= 0)
        //    {
        //        CancelInvoke("CopyRoomBundles");

        //        // Clean up temp storage folder
        //        string[] tempFiles = Directory.GetFiles(roomBundleStorageDirectory);
        //        foreach (string tempFile in tempFiles)
        //        {
        //            File.Delete(tempFile);
        //        }

        //        Directory.Delete(roomBundleStorageDirectory);

        //        // Generate the rooms
        //        UWB_Texturing.BundleHandler.UnpackAllFinalRoomTextureBundles();
        //    }
        //}
#endif
            }
}