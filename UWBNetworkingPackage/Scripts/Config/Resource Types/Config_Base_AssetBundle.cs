using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class Config_Base_AssetBundle : Config_Base_ResourceType
    {
        private static string assetSubFolder = Config_Base_ResourceType.AssetSubFolder + "/StreamingAssets";
        public static new string AssetSubFolder
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
        
        public static new string CompileAbsoluteAssetDirectory()
        {
            // Find the asset directory based off of the nodetype passed in
            return Path.Combine(Directory.GetCurrentDirectory(), AssetSubFolder);
        }

        public static new string CompileAbsoluteAssetPath(string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }

        public static new string CompileUnityAssetDirectory()
        {
            return "Assets/" + AssetSubFolder;
        }

        public static new string CompileUnityAssetPath(string filename)
        {
            return CompileUnityAssetDirectory() + '/' + filename;
        }

        public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }

        public static new string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        {
            return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }

        //public static string Extension = ".asset";
        public static string CompileFilename(string bundleName)
        {
            return bundleName; //+ Extension;
        }
    }
}