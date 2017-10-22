using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Stores constants or logic for items that are, or could potentially be, 
    /// shared between different platforms. Also stores constants or logic 
    /// regarding items that are shared between different classes to avoid 
    /// potential dependency issues.
    /// 
    /// If you can't find a constant in a file, it's probably here.
    /// </summary>
    public static class Config
    {
        //public static string ResourcesSubFolder = "Room";
        //public static string AssetSubFolder = "Room_Texture" + '/' + "Resources" + '/' + ResourcesSubFolder;
        //public static string LocalAssetFolder = "Assets" + '/' + AssetSubFolder;
        //public static string AbsoluteAssetFolder = Path.Combine(Application.dataPath, AssetSubFolder);
        
        /// <summary>
        /// Information regarding naming convention for room texture images 
        /// passed around.
        /// </summary>
        public class Images : Config_Base
        {
            static Images()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            /// <summary>
            /// Extension for the room texture images that will be 
            /// saved/transported.
            /// </summary>
            public static string Extension = ".png";
            /// <summary>
            /// Prefix for the room texture images that will be saved/transported.
            /// </summary>
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        filenameRoot = value;
                    }
                }
            }
            private static string filenameRoot = RoomObject.GameObjectName;
            public static char Separator = '_';
            
            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName;

                    // Insert additional logic here
                }
            }


            public static int GetIndex(string name)
            {
                return int.Parse(name.Split(Separator)[1]);
            }

            /// <summary>
            /// Compile the full file name (without directories), given an image 
            /// index.
            /// </summary>
            /// <param name="imageIndex">
            /// The index of the image that will help uniquely identify it.
            /// </param>
            /// <returns>
            /// A string identifying the room texture image.
            /// </returns>
            public static string CompileFilename(int imageIndex)
            {
                return FilenameRoot + Separator + imageIndex + Extension;
            }
        }

        public class CustomMesh : Config_Base
        {
            static CustomMesh()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".txt";
            private static string filenameRoot = RoomObject.GameObjectName + "Mesh"; //"RoomMesh";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains("Mesh"))
                        {
                            filenameRoot = value + "Mesh";
                        }
                        else
                        {
                            filenameRoot = value;
                        }
                    }
                }
            }

            public static string CompileFilename()
            {
                return FilenameRoot + Extension;
            }


            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName + "Mesh";

                    // Insert additional logic here
                }
            }
        }

        public class CustomOrientation : Config_Base
        {
            static CustomOrientation()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".txt";
            private static string filenameRoot = RoomObject.GameObjectName + "Orientation"; //"RoomOrientation";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains("Orientation"))
                        {
                            filenameRoot = value + "Orientation";
                        }
                        else
                        {
                            filenameRoot = value;
                        }
                    }
                }
            }

            public static string CompileFilename()
            {
                return FilenameRoot + Extension;
            }

            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName + "Orientation";

                    // Insert additional logic here
                }
            }
        }

        public class UnityMeshes : Config_Base
        {
            static UnityMeshes()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".asset";
            public static string filenameRoot = RoomObject.GameObjectName + "Mesh";//"RoomMesh";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains("Mesh"))
                        {
                            filenameRoot = value + "Mesh";
                        }
                        else
                        {
                            filenameRoot = value;
                        }
                    }
                }
            }
            public static char Separator = '_';
            
            public static int GetIndex(string filename)
            {
                return int.Parse(filename.Split(Separator)[1]);
            }
            public static string CompileFilename(int index)
            {
                return FilenameRoot + Separator + index + Extension;
            }

            public static string CompileMeshName(int index)
            {
                return FilenameRoot + Separator + index;
            }
            
            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName + "Mesh";

                    // Insert additional logic here
                }
            }
        }

        public class Material : Config_Base
        {
            static Material()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".mat";
            /// <summary>
            /// Name of the material that will be generated. Will be followed 
            /// by a suffix specified during material generation.
            /// </summary>
            private static string filenameRoot = RoomObject.GameObjectName + "Material";//"RoomMaterial";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains("Material"))
                        {
                            filenameRoot = value + "Material";
                        }
                        else
                        {
                            filenameRoot = value;
                        }
                    }
                }
            }
            public static char Separator = '_';

            public static int GetIndex(string materialName)
            {
                return int.Parse(materialName.Split(Separator)[1]);
            }
            public static string CompileFilename(int roomChildIndex)
            {
                return FilenameRoot + Separator + roomChildIndex + Extension;
            }
            public static string CompileMaterialName(int roomChildIndex)
            {
                return FilenameRoot + Separator + roomChildIndex;
            }


            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName + "Material";

                    // Insert additional logic here
                }
            }
        }

        public class MatrixArray : Config_Base
        {
            static MatrixArray()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".txt";
            public static string filenameRoot = RoomObject.GameObjectName + "Matrices";//"RoomMatrices";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains("Matrices"))
                        {
                            filenameRoot = value + "Matrices";
                        }
                        else
                        {
                            filenameRoot = value;
                        }
                    }
                }
            }

            public static string CompileFilename()
            {
                return FilenameRoot + Extension;
            }

            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName + "Matrices";

                    // Insert additional logic here
                }
            }
        }

        public class Texture2DArray : Config_Base
        {
            static Texture2DArray()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".asset";
            private static string filenameRoot = RoomObject.GameObjectName + "TextureArray";//"RoomTextureArray";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains("TextureArray"))
                        {
                            filenameRoot = value + "TextureArray";
                        }
                        else
                        {
                            filenameRoot = value;
                        }
                    }
                }
            }

            public static string CompileFilename()
            {
                return FilenameRoot + Extension;
            }

            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName + "TextureArray";

                    // Insert additional logic here
                }
            }
        }

        public class AssetBundle
        {
            public class RawPackage : Config_Base
            {
                static RawPackage()
                {
                    Config.RoomObject.Changed += UpdateNameRoot;
                }

                /// <summary>
                /// Name for the room texture information asset bundle. Bundle 
                /// includes textures and text files representing the room mesh, 
                /// localToWorld matrices (transforms model coordinate space to 
                /// world coordinate space), worldToCamera matrices (transforms 
                /// world coordinate space to camera/view space), projection 
                /// matrices (transforms camera/view space to clip space), and 
                /// supplementary information regarding the mesh (positions & 
                /// rotations).
                /// 
                /// This information is designed to be used by the UWB_Texturing 
                /// namespace classes to generate a final RoomMesh object from 
                /// the components described.
                /// </summary>
                private static string name = (RoomObject.GameObjectName + "texture").ToLower();//"roomtexture";
                public static string Name
                {
                    get
                    {
                        return name;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!value.Contains("texture"))
                            {
                                name = (value + "texture").ToLower();
                            }
                            else
                            {
                                name = value.ToLower();
                            }
                        }
                    }
                }

                public static string CompileFilename()
                {
                    return Name;
                }

                public static void UpdateNameRoot(RoomNameChangedEventArgs e)
                {
                    if (!e.NewName.Equals(e.OldName))
                    {
                        Name = (e.NewName + "texture").ToLower();

                        // Insert additional logic here
                    }
                }
            }

            public class RoomPackage : Config_Base
            {
                static RoomPackage()
                {
                    RoomObject.Changed += UpdateNameRoot;
                }

                /// <summary>
                /// Name for the standalone, textured room mesh GameObject asset 
                /// bundle.
                /// </summary>
                private static string name = (RoomObject.GameObjectName + "prefab").ToLower();//"roomprefab";
                //private static string name = (RoomObject.GameObjectName).ToLower();
                public static string Name
                {
                    get
                    {
                        return name;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!value.Contains("prefab"))
                            {
                                name = (value + "prefab").ToLower();
                            }
                            else
                            {
                                name = value.ToLower();
                            }
                            //name = value.ToLower();
                        }
                    }
                }

                public static string CompileFilename()
                {
                    return Name;
                }

                public static void UpdateNameRoot(RoomNameChangedEventArgs e)
                {
                    if (!e.NewName.Equals(e.OldName))
                    {
                        Name = (e.NewName + "prefab").ToLower();
                        //Name = (e.NewName).ToLower();

                        // Insert additional logic here
                    }
                }

                public static string ExtractRoomName(string RoomPackageName)
                {
                    return RoomPackageName.Remove(RoomPackageName.LastIndexOf("prefab"), "prefab".Length);
                }
            }
        }

        public class Prefab : Config_Base
        {
            static Prefab()
            {
                RoomObject.Changed += UpdateFilenameRoot;
            }

            public static string Extension = ".prefab";
            private static string filenameRoot = RoomObject.GameObjectName;//"Room";
            public static string FilenameRoot
            {
                get
                {
                    return filenameRoot;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        filenameRoot = value;
                    }
                }
            }

            public static string CompileFilename()
            {
                return FilenameRoot + Extension;
            }

            public static void UpdateFilenameRoot(RoomNameChangedEventArgs e)
            {
                if (!e.NewName.Equals(e.OldName))
                {
                    FilenameRoot = e.NewName;

                    // Insert additional logic here
                }
            }
        }

        public class RoomObject : Config_Base
        {
            private static string gameObjectName = "Room";

            public static string GameObjectName
            {
                get
                {
                    return gameObjectName;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        string oldName = gameObjectName;
                        gameObjectName = value;
                        OnChanged(new RoomNameChangedEventArgs(oldName, gameObjectName));
                    }
                }
            }

            public static float RecommendedShaderRefreshTime = 5.0f;

            #region Event Handling
            public static event RoomNameChangedEventHandler Changed;

            protected static void OnChanged(RoomNameChangedEventArgs e)
            {
                if (Changed != null)
                    Changed(e);
            }
            #endregion
        }

        public class Shader : Config_Base
        {
            public static string Extension = ".shader";
            public static string FilenameWithoutExtension = "MyRoomShader";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }



            public static new string AssetSubFolder = Config_Base.AssetSubFolder + "/Shaders";
            public static string QualifiedFilenameWithoutExtension = "Unlit/" + FilenameWithoutExtension;
            
            public static new string CompileUnityAssetDirectory(string roomName)
            {
                return "Assets/" + AssetSubFolder + '/' + roomName;
            }
            public static new string CompileUnityAssetPath(string filename, string roomName)
            {
                return CompileUnityAssetDirectory(roomName) + '/' + filename;
            }
            public static new string CompileAbsoluteAssetDirectory(string roomName)
            {
                return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
            }
            public static new string CompileAbsoluteAssetPath(string filename, string roomName)
            {
                return Path.Combine(CompileAbsoluteAssetDirectory(roomName), filename);
            }
            public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
            {
                return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1);
            }
        }
    }
}