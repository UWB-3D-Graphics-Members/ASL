using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace UWBNetworkingPackage
{
    public class RoomManager : MonoBehaviour
    {
        public string RoomName;

        public static class Messages
        {
            public static class Errors
            {
                public static string RawRoomBundleNotAvailable = "The raw room bundle (raw resources used to generate bundle) is unavailable. Please generate it through the appropriate means and ensure it is in the correct folder.";
                public static string RoomBundleNotAvailable = "The final room bundle is unavailable. Please generate it through the appropriate means and ensure it is in the correct folder.";
            }
        }

        void Start()
        {
            if (string.IsNullOrEmpty(RoomName))
            {
                RoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            }

            InvokeRepeating("UpdateRoomBundles", 0, 3);
        }

        void FixedUpdate()
        {
            if (!RoomName.Equals(UWB_Texturing.Config.RoomObject.GameObjectName))
            {
                UWB_Texturing.Config.RoomObject.GameObjectName = RoomName;
                // Make the directory for this room
                //string directoryPath = Config_Base.CompileAbsoluteRoomDirectory(RoomName);
                string directoryPath = Config.Current.Room.CompileAbsoluteAssetDirectory(RoomName);
                //string directoryPath = UWB_Texturing.Config.RoomObject.CompileAbsoluteAssetDirectory(RoomName);
                AbnormalDirectoryHandler.CreateDirectory(directoryPath);
            }
        }

        public void SyncDisplayedRoomName()
        {
            RoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
        }

        public static string SyncRoomName()
        {
            string roomName = GameObject.Find("RoomManager").GetComponent<RoomManager>().RoomName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
            return roomName;
        }

        // ERROR TESTING - Revisit when we establish a good way to identify 
        public static string[] GetAllRoomNames()
        {
            List<string> roomNames = new List<string>();

            //foreach(string folderPath in Directory.GetDirectories(Config_Base.CompileAbsoluteAssetDirectory()))
            foreach(string folderPath in Directory.GetDirectories(Path.Combine(Config.Current.Room.AbsoluteAssetRootFolder, Config.Current.Room.AssetSubFolder)))
            //foreach (string folderPath in Directory.GetDirectories(Path.Combine(UWB_Texturing.Config_Base.AbsoluteAssetRootFolder, Config_Base.AssetSubFolder)))
            {
                string[] pass1 = folderPath.Split('/');
                string[] pass2 = pass1[pass1.Length - 1].Split('\\');

                string directoryName = pass2[pass2.Length - 1];
                if (!directoryName.StartsWith("_"))
                {
                    roomNames.Add(directoryName);
                    Debug.Log("Room resource folder found!: " + directoryName);
                }
            }

            return roomNames.ToArray();
        }

        public void UpdateRoomBundles()
        {
            string[] roomNames = GetAllRoomNames();
            foreach(string roomName in roomNames)
            {
                Debug.Log("Debugging " + roomName + " UpdateRawRoomBundle");
                UpdateRawRoomBundle(roomName);
                Debug.Log("Debugging " + roomName + " UpdateRoomBundle");
                UpdateRoomBundle(roomName);
            }
        }

        public static void UpdateRawRoomBundle(string roomName)
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
            
            string bundleName = UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename();
            //string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(bundleName);
            string ASLBundlePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(bundleName);
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(bundleName, roomName);
            
            string ASLBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            string GeneratedBundleDirectory = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(ASLBundleDirectory))
                Directory.CreateDirectory(ASLBundleDirectory);
            if (Directory.Exists(GeneratedBundleDirectory))
                Directory.CreateDirectory(GeneratedBundleDirectory);

            if (File.Exists(ASLBundlePath)
                && File.Exists(GeneratedBundlePath))
            {
                DateTime ASLDateTime = File.GetLastWriteTime(ASLBundlePath);
                DateTime RoomTextureDateTime = File.GetLastWriteTime(GeneratedBundlePath);

                if (DateTime.Compare(ASLDateTime, RoomTextureDateTime) < 0)
                {
                    if (File.Exists(ASLBundlePath))
                    {
                        File.Delete(ASLBundlePath);
                    }
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
                else if (DateTime.Compare(RoomTextureDateTime, ASLDateTime) < 0)
                {
                    if (File.Exists(GeneratedBundlePath))
                    {
                        File.Delete(GeneratedBundlePath);
                    }
                    File.Copy(ASLBundlePath, GeneratedBundlePath);
                }
            }
            else if (File.Exists(GeneratedBundlePath)
                && !File.Exists(ASLBundlePath))
            {
                File.Copy(GeneratedBundlePath, ASLBundlePath);
            }
            else if (File.Exists(ASLBundlePath)
                && !File.Exists(GeneratedBundlePath))
            {
                File.Copy(ASLBundlePath, GeneratedBundlePath);
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void UpdateRoomBundle(string roomName)
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

            string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
            //string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));
            string ASLBundlePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(Config.Current.AssetBundle.CompileFilename(bundleName));
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(bundleName, roomName);
            //string GeneratedBundlePath = Config.AssetBundle.PC.CompileAbsoluteAssetPath(Config.AssetBundle.PC.CompileFilename(bundleName));
            
            Debug.Log("Bundle name = " + bundleName);
            Debug.Log("Current directory = " + Directory.GetCurrentDirectory());
            Debug.Log("ASL Bundle path = " + ASLBundlePath);
            Debug.Log("Generated Bundle Path = " + GeneratedBundlePath);

            string ASLBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            string GeneratedBundleDirectory = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(ASLBundleDirectory))
                Directory.CreateDirectory(ASLBundleDirectory);
            if (Directory.Exists(GeneratedBundleDirectory))
                Directory.CreateDirectory(GeneratedBundleDirectory);
            
            if (File.Exists(ASLBundlePath)
                && File.Exists(GeneratedBundlePath))
            {
                DateTime ASLDateTime = File.GetLastWriteTime(ASLBundlePath);
                DateTime RoomTextureDateTime = File.GetLastWriteTime(GeneratedBundlePath);

                if (DateTime.Compare(ASLDateTime, RoomTextureDateTime) < 0)
                {
                    if (File.Exists(ASLBundlePath))
                    {
                        File.Delete(ASLBundlePath);
                    }
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
                else if (DateTime.Compare(RoomTextureDateTime, ASLDateTime) < 0)
                {
                    if (File.Exists(GeneratedBundlePath))
                    {
                        File.Delete(GeneratedBundlePath);
                    }
                    File.Copy(ASLBundlePath, GeneratedBundlePath);
                }
            }
            else if (File.Exists(GeneratedBundlePath)
                && !File.Exists(ASLBundlePath))
            {
                File.Copy(GeneratedBundlePath, ASLBundlePath);
            }
            else if (File.Exists(ASLBundlePath)
                && !File.Exists(GeneratedBundlePath))
            {
                File.Copy(ASLBundlePath, GeneratedBundlePath);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }
    }
}