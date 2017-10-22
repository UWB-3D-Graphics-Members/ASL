using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if WINDOWS_UWP
//#if UNITY_WSA_10_0
using Windows.Storage;
using System.Threading.Tasks;
using System;
#endif

namespace UWB_Texturing
{
    /// <summary>
    /// Handles logic with unbundling asset bundle.
    /// </summary>
    public static class BundleHandler
    {
        #region Methods

        #region Instantiate Room

        public static void InstantiateRoom(string rawResourceBundlePath)
        {
            string roomName = Config.RoomObject.GameObjectName;

            // Ensure resources are available
            if (!File.Exists(Config.UnityMeshes.CompileAbsoluteAssetPath(Config.UnityMeshes.CompileFilename(0), roomName))
                || !File.Exists(Config.Material.CompileAbsoluteAssetPath(Config.Material.CompileFilename(0), roomName))
                || !File.Exists(Config.Texture2DArray.CompileAbsoluteAssetPath(Config.Texture2DArray.CompileFilename(), roomName)))
            {
                if (!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName))
                    || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName)))
                {
                    // Grab the raw resources
                    if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName)))
                    {
                        Debug.Log("Room resources for instantiating room cannot be found!");
                        return;
                    }
                    UnpackRawResourceTextureBundle(rawResourceBundlePath);
                }
                // Compile the necessary resources
                CreateRoomResources();
                //string materialDirectory = customMatricesDestinationDirectory;
                //string meshesDirectory = customMeshesDestinationDirectory;
                //string texturesDirectory = customMatricesDestinationDirectory;
                //CreateRoomResources(roomName, matrixArrayFilepath, materialDirectory, meshesDirectory, texturesDirectory, textureImagesDestinationDirectory);
            }

            // Ensure that previous room items (resources & game objects) are deleted
            RemoveRoomObject(roomName);

            // Extract camera matrices
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays_AssetsStored(out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Build room
            string orientationFilepath = Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName);
            RoomModel.BuildRoomObject(File.ReadAllLines(orientationFilepath));
            ////RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName)));
            //string[] customOrientationFileLines = File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName));
            //string unityMeshesResourceFolder = string.Join("/", customMeshesDestinationDirectory.Remove(0, customMeshesDestinationDirectory.IndexOf("Assets")).Split('\\'));// customMeshesDestinationDirectory.Remove(0, customMeshesDestinationDirectory.LastIndexOf("Resources") + "Resources".Length + 1);//Config.UnityMeshes.CompileUnityAssetDirectory(roomName);
            //string materialsResourceFolder = string.Join("/", textureImagesDestinationDirectory.Remove(0, textureImagesDestinationDirectory.IndexOf("Assets")).Split('\\'));//textureImagesDestinationDirectory.Remove(0, textureImagesDestinationDirectory.LastIndexOf("Resources") + "Resources".Length + 1);// Config.Material.CompileUnityAssetDirectory(roomName);
            ////RoomModel.BuildRoomObject(roomName, customOrientationFileLines, Config.UnityMeshes.AssetSubFolder, Config.Material.AssetSubFolder);

            //Debug.Log("Unity meshes resource folder (Unity path) = " + unityMeshesResourceFolder);
            //Debug.Log("Unity materials resources folder (Unity path) = " + materialsResourceFolder);

            //RoomModel.BuildRoomObject(roomName, customOrientationFileLines, unityMeshesResourceFolder, materialsResourceFolder);
        }

        //public static void InstantiateRoom()
        //{
        //    string roomName = Config.RoomObject.GameObjectName;
        //    string bundlePath = Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName);
        //    string customMatricesDestinationDirectory = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
        //    string customOrientationDestinationDirectory = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
        //    string customMeshesDestinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
        //    string imagesDestinationDirectory = Config.Images.CompileAbsoluteAssetDirectory(roomName);
        //    string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);

        //    Debug.Log("Raw bundle path = " + bundlePath);
        //    Debug.Log("Custom matrices destination directory = " + customMatricesDestinationDirectory);
        //    Debug.Log("Custom orientation destiantion directory = " + customOrientationDestinationDirectory);
        //    Debug.Log("Custom meshes destination directory = " + customMeshesDestinationDirectory);
        //    Debug.Log("Image destination directory = " + imagesDestinationDirectory);
        //    Debug.Log("Matrix array filepath = " + matrixArrayFilepath);

        //    InstantiateRoomFromResources(roomName, bundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, imagesDestinationDirectory, matrixArrayFilepath);
        //}

        //public static void InstantiateRoomFromResources(string roomName, string rawResourceBundlePath, string customMatricesDestinationDirectory, string customOrientationDestinationDirectory, string customMeshesDestinationDirectory, string textureImagesDestinationDirectory, string matrixArrayFilepath)
        //{
        //    // Ensure resources are available
        //    if (!File.Exists(Config.UnityMeshes.CompileAbsoluteAssetPath(Config.UnityMeshes.CompileFilename(0), roomName))
        //        || !File.Exists(Config.Material.CompileAbsoluteAssetPath(Config.Material.CompileFilename(0), roomName))
        //        || !File.Exists(Config.Texture2DArray.CompileAbsoluteAssetPath(Config.Texture2DArray.CompileFilename(), roomName)))
        //    {
        //        if (!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName))
        //            || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName)))
        //        {
        //            // Grab the raw resources
        //            if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName)))
        //            {
        //                Debug.Log("Room resources for instantiating room cannot be found!");
        //                return;
        //            }
        //            UnpackRawResourceTextureBundle(roomName, rawResourceBundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, textureImagesDestinationDirectory);
        //        }
        //        // Compile the necessary resources
        //        string materialDirectory = customMatricesDestinationDirectory;
        //        string meshesDirectory = customMeshesDestinationDirectory;
        //        string texturesDirectory = customMatricesDestinationDirectory;
        //        CreateRoomResources(roomName, matrixArrayFilepath, materialDirectory, meshesDirectory, texturesDirectory, textureImagesDestinationDirectory);
        //    }

        //    // Ensure that previous room items (resources & game objects) are deleted
        //    RemoveRoomObject();

        //    // Extract camera matrices
        //    Matrix4x4[] WorldToCameraMatrixArray;
        //    Matrix4x4[] ProjectionMatrixArray;
        //    Matrix4x4[] LocalToWorldMatrixArray;
        //    MatrixArray.LoadMatrixArrays_AssetsStored(roomName, matrixArrayFilepath, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

        //    // Build room
        //    //RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())));
        //    string[] customOrientationFileLines = File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName));
        //    string unityMeshesResourceFolder = string.Join("/", customMeshesDestinationDirectory.Remove(0, customMeshesDestinationDirectory.IndexOf("Assets")).Split('\\'));// customMeshesDestinationDirectory.Remove(0, customMeshesDestinationDirectory.LastIndexOf("Resources") + "Resources".Length + 1);//Config.UnityMeshes.CompileUnityAssetDirectory(roomName);
        //    string materialsResourceFolder = string.Join("/", textureImagesDestinationDirectory.Remove(0, textureImagesDestinationDirectory.IndexOf("Assets")).Split('\\'));//textureImagesDestinationDirectory.Remove(0, textureImagesDestinationDirectory.LastIndexOf("Resources") + "Resources".Length + 1);// Config.Material.CompileUnityAssetDirectory(roomName);
        //    //RoomModel.BuildRoomObject(roomName, customOrientationFileLines, Config.UnityMeshes.AssetSubFolder, Config.Material.AssetSubFolder);

        //    Debug.Log("Unity meshes resource folder (Unity path) = " + unityMeshesResourceFolder);
        //    Debug.Log("Unity materials resources folder (Unity path) = " + materialsResourceFolder);

        //    RoomModel.BuildRoomObject(roomName, customOrientationFileLines, unityMeshesResourceFolder, materialsResourceFolder);
        //}

        public static void InstantiateAllRooms()
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                InstantiateRoom(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName));
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }
        #endregion

        #region Packing Room Bundles
