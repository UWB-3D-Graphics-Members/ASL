using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class Config_Base_ResourceType
    {
        #region Fields/Properties

#if UNITY_WSA_10_0
        private static string absoluteAssetRootFolder = Application.persistentDataPath;
#elif UNITY_ANDROID
        private static string absoluteAssetRootFolder = "/data/data/" + Application.bundleIdentifier;
        // Application.bundleIdentifier may be replaced by Application.identifier in Unity 5.6.0+
#else
        private static string absoluteAssetRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
#endif
        public static string AbsoluteAssetRootFolder
        {
            get
            {
                return absoluteAssetRootFolder;
            }
            set
            {
#if UNITY_WSA_10_0
                absoluteAssetRootFolder = Application.persistentDataPath;
#elif UNITY_ANDROID
                absoluteAssetRootFolder = value;
#else
                //absoluteAssetRootFolder = Application.dataPath;
                absoluteAssetRootFolder = value;
                // Put in logic for all node types
#endif
                UWB_Texturing.Config_Base.AbsoluteAssetRootFolder = absoluteAssetRootFolder;
            }
        }

        private static string assetSubFolder = "ASL/Resources";
        public static string AssetSubFolder
        {
            get
            {
                return assetSubFolder;
            }
            set
            {
#if UNITY_WSA_10_0
#else
                assetSubFolder = value;
#endif
            }
        }

        //        private static string bundleSubFolder = AssetSubFolder + "/StreamingAssets";
        //        public static string BundleSubFolder
        //        {
        //            get
        //            {
        //                return bundleSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                bundleSubFolder = value;
        //#endif
        //            }
        //        }

        //        private static string roomResourceSubFolder = AssetSubFolder + "/Rooms";
        //        public static string RoomResourceSubFolder
        //        {
        //            get
        //            {
        //                return roomResourceSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                roomResourceSubFolder = value;
        //                UWB_Texturing.Config_Base.AssetSubFolder = roomResourceSubFolder;
        //#endif
        //            }
        //        }

#endregion

#region Methods

        //        public static string CompileUnityRoomDirectory()
        //        {
        //            return RoomResourceSubFolder;
        //        }
        //        public static string CompileUnityRoomDirectory(string roomName)
        //        {
        //            return RoomResourceSubFolder + '/' + roomName;
        //        }
        //        public static string CompileUnityRoomPath(string filename, string roomName)
        //        {
        //            return CompileUnityRoomDirectory(roomName) + '/' + filename;
        //        }
        //        public static string CompileAbsoluteRoomDirectory()
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, RoomResourceSubFolder);
        //#endif
        //        }
        //        public static string CompileAbsoluteRoomDirectory(string roomName)
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, Path.Combine(RoomResourceSubFolder, roomName));
        //#endif
        //        }
        //        public static string CompileAbsoluteRoomPath(string filename, string roomName)
        //        {
        //            return Path.Combine(CompileAbsoluteRoomDirectory(roomName), filename);
        //        }









        public static string CompileAbsoluteAssetDirectory()
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
#endif
        }
        public static string CompileAbsoluteAssetPath(string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }
        public static string CompileUnityAssetDirectory()
        {
            //return "Assets/" + AssetSubFolder;
            return AssetSubFolder;
        }
        public static string CompileUnityAssetPath(string filename)
        {
            return CompileUnityAssetDirectory() + '/' + filename;
        }
        //public static string CompileRoomResourcesLoadPath(string assetNameWithoutExtension, string roomName)
        //{
        //    return RoomResourceSubFolder.Substring(RoomResourceSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        //}
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }
        public static string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        {
            return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }














//        public static string CompileUnityBundleDirectory()
//        {
//            return "Assets/" + BundleSubFolder;
//            //return BundleSubFolder;
//        }
//        public static string CompileUnityBundlePath(string filename)
//        {
//            //return CompileUnityBundleDirectory() + '/' + filename;
//            return Path.Combine(CompileUnityBundleDirectory(), filename);
//        }
//        public static string CompileAbsoluteBundleDirectory()
//        {
//#if UNITY_WSA_10_0
//            return AbsoluteAssetRootFolder;
//#else
//            return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
//#endif
//        }
//        public static string CompileAbsoluteBundlePath(string filename)
//        {
//            return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
//        }

#endregion
    }
}