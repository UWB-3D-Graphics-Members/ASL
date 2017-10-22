using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UWB_Texturing
{
    /// <summary>
    /// Handles the generation of materials for the room mesh object.
    /// </summary>
    public static class MaterialManager
    {
//        /// <summary>
//        /// Generates array of room materials to be assigned to room mesh objects.
//        /// Each material will be associated with a specific mesh, since each mesh
//        /// has its own localToWorld matrix.
//        /// </summary>
//        /// <param name="roomMaterials">
//        /// Material array to hold the generated room mesh materials.
//        /// </param>
//        /// <param name="texArray">
//        /// Texture2DArray to apply to each material.
//        /// </param>
//        /// <param name="worldToCameraMatrixArray">
//        /// Array of matrices describing translation of vertices to change 
//        /// vertex world space coordinates to camera/view space.
//        /// </param>
//        /// <param name="projectionMatrixArray">
//        /// Array of matrices describing translation of camera/view space
//        /// vertex coordinates to clip space.
//        /// </param>
//        /// <param name="localToWorldMatrixArray">
//        /// Array of matrices describing translation of vertices from
//        /// local space to world space. Each array is specific to a
//        /// single generated mesh and cannot be applied to meshes in general.
//        /// </param>
//        public static void GenerateRoomMaterials(out Material[] roomMaterials, Texture2DArray texArray, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray, bool saveAsAssets)
//        {
//            roomMaterials = new Material[localToWorldMatrixArray.Length];
//            for(int i = 0; i < roomMaterials.Length; i++)
//            {
//                string materialNameSuffix = "_" + i;
//                roomMaterials[i] = GenerateRoomMaterial(texArray, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray[i]);
//                roomMaterials[i].name = roomMaterials[i].name + materialNameSuffix;

//#if UNITY_EDITOR
//                // Save as assets if applicable
//                if (saveAsAssets)
//                {
//                    AssetDatabase.CreateAsset(roomMaterials[i], CrossPlatformNames.Material.CompileAssetPath(roomMaterials[i].name));
//                    AssetDatabase.SaveAssets();
//                }
//#endif
//            }

//#if UNITY_EDITOR
//            AssetDatabase.Refresh();
//#endif
//        }

        public static void GenerateRoomMaterials(Texture2DArray texArray, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray)
        {
            string roomName = Config.RoomObject.GameObjectName;
            string destinationDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);

            //if (!Directory.Exists(Config.Material.CompileAbsoluteAssetDirectory(roomName)))
            if(!Directory.Exists(destinationDirectory))
            {
                //Directory.CreateDirectory(Config.Material.CompileAbsoluteAssetDirectory());
                //AbnormalDirectoryHandler.CreateDirectory(Config.Material.CompileAbsoluteAssetDirectory(roomName));
                AbnormalDirectoryHandler.CreateDirectory(destinationDirectory);
            }
            
            int numMaterials = localToWorldMatrixArray.Length;
            
            Debug.Log("Num of materials = " + numMaterials);
            for(int i = 0; i < numMaterials; i++)
            {
                Material roomMat = GenerateRoomMaterial(i, texArray, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray[i]);

                Debug.Log("Material created!");

#if UNITY_EDITOR
                //AssetDatabase.CreateAsset(roomMat, Config.Material.CompileUnityAssetPath(Config.Material.CompileFilename(i), roomName));
                string baseDirectory = Config_Base.AbsoluteAssetRootFolder;
                
                Debug.Log("Asset root folder = " + baseDirectory);

                string destinationDirectory_Unity = destinationDirectory.Remove(0, baseDirectory.Length - 6);
                //AssetDatabase.CreateAsset(roomMat, Path.Combine(destinationDirectory_Unity, Config.Material.CompileFilename(i)));
                AssetDatabase.CreateAsset(roomMat, Path.Combine(destinationDirectory_Unity, Config.Material.CompileFilename(i)));
                AssetDatabase.SaveAssets();
                Debug.Log("Material generated in project folder!");
#endif
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// Generate a single material for a specific room mesh.
        /// </summary>
        /// <param name="texArray">
        /// Texture2DArray to set for the material.
        /// </param>
        /// <param name="worldToCameraMatrixArray">
        /// Array of matrices describing translation of vertices to change 
        /// vertex world space coordinates to camera/view space.
        /// </param>
        /// <param name="projectionMatrixArray">
        /// Array of matrices describing translation of camera/view space 
        /// vertex coordinates to clip space.
        /// </param>
        /// <param name="localToWorldMatrix">
        /// Array of matrices describing translation of vertices from
        /// local space to world space. Each array is specific to a
        /// single generated mesh and cannot be applied to meshes in general.
        /// </param>
        /// <returns>
        /// The material generated using the passed-in parameters.
        /// </returns>
        public static Material GenerateRoomMaterial(int roomChildIndex, Texture2DArray texArray, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4 localToWorldMatrix)
        {
            Material mat = new Material(Shader.Find(Config.Shader.QualifiedFilenameWithoutExtension));

            Debug.debugging = true;
            if(Shader.Find(Config.Shader.QualifiedFilenameWithoutExtension) == null)
            {
                Debug.Log("Shader not found!");
            }
            else
            {
                Debug.Log("Shader found with name " + Config.Shader.QualifiedFilenameWithoutExtension);
            }
            Debug.debugging = false;

            mat.name = Config.Material.CompileMaterialName(roomChildIndex);
            SetShaderParams(mat, texArray, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrix);

            return mat;
        }

        public static void SetShaderParams(Material mat, Texture2DArray texArray, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4 localToWorldMatrix)
        {
            mat.SetTexture("_MyArr", texArray);
            mat.SetMatrixArray("_WorldToCameraMatrixArray", worldToCameraMatrixArray);
            mat.SetMatrixArray("_CameraProjectionMatrixArray", projectionMatrixArray);
            mat.SetMatrix("_MyObjectToWorld", localToWorldMatrix);
        }

        public static void RefreshShaderMatrices(Material mat, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4 localToWorldMatrix)
        {
            mat.SetMatrixArray("_WorldToCameraMatrixArray", worldToCameraMatrixArray);
            mat.SetMatrixArray("_CameraProjectionMatrixArray", projectionMatrixArray);
            mat.SetMatrix("_MyObjectToWorld", localToWorldMatrix);
        }

        public static Material GetRoomMaterial(string roomName, int index)
        {
            //Material roomMat = Resources.Load(CrossPlatformNames.Material.CompileAssetPath(CrossPlatformNames.Material.CompileAssetName(index))) as Material;
            string assetName = Config.Material.CompileMaterialName(index);

            Material roomMat = Resources.Load(Config.Material.CompileResourcesLoadPath(assetName, roomName)) as Material;

            return roomMat;
        }


        /// <summary>
        /// Grabs a list of the raw, unpacked texture files stored at the
        /// appropriate folder. This will not grab the textures bundled
        /// in an asset bundle, but is meant to be used during bundling
        /// logic.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTextureFiles(string roomName)
        {
            List<string> textureFilenameList = new List<string>();

            foreach (string filepath in Directory.GetFiles(Config.Images.CompileUnityAssetDirectory(roomName)))
            {
                if (Path.GetExtension(filepath).Equals(Config.Images.Extension))
                {
                    textureFilenameList.Add(Path.GetFileName(filepath));
                }
            }

            return textureFilenameList;
        }

        /// <summary>
        /// Helper function to count the number of raw texture files 
        /// stored at the appropriate folder. This will not show the number
        /// of textures loaded into an asset bundle, but is meant to be
        /// used during bundling logic.
        /// </summary>
        /// <returns></returns>
        public static int GetNumTextures(string roomName)
        {
            return GetTextureFiles(roomName).Count;
        }

        public static int GetNumMaterials(string roomName)
        {
            int numMaterials = 0;
            string[] files = Directory.GetFiles(Config.Material.CompileAbsoluteAssetDirectory(roomName));
            //string[] files = Directory.GetFiles(materialDirectory);
            for(int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(Config.Material.FilenameRoot)
                    && !files[i].Contains("meta"))
                {
                    ++numMaterials;
                }
            }

            return numMaterials;
        }
    }
}