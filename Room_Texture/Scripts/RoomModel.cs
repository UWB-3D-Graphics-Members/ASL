using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Encapsulates behavior interacting with the RoomMesh object and settings 
    /// its shader parameters. Expected to be attached to the parent room model 
    /// game object.
    /// </summary>
    public class RoomModel : MonoBehaviour
    {
        #region Fields
        private Matrix4x4[] worldToCameraMatrixArray;
        private Matrix4x4[] projectionMatrixArray;
        private Matrix4x4[] localToWorldMatrixArray;
        //public static Texture2DArray TextureArray;
        //private static Material[] MeshMaterials;
        #endregion

        #region Methods

        void Start()
        {
            string originalRoomName = Config.RoomObject.GameObjectName;


            //string roomName = Config.RoomObject.GameObjectName;
            string roomName = gameObject.name;
            Config.RoomObject.GameObjectName = roomName;
            // Deep copy matrix data
            string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
            
            Debug.Log("Matrix array filepath = " + matrixArrayFilepath);
            
            if (File.Exists(matrixArrayFilepath))
            {
                GetMatrixData(roomName, matrixArrayFilepath, out worldToCameraMatrixArray, out projectionMatrixArray, out localToWorldMatrixArray);
            }

            // Start the refresh cycle for the matrix data
            BeginShaderRefreshCycle(Config.RoomObject.RecommendedShaderRefreshTime);

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public void SetMatrixData(Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray)
        {
            this.worldToCameraMatrixArray = worldToCameraMatrixArray;
            this.projectionMatrixArray = projectionMatrixArray;
            this.localToWorldMatrixArray = localToWorldMatrixArray;
        }

        public void GetMatrixData(string roomName, string matrixArrayFilepath, out Matrix4x4[] worldToCameraMatrixArray, out Matrix4x4[] projectionMatrixArray, out Matrix4x4[] localToWorldMatrixArray)
        {
            // Try loading from file
            bool loaded = MatrixArray.LoadMatrixArrays_AssetsStored(out worldToCameraMatrixArray, out projectionMatrixArray, out localToWorldMatrixArray);
            // If file not available, try loading from asset bundle
            if (!loaded)
            {
                // NOTE: If running into an error with the asset bundle here, you MUST DELETE THE BUNDLE, and rebundle it so the open reference closes
                AssetBundle bundle = AssetBundle.LoadFromFile(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename(), roomName));
                TextAsset matrixAsset = bundle.LoadAsset<TextAsset>("RoomMatrices".ToLower());
                MatrixArray.LoadMatrixArrays_FromAssetBundle(matrixAsset, out worldToCameraMatrixArray, out projectionMatrixArray, out localToWorldMatrixArray);
                loaded = true;
            }
            else
            {
                Debug.Log("RoomModel matrix data loaded from stored assets.");
            }
        }

        public void BeginShaderRefreshCycle(float refreshTime)
        {
            InvokeRepeating("RefreshShader", 0.0f, refreshTime);
        }

        public void RefreshShader()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();
                if (childMeshRenderer != null)
                {
                    Material childMaterial = childMeshRenderer.sharedMaterial;
                    if (childMaterial != null)
                    {
                        MaterialManager.RefreshShaderMatrices(childMaterial, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray[i]);
                        //ERROR TESTING - REMOVE // childMeshRenderer.sharedMaterial = childMaterial;
                    }
                }
            }
        }

        public static GameObject BuildRoomObject(string[] orientationFileLines)
        {
            string roomName = Config.RoomObject.GameObjectName;

            Debug.Log("Building room");
            Debug.Log("Room name = " + roomName);
            //Debug.Log("Unity meshes relative directory = " + unityMeshesRelativeDirectory);
            //Debug.Log("Materials relative directory = " + materialsRelativeDirectory);
            Debug.Log("Previous directory = " + Config.UnityMeshes.AssetSubFolder);

            if (!Directory.Exists(Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName)))
            {
                //Directory.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory());
                AbnormalDirectoryHandler.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName));
            }

            Vector3[] positionArray;
            Quaternion[] rotationArray;

            // Load up the information stored from mesh supplementary information 
            // to correct for Hololens mesh translation and orientation.
            CustomOrientation.Load(orientationFileLines, out positionArray, out rotationArray);

            GameObject roomObject = new GameObject();
            //roomObject.name = Config.RoomObject.GameObjectName;
            roomObject.name = roomName;
            for (int i = 0; i < positionArray.Length; i++)
            {
                GameObject child = new GameObject();
                
                Debug.Log("Mesh resource load path = " + Config.UnityMeshes.CompileResourcesLoadPath(Config.UnityMeshes.CompileMeshName(i), roomName));
                //Debug.Log("mesh resource load path = " + Config.UnityMeshes.CompileResourcesLoadPath(unityMeshesRelativeDirectory, Config.UnityMeshes.CompileMeshName(i)));
                
                // Set mesh
                Mesh childMesh = Resources.Load(Config.UnityMeshes.CompileResourcesLoadPath(Config.UnityMeshes.CompileMeshName(i), roomName)) as Mesh;
                //Mesh childMesh = Resources.Load(Config.UnityMeshes.CompileResourcesLoadPath(unityMeshesRelativeDirectory, Config.UnityMeshes.CompileMeshName(i))) as Mesh;
                MeshFilter mf = child.AddComponent<MeshFilter>();
                mf.sharedMesh = childMesh;

                // Set material
                MeshRenderer mr = child.AddComponent<MeshRenderer>();
                mr.sharedMaterial = Resources.Load<Material>(Config.Material.CompileResourcesLoadPath(Config.Material.CompileMaterialName(i), roomName));
                //mr.sharedMaterial = Resources.Load<Material>(Config.Material.CompileResourcesLoadPath(materialsRelativeDirectory, Config.Material.CompileMaterialName(i)));

                //Debug.Log("Meshes relative directory = " + unityMeshesRelativeDirectory);
                //Debug.Log("Meshes path = " + Config.UnityMeshes.CompileResourcesLoadPath(unityMeshesRelativeDirectory, Config.UnityMeshes.CompileMeshName(i)));
                //Debug.Log("Materials relative directory = " + materialsRelativeDirectory);
                //Debug.Log("Materials path = " + Config.Material.CompileResourcesLoadPath(materialsRelativeDirectory, Config.Material.CompileMaterialName(i)));

                // Set position and rotation
                child.transform.position = positionArray[i];
                child.transform.rotation = rotationArray[i];

                // Set name
                child.name = childMesh.name;
                child.transform.parent = roomObject.transform;

                // Add mesh collider
                MeshCollider mc = child.AddComponent<MeshCollider>();
                mc.sharedMesh = childMesh;
            }
            roomObject.AddComponent<RoomModel>();

            //// Integrate it into the UWB Network
            //roomObject.AddComponent<UWBPhotonTransformView>();

            return roomObject;
        }

        //public static GameObject BuildRoomObject(string roomName, string[] orientationFileLines, string unityMeshesRelativeDirectory, string materialsRelativeDirectory)
        //{
        //    Debug.Log("Building room");
        //    Debug.Log("Room name = " + roomName);
        //    Debug.Log("Unity meshes relative directory = " + unityMeshesRelativeDirectory);
        //    Debug.Log("Materials relative directory = " + materialsRelativeDirectory);
        //    Debug.Log("Previous directory = " + Config.UnityMeshes.AssetSubFolder);

        //    if (!Directory.Exists(Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName)))
        //    {
        //        //Directory.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory());
        //        AbnormalDirectoryHandler.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName));
        //    }

        //    Vector3[] positionArray;
        //    Quaternion[] rotationArray;
            
        //    // Load up the information stored from mesh supplementary information 
        //    // to correct for Hololens mesh translation and orientation.
        //    CustomOrientation.Load(orientationFileLines, out positionArray, out rotationArray);

        //    GameObject roomObject = new GameObject();
        //    //roomObject.name = Config.RoomObject.GameObjectName;
        //    roomObject.name = roomName;
        //    for(int i = 0; i < positionArray.Length; i++)
        //    {
        //        GameObject child = new GameObject();

        //        Debug.Log("mesh resource load path = " + Config.UnityMeshes.CompileResourcesLoadPath(unityMeshesRelativeDirectory, Config.UnityMeshes.CompileMeshName(i)));

        //        // Set mesh
        //        //Mesh childMesh = Resources.Load(Config.UnityMeshes.CompileResourcesLoadPath(Config.UnityMeshes.CompileMeshName(i))) as Mesh;
        //        Mesh childMesh = Resources.Load(Config.UnityMeshes.CompileResourcesLoadPath(unityMeshesRelativeDirectory, Config.UnityMeshes.CompileMeshName(i))) as Mesh;
        //        MeshFilter mf = child.AddComponent<MeshFilter>();
        //        mf.sharedMesh = childMesh;

        //        // Set material
        //        MeshRenderer mr = child.AddComponent<MeshRenderer>();
        //        //mr.sharedMaterial = Resources.Load<Material>(Config.Material.CompileResourcesLoadPath(Config.Material.CompileMaterialName(i)));
        //        mr.sharedMaterial = Resources.Load<Material>(Config.Material.CompileResourcesLoadPath(materialsRelativeDirectory, Config.Material.CompileMaterialName(i)));
                
        //        Debug.Log("Meshes relative directory = " + unityMeshesRelativeDirectory);
        //        Debug.Log("Meshes path = " + Config.UnityMeshes.CompileResourcesLoadPath(unityMeshesRelativeDirectory, Config.UnityMeshes.CompileMeshName(i)));
        //        Debug.Log("Materials relative directory = " + materialsRelativeDirectory);
        //        Debug.Log("Materials path = " + Config.Material.CompileResourcesLoadPath(materialsRelativeDirectory, Config.Material.CompileMaterialName(i)));
                
        //        // Set position and rotation
        //        child.transform.position = positionArray[i];
        //        child.transform.rotation = rotationArray[i];

        //        // Set name
        //        child.name = childMesh.name;
        //        child.transform.parent = roomObject.transform;

        //        // Add mesh collider
        //        MeshCollider mc = child.AddComponent<MeshCollider>();
        //        mc.sharedMesh = childMesh;
        //    }
        //    roomObject.AddComponent<RoomModel>();

        //    // Integrate it into the UWB Network
        //    roomObject.AddComponent<UWBPhotonTransformView>();
            
        //    return roomObject;
        //}
        
        #endregion
    }
}