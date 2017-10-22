using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UWB_Texturing
{
    public static class CustomMesh
    {
        /// <summary>
        /// Sub-class used to hold constant strings for exception messages, 
        /// debug messages, and user messages.
        /// </summary>
        public static class Messages
        {
            /// <summary>
            /// Use when mesh array passed in from other source is invalid for use.
            /// </summary>
            public static string InvalidMeshArray = "Invalid mesh array. Mesh was null or had zero entries.";
            public static string InvalidMesh = "Invalid mesh found. Mesh was null.";
            public static string MeshFileNotFound = "Mesh file does not exist at given path!";
        }

        #region Constants
        public static string Separator = "\n";
        public static string MeshID = "o ";
        public static string VertexID = "v ";
        public static string VertexNormalID = "vn ";
        public static string TriangleID = "f ";

        public static string SupplementaryInfoSeparator = "===";
        public static string PositionID = "MeshPositions";
        public static string RotationID = "MeshRotations";
        #endregion

        #region Methods
        #region Main Methods

        #region Save Mesh
        public static void SaveMesh(Mesh mesh)
        {
            if (mesh != null)
            {
                string roomName = Config.RoomObject.GameObjectName;
                string meshString = WriteMeshString(mesh);
                string meshFilePath = Config.CustomMesh.CompileAbsoluteAssetPath(Config.CustomMesh.CompileFilename(), roomName);
                //Directory.CreateDirectory(meshFilePath);
                AbnormalDirectoryHandler.CreateDirectoryFromFile(meshFilePath);
                File.WriteAllText(meshFilePath, meshString);
            }
            else
            {
                Debug.Log(Messages.InvalidMesh);
            }
        }
        
        /// <summary>
        /// Iterates through an array of meshes to generate a text file 
        /// representing this mesh.
        /// </summary>
        /// <param name="meshes">
        /// A non-null array of meshes with vertices and triangles.
        /// </param>
        public static void SaveMesh(Mesh[] meshes)
        {
            if (meshes != null && meshes.Length > 0)
            {
                string roomName = Config.RoomObject.GameObjectName;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < meshes.Length; i++)
                {
                    sb.Append(WriteMeshString(meshes[i], i));
                }

                File.WriteAllText(Config.CustomMesh.CompileAbsoluteAssetPath(Config.CustomMesh.CompileFilename(), roomName), sb.ToString());
            }
            else
            {
                Debug.Log(Messages.InvalidMeshArray);
            }
        }

        public static string WriteMeshString(Mesh mesh)
        {
            return WriteMeshString(mesh, 0);
        }

        /// <summary>
        /// Logic for writing a mesh to a string (i.e. for writing to a text file for transferral of meshes).
        /// 
        /// 
        /// General structure:
        /// o MeshName
        /// v vertexX vertexY vertexZ
        /// 
        /// vn vertexNormalX vertexNormalY vertexNormalZ
        /// 
        /// f triangleVertex1//triangleVertex1 triangleVertex2//triangleVertex2 triangleVertex3//triangleVertex3
        /// 
        /// NOTE: Referenced from ExportOBJ file online @ http://wiki.unity3d.com/index.php?title=ExportOBJ
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static string WriteMeshString(Mesh mesh, int meshIndexSuffix)
        {
            if (mesh != null)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(CustomMesh.Separator);
                sb.Append(MeshID + Config.UnityMeshes.CompileMeshName(meshIndexSuffix) + '\n');
                foreach (Vector3 vertex in mesh.vertices)
                {
                    sb.Append(GetVertexString(vertex));
                }
                sb.Append(Separator);
                foreach (Vector3 normal in mesh.normals)
                {
                    sb.Append(GetVertexNormalString(normal));
                }
                sb.Append(Separator);
                // Append triangles (i.e. which vertices each triangle uses)
                for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                {
                    sb.Append(Separator);
                    sb.Append("submesh" + submesh + "\n");
                    int[] triangles = mesh.GetTriangles(submesh);
                    for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex += 3)
                    {
                        sb.Append(string.Format("f {0}//{0} {1}//{1} {2}//{2}\n", triangles[triangleIndex], triangles[triangleIndex + 1], triangles[triangleIndex + 2]));
                    }
                }

                return sb.ToString();
            }
            else
            {
                Debug.Log(Messages.InvalidMesh);
                return "";
            }
        }
        
        /// <summary>
        /// Turns a vertex Vector3 into a readable vertex string of "v vertexX 
        /// vertexY vertexZ".
        /// </summary>
        /// <param name="vertex">
        /// A Vector3 representing a mesh vertex.
        /// </param>
        /// <returns>
        /// A readable string representing the vertex's entry in the mesh text 
        /// file.
        /// </returns>
        public static string GetVertexString(Vector3 vertex)
        {
            string vertexString = "";
            vertexString += VertexID;
            vertexString += vertex.x.ToString() + ' ';
            vertexString += vertex.y.ToString() + ' ';
            vertexString += vertex.z.ToString() + '\n';

            return vertexString;
        }

        /// <summary>
        /// Turns a vertexNormal Vector3 into a readable vertexNormal string of
        /// "vn vertexNormalX vertexNormalY vertexNormalZ".
        /// </summary>
        /// <param name="vertexNormal">
        /// A Vector3 representing a vertex normal (the normal pointing away 
        /// from the vertex that is calculated by the mesh when instantiated).
        /// </param>
        /// <returns>
        /// A readable string representing the vertex's entry in the mesh text file.
        /// </returns>
        public static string GetVertexNormalString(Vector3 vertexNormal)
        {
            string vertexNormalString = "";
            vertexNormalString += VertexNormalID;
            vertexNormalString += vertexNormal.x.ToString() + ' ';
            vertexNormalString += vertexNormal.y.ToString() + ' ';
            vertexNormalString += vertexNormal.z.ToString() + '\n';

            return vertexNormalString;
        }

        #endregion

        #region Load Mesh
        public static void LoadMesh(string filepath)
        {
            if (File.Exists(filepath))
            {
                string roomName = Config.RoomObject.GameObjectName;
                Debug.Log("Filepath for loading meshes = " + filepath);

                LoadMesh(File.ReadAllLines(filepath));
            }
            else
            {
                Debug.Log(Messages.MeshFileNotFound);
            }
        }

        public static void LoadMesh(TextAsset meshAsset)
        {
            LoadMesh(SplitTextAsset(meshAsset));
        }

        public static void LoadMesh(string[] fileContents)
        {
            string roomName = Config.RoomObject.GameObjectName;
            string destinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);

            // ID the markers at the beginning of each line
            string meshID = MeshID.TrimEnd();
            string vertexID = VertexID.TrimEnd();
            string vertexNormalID = VertexNormalID.TrimEnd();
            string triangleID = TriangleID.TrimEnd();

            // Initialize items used while reading the file
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();
            Mesh m = new Mesh();
            bool MeshRead = false;
            
            int lineIndex = 0;
            int meshIndex = 0;
            while (lineIndex < fileContents.Length)
            {
                string line = fileContents[lineIndex].Trim();
                string[] lineContents = line.Split(' ');
                if (lineContents.Length == 0)
                {
                    // Ignore blank lines
                    continue;
                }

                // ID the marker telling you what info the line contains
                string marker = lineContents[0];

                // marker = "o"
                if (marker.Equals(meshID))
                {
                    // Demarcates a new mesh object -> create a new mesh to store info
                    m = new Mesh();
                    m.name = lineContents[1];
                }
                // marker = "v"
                else if (marker.Equals(vertexID))
                {
                    // IDs a vertex to read in
                    Vector3 vertex = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
                    vertices.Add(vertex);
                }
                // marker = "vn"
                else if (marker.Equals(vertexNormalID))
                {
                    // IDs a vertex normal to read in
                    Vector3 normal = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
                    normals.Add(normal);
                }
                // marker = "f"
                else if (marker.Equals(triangleID))
                {
                    // IDs a set of vertices that make up a triangle
                    do
                    {
                        triangles.Add(int.Parse(lineContents[1].Split('/')[0]));
                        triangles.Add(int.Parse(lineContents[2].Split('/')[0]));
                        triangles.Add(int.Parse(lineContents[3].Split('/')[0]));

                        // Reset variables
                        ++lineIndex;
                        if (lineIndex < fileContents.Length)
                        {
                            line = fileContents[lineIndex];
                            lineContents = line.Split(' ');
                            marker = lineContents[0];
                        }
                        else
                        {
                            marker = "";
                        }
                    } while (marker.Contains(triangleID));
                    --lineIndex;

                    MeshRead = true;
                }

                ++lineIndex;
                if (MeshRead)
                {
                    // Set appropriate values
                    m.SetVertices(vertices);
                    vertices = new List<Vector3>();
                    m.SetNormals(normals);
                    normals = new List<Vector3>();
                    m.SetTriangles(triangles.ToArray(), 0);
                    triangles = new List<int>();
                    m.RecalculateBounds();
                    m.RecalculateNormals();

#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(m, Config.UnityMeshes.CompileUnityAssetPath(Config.UnityMeshes.CompileFilename(meshIndex), roomName));
                    //string destinationDirectory_Unity = string.Join("/", destinationDirectory.Remove(0, destinationDirectory.IndexOf("Assets")).Split('\\'));
                    //string destinationFilepath_Unity = destinationDirectory_Unity + '/' + Config.UnityMeshes.CompileFilename(meshIndex);
                    //AssetDatabase.CreateAsset(m, destinationFilepath_Unity);
                    // ERROR TESTING - REMOVE // AssetDatabase.SaveAssets();
#endif
                    MeshRead = false;
                    ++meshIndex;
                }
            }

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        
        #endregion