#if UNITY_EDITOR
        public static void PackRawRoomTextureBundle(BuildTarget targetPlatform)
        {
            string roomName = Config.RoomObject.GameObjectName;
            string destinationDirectory = Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory(roomName);

            // Ensure that resources exist in project
            if (!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName))
                || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName))
                || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.Images.CompileFilename(0), roomName)))
            {
                //Debug.Log("Raw room resources do not exist. Ensure that items have been saved from room texturing device (e.g. Hololens)");
                // Grab the raw resources
                if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName)))
                {
                    Debug.Log("Room resources for instantiating room cannot be found!");
                    return;
                }
                //UnpackRoomTextureBundle();
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            // Bundle room texture together
            buildMap[0] = new AssetBundleBuild();
            buildMap[0].assetBundleName = Config.AssetBundle.RawPackage.Name;

            // Gather number of assets to place into asset bundle
            int numTextures = MaterialManager.GetNumTextures(roomName);
            int numMeshFiles = 1;
            int numSupplementaryMeshInfoFiles = 1;
            int numMatrixFiles = 1;
            int numAssets = numTextures + numMeshFiles + numSupplementaryMeshInfoFiles + numMatrixFiles;
            string[] textureAssets = new string[numAssets];

            // Assign assets to asset bundle
            int index = 0;
            // Textures
            for (; index < numTextures; index++)
            {
                textureAssets[index] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.Images.CompileFilename(index)); //Config.AssetBundle.InputFolder
            }
            // Mesh
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.CustomMesh.CompileFilename()); //Config.AssetBundle.InputFolder
            // Mesh Supplementary Info
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.CustomOrientation.CompileFilename()); //Config.AssetBundle.InputFolder
            // Matrix Arrays
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.MatrixArray.CompileFilename()); //Config.AssetBundle.InputFolder
            buildMap[0].assetNames = textureAssets;

            // Write asset bundle
            if (!Directory.Exists(destinationDirectory))
            {
                //Directory.CreateDirectory(destinationDirectory);
                AbnormalDirectoryHandler.CreateDirectory(destinationDirectory);
                Debug.Log("Asset Bundle folder created: " + destinationDirectory);
            }
            BuildPipeline.BuildAssetBundles(destinationDirectory, buildMap, BuildAssetBundleOptions.StrictMode, targetPlatform);

            //try
            //{
            //    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            //}
            //catch (System.ArgumentException)
            //{
            //    Directory.CreateDirectory(Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory());
            //    Debug.Log("Asset Bundle folder created: " + Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory());
            //    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            //}

            Debug.Log("Raw Room Resources Bundle generated at " + Path.Combine(destinationDirectory, buildMap[0].assetBundleName));

            BundleHandler.CleanAssetBundleGeneration(destinationDirectory);

            AssetDatabase.Refresh();
        }

        public static void PackRawRoomTextureBundles(string[] roomNames, BuildTarget targetPlatform)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                PackRawRoomTextureBundle(targetPlatform);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void PackAllRawRoomTextureBundles(BuildTarget targetPlatform)
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            PackRawRoomTextureBundles(roomNames, targetPlatform);
        }

        //public static void PackFinalRoomBundle(BuildTarget targetPlatform)
        //{
        //    string roomName = Config.RoomObject.GameObjectName;
        //    string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
        //    PackFinalRoomBundle(destinationDirectory, targetPlatform, roomName);
        //}

        //public static void PackFinalRoomBundle(BuildTarget targetPlatform, string roomName)
        //{
        //    GameObject room = GameObject.Find(roomName);
        //    if(room == null)
        //    {
        //        string roomPrefabPath = Config.Prefab.CompileAbsoluteAssetPath(Config.Prefab.CompileFilename(), roomName);
        //        if (!File.Exists(roomPrefabPath))
        //        {
        //            InstantiateRoom(roomName);
        //            room = GameObject.Find(roomName);
        //            if (room != null)
        //            {
        //                PrefabHandler.CreatePrefab(GameObject.Find(roomName), roomName);
        //                RemoveRoomObject();
        //            }
        //            else
        //            {
        //                Debug.Log("Couldn't pack room prefab.");
        //                return;
        //            }
        //        }
        //    }

        //    AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

        //    // Bundle room texture items together
        //    buildMap[0] = new AssetBundleBuild();
        //    buildMap[0].assetBundleName = Config.AssetBundle.RoomPackage.Name;

        //    // Gather number of assets to place into asset bundle
        //    int numMaterials = MaterialManager.GetNumMaterials(roomName);
        //    string meshDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
        //    int numMeshFiles = CustomMesh.GetNumMeshes(meshDirectory);
        //    int numTexArrays = 1;
        //    int numRoomPrefabs = 1;
        //    int numShaders = 1;
        //    int numMatrixFiles = 1;
        //    int numAssets = numMaterials + numMeshFiles + numTexArrays + numRoomPrefabs + numShaders + numMatrixFiles;
        //    string[] roomAssets = new string[numAssets];

        //    // Assign assets to asset bundle
        //    int index = 0;
        //    // Materials
        //    for (int i = 0; i < numMaterials; i++)
        //    {
        //        string materialName = Config.Material.CompileMaterialName(i);
        //        roomAssets[index++] = Config.Material.CompileUnityAssetPath(materialName, roomName);
        //    }
        //    // Meshes
        //    for (int i = 0; i < numMeshFiles; i++)
        //    {
        //        string meshName = Config.UnityMeshes.CompileMeshName(i);
        //        // ERROR TESTING - reference Mesh, not CustomMesh
        //        roomAssets[index++] = Config.CustomMesh.CompileUnityAssetPath(meshName, roomName);
        //    }
        //    // Texture2DArray
        //    roomAssets[index++] = Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename(), roomName);
        //    // Room Prefab
        //    roomAssets[index++] = Config.Prefab.CompileUnityAssetPath(Config.Prefab.CompileFilename(), roomName);
        //    // Shaders
        //    roomAssets[index++] = Config.Shader.CompileUnityAssetPath(Config.Shader.CompileFilename(), roomName);
        //    // Matrix Files
        //    //roomAssets[index++] = CrossPlatformNames.Matrices.CompileAssetPath(); // ERROR TESTING - Currently sitting outside Resources folder. Fix this.
        //    //Debug.Log("Matrix filepath = " + Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename());
        //    roomAssets[index++] = Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName) + '/' + Config.MatrixArray.CompileFilename();
        //    buildMap[0].assetNames = roomAssets;

        //    // Write asset bundle

        //    if (!Directory.Exists(destinationDirectory))
        //    {
        //    //Directory.CreateDirectory(destinationDirectory);
        //    AbnormalDirectoryHandler.CreateDirectory(destinationDirectory);
        //        Debug.Log("Asset Bundle folder created: " + destinationDirectory);
        //    }
        //    BuildPipeline.BuildAssetBundles(destinationDirectory, buildMap, BuildAssetBundleOptions.StrictMode, targetPlatform);

        //    Debug.Log("Room Prefab Bundle generated at " + Path.Combine(destinationDirectory, buildMap[0].assetBundleName));

        //    BundleHandler.CleanAssetBundleGeneration(destinationDirectory);

        //    AssetDatabase.Refresh();
        //}

        public static void PackFinalRoomBundle(BuildTarget targetPlatform)
        {
            string roomName = Config.RoomObject.GameObjectName;
            GameObject room = GameObject.Find(roomName);
            if (room == null)
            {
                string roomPrefabPath = Config.Prefab.CompileAbsoluteAssetPath(Config.Prefab.CompileFilename(), roomName);
                if (!File.Exists(roomPrefabPath))
                {
                    InstantiateRoom(roomName);
                    room = GameObject.Find(roomName);
                    if (room != null)
                    {
                        PrefabHandler.CreateRoomPrefab(GameObject.Find(roomName));
                        //RemoveRoomObject();
                    }
                    else
                    {
                        Debug.Log("Couldn't pack room prefab.");
                        return;
                    }
                }
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            PrepBundleAssetsForWriting(ref buildMap);
            WriteAssetBundle(buildMap, targetPlatform);
        }

        public static void PackFinalRoomBundles(string[] roomNames, BuildTarget targetPlatform)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                PackFinalRoomBundle(targetPlatform);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void PackAllFinalRoomBundles(BuildTarget targetPlatform)
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            PackFinalRoomBundles(roomNames, targetPlatform);
        }

        #region Helper Functions
        // Preps assets for writing by assigning to the buildMap
        public static void PrepBundleAssetsForWriting(ref AssetBundleBuild[] buildMap)
        {
            string roomName = Config.RoomObject.GameObjectName;

            // Bundle room texture items together
            buildMap[0] = new AssetBundleBuild();
            buildMap[0].assetBundleName = Config.AssetBundle.RoomPackage.Name;

            // Gather number of assets to place into asset bundle
            int numMaterials = MaterialManager.GetNumMaterials(roomName);
            string meshDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            int numMeshFiles = CustomMesh.GetNumMeshes(meshDirectory);
            int numTexArrays = 1;
            int numRoomPrefabs = 1;
            int numShaders = 1;
            int numMatrixFiles = 1;
            int numAssets = numMaterials + numMeshFiles + numTexArrays + numRoomPrefabs + numShaders + numMatrixFiles;
            string[] roomAssets = new string[numAssets];

            // Assign assets to asset bundle
            int index = 0;
            // Materials
            for (int i = 0; i < numMaterials; i++)
            {
                string materialName = Config.Material.CompileMaterialName(i);
                roomAssets[index++] = Config.Material.CompileUnityAssetPath(materialName, roomName);
            }
            // Meshes
            for (int i = 0; i < numMeshFiles; i++)
            {
                string meshName = Config.UnityMeshes.CompileMeshName(i);
                // ERROR TESTING - reference Mesh, not CustomMesh
                roomAssets[index++] = Config.CustomMesh.CompileUnityAssetPath(meshName, roomName);
            }
            // Texture2DArray
            roomAssets[index++] = Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename(), roomName);
            // Room Prefab
            roomAssets[index++] = Config.Prefab.CompileUnityAssetPath(Config.Prefab.CompileFilename(), roomName);
            // Shaders
            roomAssets[index++] = Config.Shader.CompileUnityAssetPath(Config.Shader.CompileFilename(), roomName);
            // Matrix Files
            //roomAssets[index++] = CrossPlatformNames.Matrices.CompileAssetPath(); // ERROR TESTING - Currently sitting outside Resources folder. Fix this.
            //Debug.Log("Matrix filepath = " + Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename());
            roomAssets[index++] = Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName) + '/' + Config.MatrixArray.CompileFilename();
            buildMap[0].assetNames = roomAssets;
        }

        public static void WriteAssetBundle(AssetBundleBuild[] buildMap, BuildTarget targetPlatform)
        {
            string roomName = Config.RoomObject.GameObjectName;
            string bundleDirectory = Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory(roomName);
            // Write asset bundle
            if (!Directory.Exists(bundleDirectory))
            {
                //Directory.CreateDirectory(destinationDirectory);
                AbnormalDirectoryHandler.CreateDirectory(bundleDirectory);
                Debug.Log("Asset Bundle folder created: " + bundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(bundleDirectory, buildMap, BuildAssetBundleOptions.StrictMode, targetPlatform);

            Debug.Log("Room Prefab Bundle generated at " + Path.Combine(bundleDirectory, buildMap[0].assetBundleName));

            BundleHandler.CleanAssetBundleGeneration(bundleDirectory);

            AssetDatabase.Refresh();
        }


        // ERROR TESTING - Removing the manifest is unwanted IF there will be more than one asset bundle
        // associated with something (like the room). Remove the Path.GetExtension(bundleFilepath).Equals(".manifest")
        // line if you want to organize the asset bundles or expand them better by having multiple asset
        // bundles per thing
        public static void CleanAssetBundleGeneration(string bundleDestinationDirectory)
        {
            // Clean up erroneous asset bundle generation
            //string[] bundleFilepaths = Directory.GetFiles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory());
            string[] bundleFilepaths = Directory.GetFiles(bundleDestinationDirectory);

            //Debug.Log("bundle destination directory = " + bundleDestinationDirectory);
            string extraBundleName = GetExtraBundleName(bundleDestinationDirectory);
            for (int i = 0; i < bundleFilepaths.Length; i++)
            {
                string bundleFilepath = bundleFilepaths[i];
                string bundleFilename = Path.GetFileNameWithoutExtension(bundleFilepath);
                if (bundleFilename.Equals(extraBundleName))
                {
                    if (!Path.HasExtension(bundleFilepath)
                        || Path.GetExtension(bundleFilepath).Equals(".meta")
                        || Path.GetExtension(bundleFilepath).Equals(".manifest"))
                    {
                        //Debug.Log("Deleting " + bundleFilepath);
                        //Debug.Log("filename to compare = " + bundleFilename);

                        File.Delete(bundleFilepath);
                    }
                }
            }
        }

        private static string GetExtraBundleName(string bundleDirectory)
        {
            //string[] pass1 = CompileUnityAssetDirectory().Split('/');
            //string[] pass2 = pass1[pass1.Length - 1].Split('\\');
            //return pass2[pass2.Length - 1];

            string[] pass1 = bundleDirectory.Split('/');
            string[] pass2 = pass1[pass1.Length - 1].Split('\\');
            return pass2[pass2.Length - 1];
        }

        #endregion

#endif
        #endregion

        #region Unpacking Room Bundles
        /// <summary>
        /// Runs through the logic of unpacking the room texture bundle, then 
        /// takes the information extracted to generate the room mesh/room 
        /// mesh material appropriately. Assumes certain asset names, asset 
        /// bundle names, and folder locations.
        /// 
        /// NOTE: There is some issue with using constants when specifying 
        /// asset names in a bundle, so names are HARDCODED in the method.
        /// </summary>
        public static void UnpackRawResourceTextureBundle(string rawResourceBundlePath)
        {
            string roomName = Config.RoomObject.GameObjectName;

            string customMatricesDestinationDirectory = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
            string customOrientationDestinationDirectory = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
            string customMeshesDestinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            string textureImagesDestinationDirectory = Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName);

            AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(rawResourceBundlePath);

            // Extract specific text file assets
            // NOTE: Asset name has to be hardcoded.
            //TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
            //TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset("RoomOrientation".ToLower()) as TextAsset;
            //TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset("RoomMesh".ToLower()) as TextAsset;

            TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset(Config.MatrixArray.FilenameRoot.ToLower()) as TextAsset;
            TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset(Config.CustomOrientation.FilenameRoot.ToLower()) as TextAsset;
            TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset(Config.CustomMesh.FilenameRoot.ToLower()) as TextAsset;

            string customMatricesFilepath = Path.Combine(customMatricesDestinationDirectory, Config.MatrixArray.CompileFilename());
            //File.WriteAllLines(customMatricesFilepath, roomMatricesTextAsset.text.Split('\n'));
            //File.WriteAllLines(customMatricesFilepath, string.Join(null, roomMatricesTextAsset.text.Split('\n')).Split(new string[1] { "\r\r" }, System.StringSplitOptions.None));
            File.WriteAllLines(customMatricesFilepath, roomMatricesTextAsset.text.Split(new string[1] { "\r\n" }, System.StringSplitOptions.None));
            //File.WriteAllLines(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename()), roomMatricesTextAsset.text.Split('\n'));
            string customOrientationFilepath = Path.Combine(customOrientationDestinationDirectory, Config.CustomOrientation.CompileFilename());
            //File.WriteAllLines(customOrientationFilepath, roomOrientationTextAsset.text.Split('\n'));
            File.WriteAllLines(customOrientationFilepath, roomOrientationTextAsset.text.Split(new string[1] { "\r\n" }, System.StringSplitOptions.None));
            string customMeshesFilepath = Path.Combine(customMeshesDestinationDirectory, Config.CustomMesh.CompileFilename());
            //File.WriteAllLines(customMeshesFilepath, roomMeshesTextAsset.text.Split('\n'));
            File.WriteAllLines(customMeshesFilepath, roomMeshesTextAsset.text.Split(new string[1] { "\r\n" }, System.StringSplitOptions.None));

            // Extract textures
            Texture2D[] rawBundledTexArray = roomTextureBundle.LoadAllAssets<Texture2D>();
            for (int i = 0; i < rawBundledTexArray.Length; i++)
            {
                string textureImageFilepath = Path.Combine(textureImagesDestinationDirectory, Config.Images.CompileFilename(Config.Images.GetIndex(rawBundledTexArray[i].name)));
                //File.WriteAllBytes(Config.Images.CompileAbsoluteAssetPath(Config.Images.CompileFilename(Config.Images.GetIndex(rawBundledTexArray[i].name))), rawBundledTexArray[i].EncodeToPNG());
                File.WriteAllBytes(textureImageFilepath, rawBundledTexArray[i].EncodeToPNG());
            }

            // Unload asset bundle so future loads will not fail
            roomTextureBundle.Unload(true);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void UnpackRawResourceTextureBundles(string[] roomNames)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                UnpackRawResourceTextureBundle(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName));
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void UnpackAllRawResourceTextureBundles()
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            UnpackRawResourceTextureBundles(roomNames);
        }

        //public static void UnpackRawResourceTextureBundle(string roomName)
        //{
        //    string bundlePath = Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName);
        //    string customMatricesDestinationDirectory = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
        //    string customOrientationDestinationDirectory = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
        //    string customMeshesDestinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
        //    string imagesDestinationDirectory = Config.Images.CompileAbsoluteAssetDirectory(roomName);

        //    UnpackRawResourceTextureBundle(roomName, bundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, imagesDestinationDirectory);
        //}

        public static void UnpackFinalRoomTextureBundle(string bundlePath)
        {
            if (File.Exists(bundlePath)) {

                string roomName = Config.RoomObject.GameObjectName;

                string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);
                string meshesDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
                string texturesDirectory = Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName);
                AbnormalDirectoryHandler.CreateDirectoryFromFile(materialsDirectory);
                AbnormalDirectoryHandler.CreateDirectoryFromFile(meshesDirectory);
                AbnormalDirectoryHandler.CreateDirectoryFromFile(texturesDirectory);

                // Ensure that previous room items (resources & game objects) are deleted
                RemoveRoomObject(roomName);
                //RemoveRoomResources(roomName);

                AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(bundlePath);
                //TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
                TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset(Config.MatrixArray.FilenameRoot.ToLower()) as TextAsset;
                //Directory.CreateDirectory(roomMatrixPath);
                File.WriteAllText(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName), roomMatricesTextAsset.text);

                GameObject room = roomTextureBundle.LoadAsset(Config.Prefab.CompileFilename()) as GameObject;
                room = GameObject.Instantiate(room);
                room.name = Config.RoomObject.GameObjectName;
                // Destroy existing room script (Unity bug causes bad script reference? Or script HAS to be in resources?)
                GameObject.Destroy(room.GetComponent<RoomModel>());
                RoomModel roomModel = room.AddComponent<RoomModel>();

                // Grab the matrices and set them
                Matrix4x4[] worldToCameraArray;
                Matrix4x4[] projectionArray;
                Matrix4x4[] localToWorldArray;
                MatrixArray.LoadMatrixArrays_FromAssetBundle(roomMatricesTextAsset, out worldToCameraArray, out projectionArray, out localToWorldArray);
                roomModel.SetMatrixData(worldToCameraArray, projectionArray, localToWorldArray);

                //// Get the materials
                //// Update by resetting the texture arrays to them to get them to display
                //Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.FilenameWithoutExtension + Config.Texture2DArray.Extension) as Texture2DArray;
                //for(int i = 0; i < room.transform.childCount; i++)
                //{
                //    GameObject child = room.transform.GetChild(i).gameObject;
                //    child.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MyArr", texArray);
                //}

                //Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.FilenameRoot + Config.Texture2DArray.Extension) as Texture2DArray;
                Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.CompileFilename()) as Texture2DArray;
                for (int i = 0; i < room.transform.childCount; i++)
                {
                    GameObject child = room.transform.GetChild(i).gameObject;
                    // ERROR TESTING
                    //Material childMaterial = MaterialManager.GenerateRoomMaterial(i, texArray, worldToCameraArray, projectionArray, localToWorldArray[i]);
                    //child.GetComponent<MeshRenderer>().sharedMaterial = childMaterial;
                    //child.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MyArr", texArray);

                    child.GetComponent<MeshRenderer>().sharedMaterial.shader = Shader.Find(Config.Shader.QualifiedFilenameWithoutExtension);
                    child.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MyArr", texArray);

                }

                //for(int i = 0; i < room.transform.childCount; i++)
                //{
                //    GameObject child = room.transform.GetChild(i).gameObject;
                //    Material childMaterial = roomTextureBundle.LoadAsset<Material>(Config.Material.CompileFilename(i));
                //    if(childMaterial == null)
                //        UnityEngine.Debug.Log("Material not found!");
                //    child.GetComponent<MeshRenderer>().sharedMaterial = childMaterial;
                //    child.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MyArr", texArray);
                //}


                // Unload the room texture bundle to reduce memory usage
                // false indicates you are creating a COPY of the items inside 
                // the room bundle, so you can delete or remove the texture bundle 
                // and not keep it open afterwards as long as the matrices are 
                // dynamically saved
                roomTextureBundle.Unload(false);
            }
            else
            {
                throw new System.Exception("Asset bundle unload failed.");
            }
        }

        //public static void UnpackFinalRoomTextureBundle()
        //{
        //    string roomName = Config.RoomObject.GameObjectName;
        //    string bundlePath = Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename(), roomName);
        //    //string roomMatrixPath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
        //    //UnpackFinalRoomTextureBundle(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename()), roomMatrixPath);
        //    UnpackFinalRoomTextureBundle(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename(), roomName), Config.RoomObject.CompileAbsoluteAssetDirectory(roomName));
        //}

        public static void UnpackFinalRoomTextureBundles(string[] roomNames)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                UnpackFinalRoomTextureBundle(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename(), roomName));
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void UnpackAllFinalRoomTextureBundles()
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            UnpackFinalRoomTextureBundles(roomNames);
        }

        #endregion

        #region Create Resources

        public static void CreateRoomResources()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);

            if (!Config.Images.FilenameRoot.Contains(roomName))
            {
                UnityEngine.Debug.Log("Room name of \'" + roomName + "\' does not exist in image filenameRoot of \'" + Config.Images.FilenameRoot + "\'");
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
                UnityEngine.Debug.Log("Changed room name in UWB_Texturing Config file to match. BE SURE TO SWITCH BACK IF NECESSARY");

                return;
            }

            // Ensure that previous room items (resources) are deleted
            RemoveRoomResources(roomName);

            // Extract camera matricesstring matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename());
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays_AssetsStored(out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Extract textures
            Queue<Texture2D> rawTexQueue = new Queue<Texture2D>();
            //foreach(string filepath in Directory.GetFiles(Config.Images.CompileAbsoluteAssetDirectory(roomName)))

            //Debug.Log("Images directory = " + imagesDirectory);
            string imagesDirectory = Config.Images.CompileAbsoluteAssetDirectory(roomName);
            foreach (string filepath in Directory.GetFiles(imagesDirectory))
            {
                if (filepath.Contains(Config.Images.FilenameRoot) && filepath.EndsWith(Config.Images.Extension))
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.LoadImage(File.ReadAllBytes(filepath));
                    tex.name = Path.GetFileNameWithoutExtension(filepath);
                    rawTexQueue.Enqueue(tex);
                }
            }
            Texture2D[] rawTexArray = rawTexQueue.ToArray();
            Texture2D[] sortedTexArray = new Texture2D[rawTexArray.Length];
            for (int i = 0; i < rawTexArray.Length; i++)
            {
                int imageIndex = Config.Images.GetIndex(Path.GetFileNameWithoutExtension(rawTexArray[i].name));
                sortedTexArray[imageIndex] = rawTexArray[i];
            }

            if (sortedTexArray == null)
            {
                Debug.Log("Null tex array");
            }
            else
            {
                Debug.Log("Bundled tex array size = " + sortedTexArray.Length);
            }

            Debug.Log("About to create texture array");

            // Create Texture2DArray
            Texture2DArray TextureArray = new Texture2DArray(sortedTexArray[0].width, sortedTexArray[0].height, sortedTexArray.Length, sortedTexArray[0].format, false);
            if (WorldToCameraMatrixArray != null
                && ProjectionMatrixArray != null
                && LocalToWorldMatrixArray != null
                && sortedTexArray != null
                && TextureArray != null)
            {
                // Copy textures into texture2Darray
                for (int i = 0; i < sortedTexArray.Length; i++)
                {
                    Graphics.CopyTexture(sortedTexArray[i], 0, 0, TextureArray, i, 0);
                }

#if UNITY_EDITOR
                if (!Directory.Exists(Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName)))
                {
                    //Directory.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory());
                    AbnormalDirectoryHandler.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName));
                }

                // Save Texture2DArray as asset if appropriate
                AssetDatabase.CreateAsset(TextureArray, Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename(), roomName));
                //AssetDatabase.CreateAsset(TextureArray, texturesDirectory);
                AssetDatabase.SaveAssets();
