using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    public class Config_Base
    {
        #region Asset Path Event Handling
        public static event AssetPathChangedEventHandler AssetPathChanged;
        
        protected static void OnAssetPathChanged(AssetPathChangedEventArgs e)
        {
            Debug.Log("OnAssetPathChanged triggered! new path = " + e.NewRootFolder + "; old path = " + e.OldRootFolder);
            
            if (AssetPathChanged != null)
                AssetPathChanged(e);
        }

        public static event AssetSubFolderChangedEventHandler AssetSubFolderChanged;

        protected static void OnAssetSubFolderChanged(AssetSubFolderChangedEventArgs e)
        {
            Debug.Log("OnAssetSubFolderChanged triggered! new path = " + e.NewSubFolder + "; old path = " + e.OldSubFolder);
            Debug.Log("raw resource bundle path = " + Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), Config.RoomObject.GameObjectName));
            
            if (AssetSubFolderChanged != null)
                AssetSubFolderChanged(e);
            
        }
        #endregion

        #region Fields/Properties
#if UNITY_WSA_10_0
        private static string absoluteAssetRootFolder = Application.persistentDataPath;
#else
        //public static string absoluteAssetRootFolder = Application.dataPath;
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
                if (!string.IsNullOrEmpty(value))
                {
                    string oldAbsoluteAssetRootFolder = absoluteAssetRootFolder;
                    string newAbsoluteAssetRootFolder = value;
                    absoluteAssetRootFolder = newAbsoluteAssetRootFolder;
                    OnAssetPathChanged(new AssetPathChangedEventArgs(oldAbsoluteAssetRootFolder, newAbsoluteAssetRootFolder));
                }
            }
        }

        //public static string AssetSubFolder = "ASL/Room_Texture/Resources/Room";
        public static string assetSubFolder = (Directory.Exists(Path.Combine(AbsoluteAssetRootFolder, "ASL"))) // Check if this is freestanding or exists inside of the ASL library
            ? "ASL/Room_Texture/Resources/Rooms"
            : "Room_Texture/Resources/Rooms";
        //private static string assetSubFolder = "Room_Texture/Resources/Rooms";
        public static string AssetSubFolder
        {
            get
            {
                return assetSubFolder;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string oldAssetSubFolder = assetSubFolder;
                    string newAssetSubFolder = value;
                    assetSubFolder = newAssetSubFolder;
                    OnAssetSubFolderChanged(new AssetSubFolderChangedEventArgs(oldAssetSubFolder, newAssetSubFolder));
                }
            }
        }

#endregion

        #region Methods

        public static string CompileUnityAssetDirectory(string roomName)
        {
            return "Assets/" + AssetSubFolder + '/' + roomName;
        }
        public static string CompileUnityAssetPath(string filename, string roomName)
        {
            return CompileUnityAssetDirectory(roomName) + '/' + filename;
        }
        public static string CompileAbsoluteAssetDirectory(string roomName)
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            //return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
            return Path.Combine(AbsoluteAssetRootFolder, Path.Combine(AssetSubFolder, roomName));
#endif
        }
        public static string CompileAbsoluteAssetPath(string filename, string roomName)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(roomName), filename);
        }
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension, string roomName)
        {
            int loadPathStartIndex = CompileUnityAssetDirectory(roomName).IndexOf("Resources") + "Resources".Length + 1;
            if (loadPathStartIndex < CompileUnityAssetDirectory(roomName).Length)
            {
                //return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
                return CompileUnityAssetDirectory(roomName).Substring(loadPathStartIndex) + '/' + assetNameWithoutExtension;
            }
            else
            {
                return assetNameWithoutExtension;
            }
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }
        //public static string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        //{
        //    int loadPathStartIndex = assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1;
        //    if (loadPathStartIndex < assetSubDirectory.Length)
        //    {
        //        //return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        //        return assetSubDirectory.Substring(loadPathStartIndex) + '/' + assetNameWithoutExtension;
        //    }
        //    else
        //    {
        //        return assetNameWithoutExtension;
        //    }
        //}
#endregion
    }
}