//        /// <summary>
//        /// Logic to load a mesh from a text file representing a mesh and submeshes.
//        /// All meshes are turned into standalone meshes that are then parented by a 
//        /// newly instantiated GameObject. Names the parent object "RoomMesh".
//        /// Assumes that all meshes are demarcated by the MeshID for the beginning line
//        /// and terminated by the end of the last TriangleID line.
//        /// </summary>
//        /// <param name="meshSupplementaryInfoTextAsset"></param>
//        /// <returns></returns>
//        public static GameObject InstantiateRoomObject(TextAsset roomMeshTextAsset, TextAsset meshSupplementaryInfoTextAsset, bool saveAsAsset)
//        {
//            string roomName = Config.RoomObject.GameObjectName;

//            if (!Directory.Exists(Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName)))
//            {
//                //Directory.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory());
//                AbnormalDirectoryHandler.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName));
//            }

//            Vector3[] positionArray;
//            Quaternion[] rotationArray;

//            // Load up the information stored from mesh supplementary information 
//            // to correct for Hololens mesh translation and orientation.
//            string[] orientationFileLines = SplitTextAsset(meshSupplementaryInfoTextAsset);
//            CustomOrientation.Load(orientationFileLines, out positionArray, out rotationArray);

//            // ID the markers at the beginning of each line
//            string meshID = MeshID.TrimEnd();
//            string vertexID = VertexID.TrimEnd();
//            string vertexNormalID = VertexNormalID.TrimEnd();
//            string triangleID = TriangleID.TrimEnd();

