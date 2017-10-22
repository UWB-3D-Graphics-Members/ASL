using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UWBNetworkingPackage
{
    public class RoomHandler : MonoBehaviour
    {
        public static void Start()
        {
            UWB_Texturing.Config.RoomObject.Changed += GenerateRoomFolder;
        }

        public static void ProcessAllRooms()
        {
            string[] roomNames = GetRoomNames();

            for (int i = 0; i < roomNames.Length; i++)
            {
                string roomName = roomNames[i];
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                CreateRoomResources(roomName);
                InstantiateRoom(roomName);
            }
        }

        public static string[] GetRoomNames()
        {
            //string[] roomNames = Directory.GetDirectories(Config_Base.CompileAbsoluteRoomDirectory());
            string[] roomNames = Directory.GetDirectories(Config.Current.Room.CompileAbsoluteAssetDirectory());
            for (int i = 0; i < roomNames.Length; i++)
            {
                string pass1 = roomNames[i].Split('/')[roomNames[i].Split('/').Length];
                string roomName = pass1.Split('\\')[pass1.Split('\\').Length];
                roomNames[i] = roomName;
            }
            return roomNames;
        }

        public static void CreateRoomResources(string roomName)
        {
            //string matrixArrayFilepath = Config.AssetBundle.Current.CompileAbsoluteRoomPath(UWB_Texturing.Config.MatrixArray.CompileFilename(), roomName);
            //string materialsDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            //string meshesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            //string texturesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            //string imagesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            UWB_Texturing.BundleHandler.CreateRoomResources();
        }

        public static void InstantiateRoom(string roomName)
        {
            //string rawResourceBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
            string rawResourceBundlePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
            UWB_Texturing.BundleHandler.InstantiateRoom(rawResourceBundlePath);
        }

        //public static void InstantiateRoomFromResources(string roomName)
        //{
        //    string matrixArrayFilepath = Config.AssetBundle.Current.CompileAbsoluteRoomPath(UWB_Texturing.Config.MatrixArray.CompileFilename(), roomName);
        //    string rawRoomBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
        //    string customMatricesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    string customOrientationDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    string customMeshesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    string textureImagesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    UWB_Texturing.BundleHandler.InstantiateRoomFromResources(roomName, rawRoomBundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, textureImagesDestinationDirectory, matrixArrayFilepath);
        //}

        public static void GenerateRoomFolder(UWB_Texturing.RoomNameChangedEventArgs e)
        {
            string roomName = e.NewName;
            //string roomDirectory = Config.Room.CompileAbsoluteRoomDirectory(roomName);
            string roomDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory(roomName);
            if (!Directory.Exists(roomDirectory))
            {
                AbnormalDirectoryHandler.CreateDirectory(roomDirectory);
            }
        }

#if UNITY_EDITOR
        public static void PackRoomBundle(string roomName)
        {
            // Have to generate Android bundle first or the PC one will be overwritten
            // with the Android one (i.e. deleted) because of how this hooks into the 
            // RoomTexture module
            //string roomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            RoomManager.UpdateRoomBundle(roomName);
            UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.Android);
            string AndroidBundlePath = Config.Android.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename(), roomName);
            if (File.Exists(AndroidBundlePath))
            {
                File.Delete(AndroidBundlePath);
            }
            File.Copy(GeneratedBundlePath, AndroidBundlePath);
            UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.StandaloneWindows);
        }

        public static void PackAllRoomBundles()
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach(string roomName in roomNames)
            {
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
                PackRoomBundle(roomName);
            }

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }

        //public static void PackRoomBundle(BuildTarget targetPlatform)
        //{
        //    //string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
        //    string destinationDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
        //    //UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
        //    UWB_Texturing.BundleHandler.PackFinalRoomBundle(targetPlatform);
        //}

        public static void PackRawRoomResourceBundle(string roomName)
        {
            string roomNAme = UWB_Texturing.Config.RoomObject.GameObjectName;
            RoomManager.UpdateRawRoomBundle(roomName);
            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.Android);
            string AndroidBundlePath = Config.Android.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename(), roomName);
            if (File.Exists(AndroidBundlePath))
            {
                File.Delete(AndroidBundlePath);
            }
            File.Copy(GeneratedBundlePath, AndroidBundlePath);
            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.StandaloneWindows);
        }

        public static void PackAllRawRoomResourceBundles()
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach (string roomName in roomNames)
            {
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
                PackRawRoomResourceBundle(roomName);
            }

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }
#endif
    }
}