#endif

                // Extract room mesh
                CustomMesh.LoadMesh(Config.CustomMesh.CompileAbsoluteAssetPath(Config.CustomMesh.CompileFilename(), roomName));
                //CustomMesh.LoadMesh(Path.Combine(meshesDirectory, Config.CustomMesh.CompileFilename()), meshesDirectory, roomName);

                //Debug.Log("Mesh load path = " + Path.Combine(meshesDirectory, Config.CustomMesh.CompileFilename()));

                Debug.Log("Length of cam array = " + WorldToCameraMatrixArray.Length);
                Debug.Log("Length of proj array = " + ProjectionMatrixArray.Length);
                Debug.Log("Length of Local array = " + LocalToWorldMatrixArray.Length);
                Debug.Log("Length of texture array = " + TextureArray.depth);

                // Generate materials
                MaterialManager.GenerateRoomMaterials(TextureArray, WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray);
            }
            else
            {
                if (WorldToCameraMatrixArray == null)
                {
                    UnityEngine.Debug.Log("WorldToCameraMatrixArray is null");
                }
                if (LocalToWorldMatrixArray == null)
                {
                    UnityEngine.Debug.Log("LocalToWorldMatrixArray is null");
                }
                if (ProjectionMatrixArray == null)
                {
                    UnityEngine.Debug.Log("ProjectionMatrixArray is null");
                }
                if (sortedTexArray == null)
                {
                    UnityEngine.Debug.Log("SortedTexArray is null");
                }
                if (TextureArray == null)
                {
                    UnityEngine.Debug.Log("TextureArray is null");
                }
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        #endregion

        #region Remove/Delete

        public static void RemoveRoomObject(string roomName)
        {
            GameObject room = GameObject.Find(roomName);
            if (room != null)
            {
                GameObject.DestroyImmediate(room);
            }
        }

        public static void RemoveRoomObjects(string[] roomNames)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                RemoveRoomObject(roomName);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void RemoveAllRoomObjects()
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            RemoveRoomObjects(roomNames);
        }

        // Assumes all resources sit in a subfolder in the resources folder
        public static void RemoveRoomResources(string roomName)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;
            Config.RoomObject.GameObjectName = roomName;

            // Remove materials
            string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(materialsDirectory))
            {
                string[] files = Directory.GetFiles(materialsDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Material.FilenameRoot)
                        && files[i].Contains(Config.Material.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove meshes
            //string customMeshDirectory = Config.UnityMeshes.AbsoluteAssetRootFolder + '/' + Config.CustomMesh.AssetSubFolder;
            string customMeshDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(customMeshDirectory))
            {
                string[] files = Directory.GetFiles(customMeshDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.UnityMeshes.FilenameRoot)
                        && files[i].Contains(Config.UnityMeshes.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove Texture2DArray
            string textureArrayDirectory = Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(textureArrayDirectory))
            {
                string[] files = Directory.GetFiles(textureArrayDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Texture2DArray.FilenameRoot)
                        && files[i].Contains(Config.Texture2DArray.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void RemoveRoomResources(string[] roomNames)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                RemoveRoomResources(roomName);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void RemoveAllRoomResources()
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            RemoveRoomResources(roomNames);
        }

        public static void RemoveRawInfo(string roomName)
        {
            // Remove images
            string imageFolder = Config.Images.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(imageFolder))
            {
                string[] files = Directory.GetFiles(imageFolder);
                for(int i = 0; i < files.Length; i++)
                {
                    if(files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove text assets
            string matrixFileFolder = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(matrixFileFolder))
            {
                string[] files = Directory.GetFiles(matrixFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            string meshFileFolder = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(meshFileFolder))
            {
                string[] files = Directory.GetFiles(meshFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            string orientationFileFolder = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(orientationFileFolder))
            {
                string[] files = Directory.GetFiles(orientationFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
        }

        public static void RemoveRawInfo(string[] roomNames)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach (string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                RemoveRawInfo(roomName);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void RemoveAllRawInfo()
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            RemoveRawInfo(roomNames);
        }
#endregion
#endregion
    }
}