//            // Initialize items used while reading the file
//            Queue<GameObject> meshObjects = new Queue<GameObject>();
//            GameObject mesh = new GameObject();
//            List<Vector3> vertices = new List<Vector3>();
//            List<Vector3> normals = new List<Vector3>();
//            List<int> triangles = new List<int>();
//            bool MeshRead = false;

//            //string[] fileContents = File.ReadAllLines(OutputFilepath);
//            string[] fileContents = SplitTextAsset(roomMeshTextAsset);
//            int lineIndex = 0;
//            int meshCount = 0;
//            while (lineIndex < fileContents.Length)
//            {
//                string line = fileContents[lineIndex].Trim();
//                string[] lineContents = line.Split(' ');
//                if (lineContents.Length == 0)
//                {
//                    // Ignore blank lines
//                    continue;
//                }

//                // ID the marker telling you what info the line contains
//                string marker = lineContents[0];

//                // marker = "o"
//                if (marker.Equals(meshID))
//                {
//                    // Demarcates a new mesh object -> create a new mesh to store info
//                    GameObject.DestroyImmediate(mesh);
//                    mesh = new GameObject();
//                    mesh.name = lineContents[1];
//                }
//                // marker = "v"
//                else if (marker.Equals(vertexID))
//                {
//                    // IDs a vertex to read in
//                    Vector3 vertex = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
//                    vertices.Add(vertex);
//                }
//                // marker = "vn"
//                else if (marker.Equals(vertexNormalID))
//                {
//                    // IDs a vertex normal to read in
//                    Vector3 normal = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
//                    normals.Add(normal);
//                }
//                // marker = "f"
//                else if (marker.Equals(triangleID))
//                {
//                    // IDs a set of vertices that make up a triangle
//                    do
//                    {
//                        triangles.Add(int.Parse(lineContents[1].Split('/')[0]));
//                        triangles.Add(int.Parse(lineContents[2].Split('/')[0]));
//                        triangles.Add(int.Parse(lineContents[3].Split('/')[0]));

//                        // Reset variables
//                        ++lineIndex;
//                        if (lineIndex < fileContents.Length)
//                        {
//                            line = fileContents[lineIndex];
//                            lineContents = line.Split(' ');
//                            marker = lineContents[0];
//                        }
//                        else
//                        {
//                            marker = "";
//                        }
//                    } while (marker.Contains(triangleID));
//                    --lineIndex;

//                    MeshRead = true;
//                }

//                ++lineIndex;
//                if (MeshRead)
//                {
//                    // If the triangle list has been fully read, that means you 
//                    // have all the info you need to form the mesh.
//                    if (positionArray != null)
//                    {
//                        mesh.transform.position = positionArray[meshCount];
//                    }
//                    if (rotationArray != null)
//                    {
//                        mesh.transform.rotation = rotationArray[meshCount];
//                    }

//                    // Add neccessary components to the mesh-containing gameobject
//                    mesh.AddComponent<MeshFilter>();
//                    mesh.GetComponent<MeshFilter>().sharedMesh = new Mesh();
//                    Mesh m = mesh.GetComponent<MeshFilter>().sharedMesh;

//                    // Set appropriate values
//                    m.SetVertices(vertices);
//                    vertices = new List<Vector3>();
//                    m.SetNormals(normals);
//                    normals = new List<Vector3>();
//                    m.SetTriangles(triangles.ToArray(), 0);
//                    triangles = new List<int>();
//                    mesh.AddComponent<MeshRenderer>();
//                    m.RecalculateBounds();
//                    m.RecalculateNormals();
//                    m.name = mesh.name;

//#if UNITY_EDITOR
//                    // Save as asset if applicable
//                    if (saveAsAsset)
//                    {
//                        AssetDatabase.CreateAsset(m, Config.CustomMesh.CompileUnityAssetPath(m.name, roomName));
//                        AssetDatabase.SaveAssets();
//                    }
//#endif

//                    // Push them into the queue of meshes to be parented by the 
//                    // final room mesh parent game object
//                    meshObjects.Enqueue(mesh);
//                    mesh = new GameObject();
//                    MeshRead = false;
//                    ++meshCount;
//                }
//            }

//            // Assign all submeshes to a parent mesh object
//            while (meshObjects.Count > 0)
//            {
//                // Mesh should be an empty new GameObject by this point
//                meshObjects.Dequeue().transform.parent = mesh.transform;
//            }
//            mesh.name = Config.RoomObject.GameObjectName;

//#if UNITY_EDITOR
//            if (saveAsAsset)
//                AssetDatabase.Refresh();
//#endif

//            return mesh;
//        }
        
        public static Mesh InstantiateMesh(List<Vector3> vertices, int[] triangles)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        public static Mesh InstantiateMesh(List<Vector3> vertices, int[] triangles, List<Vector3> normals)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetNormals(normals);
            mesh.RecalculateBounds();

            return mesh;
        }
        #endregion

        #region Helper Functions

        public static int GetNumMeshes(string meshDirectory)
        {
            int numMeshes = 0;
            string[] files = Directory.GetFiles(meshDirectory);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(Config.CustomMesh.FilenameRoot))
                {
                    ++numMeshes;
                }
            }

            return numMeshes;
        }

        /// <summary>
        /// Create a readable string representing the components of a Vector3. 
        /// Takes the format of "vectorX vectorY vectorZ" in floats.
        /// </summary>
        /// <param name="vec">
        /// The vector to create a readable string from.
        /// </param>
        /// <returns>
        /// A readable string representing the components of a Vector3.
        /// </returns>
        private static string Vector3ToString(Vector3 vec)
        {
            string vecString = vec.x.ToString() + ' ' + vec.y.ToString() + ' ' + vec.z.ToString() + '\n';

            return vecString;
        }

        /// <summary>
        /// Create a readable string representing the components of a Quaternion.
        /// Takes the format of "xAxis yAxis zAxis rotationDegrees".
        /// </summary>
        /// <param name="q">
        /// The quaternion to create a readable string from.
        /// </param>
        /// <returns>
        /// A readable string representing the components of a Quaternion.
        /// </returns>
        private static string QuaternionToString(Quaternion q)
        {
            string qString = q.x.ToString() + ' ' + q.y.ToString() + ' ' + q.z.ToString() + ' ' + q.w.ToString() + '\n';

            return qString;
        }

        /// <summary>
        /// Splits the contents of a TextAsset by the newline character.
        /// </summary>
        /// <param name="textAsset">
        /// The TextAsset to be split.
        /// </param>
        /// <returns>
        /// An array of strings (i.e. the lines) of the text asset.
        /// </returns>
        private static string[] SplitTextAsset(TextAsset textAsset)
        {
            return textAsset.text.Split('\n');
        }
        #endregion
        #endregion
    }
}