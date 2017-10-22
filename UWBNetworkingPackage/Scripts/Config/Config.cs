using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class Config : Config_Base
    {
        static Config()
        {
            //Config_Base.SetFolders();
        }

        public static new void Start(NodeType platform)
        {
            // Triggers static constructor
            Config_Base.Start(platform);

            Current.Room.Start();
        }

        public class Current : Config_Base
        {
            public class AssetBundle : Config_Base_AssetBundle
            {
                public static new string AssetSubFolder
                {
                    get
                    {
                        switch (NodeType)
                        {
                            case NodeType.PC:
                                return PC.AssetBundle.AssetSubFolder;
                            case NodeType.Android:
                            case NodeType.Tango:
                                return Android.AssetBundle.AssetSubFolder;
                            case NodeType.Kinect:
                                return Kinect.AssetBundle.AssetSubFolder;
                            case NodeType.Oculus:
                                return Oculus.AssetBundle.AssetSubFolder;
                            case NodeType.Vive:
                                return Vive.AssetBundle.AssetSubFolder;
                            case NodeType.Hololens:
                                return Hololens.AssetBundle.AssetSubFolder;
                            default:
                                throw new System.Exception(Messages.PlatformNotFound);
                        }
                    }
                    set
                    {
                        switch (NodeType)
                        {
                            case NodeType.PC:
                                PC.AssetBundle.AssetSubFolder = value;
                                break;
                            case NodeType.Android:
                            case NodeType.Tango:
                                Android.AssetBundle.AssetSubFolder = value;
                                break;
                            case NodeType.Kinect:
                                Kinect.AssetBundle.AssetSubFolder = value;
                                break;
                            case NodeType.Oculus:
                                Oculus.AssetBundle.AssetSubFolder = value;
                                break;
                            case NodeType.Vive:
                                Vive.AssetBundle.AssetSubFolder = value;
                                break;
                            case NodeType.Hololens:
                                Hololens.AssetBundle.AssetSubFolder = value;
                                break;
                            default:
                                throw new System.Exception(Messages.PlatformNotFound);
                        }
                    }
                }

                public static new string CompileAbsoluteAssetDirectory()
                {
                    switch (NodeType)
                    {
                        case NodeType.Vive:
                            return Vive.AssetBundle.CompileAbsoluteAssetDirectory();
                        case NodeType.Oculus:
                            return Oculus.AssetBundle.CompileAbsoluteAssetDirectory();
                        case NodeType.Kinect:
                            return Kinect.AssetBundle.CompileAbsoluteAssetDirectory();
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.AssetBundle.CompileAbsoluteAssetDirectory();
                        case NodeType.PC:
                            return PC.AssetBundle.CompileAbsoluteAssetDirectory();
                        case NodeType.Hololens:
                            return Hololens.AssetBundle.CompileAbsoluteAssetDirectory();
                        default:
                            throw new System.Exception(Messages.PlatformNotFound);
                    }
                }

                public static new string CompileAbsoluteAssetPath(string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.AssetBundle.CompileAbsoluteAssetPath(filename);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.AssetBundle.CompileAbsoluteAssetPath(filename);
                        case NodeType.Kinect:
                            return Kinect.AssetBundle.CompileAbsoluteAssetPath(filename);
                        case NodeType.Oculus:
                            return Oculus.AssetBundle.CompileAbsoluteAssetPath(filename);
                        case NodeType.Vive:
                            return Vive.AssetBundle.CompileAbsoluteAssetPath(filename);
                        case NodeType.Hololens:
                            return Hololens.AssetBundle.CompileAbsoluteAssetPath(filename);
                        default:
                            throw new System.Exception(Messages.PlatformNotFound);
                    }
                }

                public static new string CompileUnityAssetDirectory()
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.AssetBundle.CompileUnityAssetDirectory();
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.AssetBundle.CompileUnityAssetDirectory();
                        case NodeType.Kinect:
                            return Kinect.AssetBundle.CompileUnityAssetDirectory();
                        case NodeType.Oculus:
                            return Oculus.AssetBundle.CompileUnityAssetDirectory();
                        case NodeType.Vive:
                            return Vive.AssetBundle.CompileUnityAssetDirectory();
                        case NodeType.Hololens:
                            return Hololens.AssetBundle.CompileUnityAssetDirectory();
                        default:
                            throw new System.Exception(Messages.PlatformNotFound);
                    }
                }

                public static new string CompileUnityAssetPath(string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.AssetBundle.CompileUnityAssetPath(filename);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.AssetBundle.CompileUnityAssetPath(filename);
                        case NodeType.Kinect:
                            return Kinect.AssetBundle.CompileUnityAssetPath(filename);
                        case NodeType.Oculus:
                            return Oculus.AssetBundle.CompileUnityAssetPath(filename);
                        case NodeType.Vive:
                            return Vive.AssetBundle.CompileUnityAssetPath(filename);
                        case NodeType.Hololens:
                            return Hololens.AssetBundle.CompileUnityAssetPath(filename);
                        default:
                            throw new System.Exception(Messages.PlatformNotFound);
                    }
                }

                public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Kinect:
                            return Kinect.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Oculus:
                            return Oculus.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Vive:
                            return Vive.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Hololens:
                            return Hololens.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                        default:
                            throw new System.Exception(Messages.PlatformNotFound);
                    }
                }
                //private static string assetSubFolder = Config_Base_Room.AssetSubFolder + "/StreamingAssets/";


                //public new static string AssetSubFolder = "ASL/Resources";
                //public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";


                //                public new static string AssetSubFolder
                //                {
                //                    get
                //                    {
                //                        return assetSubFolder;
                //                    }
                //                    set
                //                    {
                //#if UNITY_WSA_10_0
                //#else
                //                        assetSubFolder = value;
                //#endif
                //                    }
                //                }

                //public new static string CompileUnityBundleDirectory()
                //{
                //    switch (NodeType)
                //    {
                //        case NodeType.PC:
                //            return PC.CompileUnityBundleDirectory();
                //        case NodeType.Android:
                //            return Android.CompileUnityBundleDirectory();
                //        case NodeType.Hololens:
                //            return Hololens.CompileUnityBundleDirectory();
                //        case NodeType.Kinect:
                //            return Kinect.CompileUnityBundleDirectory();
                //        case NodeType.Oculus:
                //            return Oculus.CompileUnityBundleDirectory();
                //        case NodeType.Vive:
                //            return Vive.CompileUnityBundleDirectory();
                //        default:
                //            throw new System.Exception("Unrecognized platform.");
                //    }
                //}

                //public new static string CompileUnityBundlePath(string filename)
                //{
                //    switch (NodeType)
                //    {
                //        case NodeType.PC:
                //            return PC.CompileUnityBundlePath(filename);
                //        case NodeType.Android:
                //            return Android.CompileUnityBundlePath(filename);
                //        case NodeType.Hololens:
                //            return Hololens.CompileUnityBundlePath(filename);
                //        case NodeType.Kinect:
                //            return Kinect.CompileUnityBundlePath(filename);
                //        case NodeType.Oculus:
                //            return Oculus.CompileUnityBundlePath(filename);
                //        case NodeType.Vive:
                //            return Vive.CompileUnityBundlePath(filename);
                //        default:
                //            throw new System.Exception("Unrecognized platform.");
                //    }
                //}

                //public new static string CompileAbsoluteBundleDirectory()
                //{
                //    switch (NodeType)
                //    {
                //        case NodeType.PC:
                //            return PC.CompileAbsoluteBundleDirectory();
                //        case NodeType.Android:
                //            return Android.CompileAbsoluteBundleDirectory();
                //        case NodeType.Hololens:
                //            return Hololens.CompileAbsoluteBundleDirectory();
                //        case NodeType.Kinect:
                //            return Kinect.CompileAbsoluteBundleDirectory();
                //        case NodeType.Oculus:
                //            return Oculus.CompileAbsoluteBundleDirectory();
                //        case NodeType.Vive:
                //            return Vive.CompileAbsoluteBundleDirectory();
                //        default:
                //            throw new System.Exception("Unrecognized platform.");
                //    }
                //}

                //public new static string CompileAbsoluteBundlePath(string filename)
                //{

                //    switch (NodeType)
                //    {
                //        case NodeType.PC:
                //            return PC.CompileAbsoluteBundlePath(filename);
                //        case NodeType.Android:
                //            return Android.CompileAbsoluteBundlePath(filename);
                //        case NodeType.Hololens:
                //            return Hololens.CompileAbsoluteBundlePath(filename);
                //        case NodeType.Kinect:
                //            return Kinect.CompileAbsoluteBundlePath(filename);
                //        case NodeType.Oculus:
                //            return Oculus.CompileAbsoluteBundlePath(filename);
                //        case NodeType.Vive:
                //            return Vive.CompileAbsoluteBundlePath(filename);
                //        default:
                //            throw new System.Exception("Unrecognized platform.");
                //    }
                //}
            }

            public class Room : Config_Base_Room
            {
                public static new string CompileAbsoluteAssetDirectory()
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileAbsoluteAssetDirectory();
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileAbsoluteAssetDirectory();
                        case NodeType.Hololens:
                            return Hololens.Room.CompileAbsoluteAssetDirectory();
                        case NodeType.Kinect:
                            return Kinect.Room.CompileAbsoluteAssetDirectory();
                        case NodeType.Oculus:
                            return Oculus.Room.CompileAbsoluteAssetDirectory();
                        case NodeType.Vive:
                            return Vive.Room.CompileAbsoluteAssetDirectory();
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileAbsoluteAssetDirectory(string roomName)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileAbsoluteAssetDirectory(roomName);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileAbsoluteAssetDirectory(roomName);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileAbsoluteAssetDirectory(roomName);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileAbsoluteAssetDirectory(roomName);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileAbsoluteAssetDirectory(roomName);
                        case NodeType.Vive:
                            return Vive.Room.CompileAbsoluteAssetDirectory(roomName);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileAbsoluteAssetPath(string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileAbsoluteAssetPath(filename);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileAbsoluteAssetPath(filename);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileAbsoluteAssetPath(filename);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileAbsoluteAssetPath(filename);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileAbsoluteAssetPath(filename);
                        case NodeType.Vive:
                            return Vive.Room.CompileAbsoluteAssetPath(filename);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileAbsoluteAssetPath(string roomName, string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileAbsoluteAssetPath(roomName, filename);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileAbsoluteAssetPath(roomName, filename);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileAbsoluteAssetPath(roomName, filename);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileAbsoluteAssetPath(roomName, filename);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileAbsoluteAssetPath(roomName, filename);
                        case NodeType.Vive:
                            return Vive.Room.CompileAbsoluteAssetPath(roomName, filename);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileUnityAssetDirectory()
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileUnityAssetDirectory();
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileUnityAssetDirectory();
                        case NodeType.Hololens:
                            return Hololens.Room.CompileUnityAssetDirectory();
                        case NodeType.Kinect:
                            return Kinect.Room.CompileUnityAssetDirectory();
                        case NodeType.Oculus:
                            return Oculus.Room.CompileUnityAssetDirectory();
                        case NodeType.Vive:
                            return Vive.Room.CompileUnityAssetDirectory();
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileUnityAssetDirectory(string roomName)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileUnityAssetDirectory(roomName);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileUnityAssetDirectory(roomName);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileUnityAssetDirectory(roomName);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileUnityAssetDirectory(roomName);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileUnityAssetDirectory(roomName);
                        case NodeType.Vive:
                            return Vive.Room.CompileUnityAssetDirectory(roomName);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileUnityAssetPath(string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileUnityAssetPath(filename);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileUnityAssetPath(filename);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileUnityAssetPath(filename);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileUnityAssetPath(filename);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileUnityAssetPath(filename);
                        case NodeType.Vive:
                            return Vive.Room.CompileUnityAssetPath(filename);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileUnityAssetPath(string roomName, string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileUnityAssetPath(roomName, filename);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileUnityAssetPath(roomName, filename);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileUnityAssetPath(roomName, filename);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileUnityAssetPath(roomName, filename);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileUnityAssetPath(roomName, filename);
                        case NodeType.Vive:
                            return Vive.Room.CompileUnityAssetPath(roomName, filename);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileResourcesLoadPath(assetNameWithoutExtension);
                        case NodeType.Vive:
                            return Vive.Room.CompileResourcesLoadPath(assetNameWithoutExtension);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public static new string CompileResourcesLoadPath(string roomName, string assetNameWithoutExtension)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.Room.CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
                        case NodeType.Android:
                        case NodeType.Tango:
                            return Android.Room.CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
                        case NodeType.Hololens:
                            return Hololens.Room.CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
                        case NodeType.Kinect:
                            return Kinect.Room.CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
                        case NodeType.Oculus:
                            return Oculus.Room.CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
                        case NodeType.Vive:
                            return Vive.Room.CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                //public class AssetBundle : Config_Base_AssetBundle
                //{
                //    public new static string CompileUnityBundleDirectory(string roomName)
                //    {
                //        return "Assets/" + RoomResourceSubFolder;
                //    }
                //    public new static string CompileUnityBundlePath(string roomName, string filename)
                //    {
                //        return CompileUnityBundleDirectory() + '/' + filename;
                //    }
                //    public new static string CompileAbsoluteBundleDirectory(string roomName)
                //    {
                //        return Path.Combine(Directory.GetCurrentDirectory(), RoomResourceSubFolder);
                //    }
                //    public new static string CompileAbsoluteBundlePath(string roomName,
                //        string filename)
                //    {
                //        return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
                //    }
                //}
            }
        }

        public class Android
        {
            public class AssetBundle : Config_Base_AssetBundle
            {
                static AssetBundle()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());

                    Debug.debugging = true;
                    Debug.Log("Asset Bundle directory created at " + CompileAbsoluteAssetDirectory());
                    Debug.debugging = false;
                }

                private static string assetSubFolder = Config_Base_AssetBundle.AssetSubFolder + "/AssetBundlesAndroid";
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
                    return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
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
            }

            public class Room : Config_Base_Room
            {
                static Room()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());

                    Debug.debugging = true;
                    Debug.Log("Room directory created at " + CompileAbsoluteAssetDirectory());
                    Debug.debugging = false;
                }
            }
        }

        public class Hololens
        {
            //public Hololens()
            //{
            //    NodeType = NodeType.Hololens;
            //}

            public class AssetBundle : Config_Base_AssetBundle
            {
                // ERROR TESTING - Need to update this section because the pathways WILL be different
            }

            public class Room : Config_Base_Room
            {
                // ERROR TESTING - Need to update this section because the pathways WILL be different
            }
        }

        public class Kinect
        {
            //public Kinect()
            //{
            //    NodeType = NodeType.Kinect;
            //}

            public class AssetBundle : Config_Base_AssetBundle
            {
                static AssetBundle()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }

                public static new string AssetSubFolder
                {
                    get
                    {
                        return PC.AssetBundle.AssetSubFolder;
                    }
                    set
                    {
                        PC.AssetBundle.AssetSubFolder = value;
                    }
                }

                public static new string CompileAbsoluteAssetDirectory()
                {
                    return PC.AssetBundle.CompileAbsoluteAssetDirectory();
                }

                public static new string CompileAbsoluteAssetPath(string filename)
                {
                    return PC.AssetBundle.CompileAbsoluteAssetPath(filename);
                }

                public static new string CompileUnityAssetDirectory()
                {
                    return PC.AssetBundle.CompileUnityAssetDirectory();
                }

                public static new string CompileUnityAssetPath(string filename)
                {
                    return PC.AssetBundle.CompileUnityAssetPath(filename);
                }

                public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
                {
                    return PC.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                }
            }

            public class Room : Config_Base_Room
            {
                static Room()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }
            }
        }

        public class Oculus
        {
            //public Oculus()
            //{
            //    NodeType = NodeType.Oculus;
            //}

            public class AssetBundle : Config_Base_AssetBundle
            {
                static AssetBundle()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }

                public static new string AssetSubFolder
                {
                    get
                    {
                        return PC.AssetBundle.AssetSubFolder;
                    }
                    set
                    {
                        PC.AssetBundle.AssetSubFolder = value;
                    }
                }

                public static new string CompileAbsoluteAssetDirectory()
                {
                    return PC.AssetBundle.CompileAbsoluteAssetDirectory();
                }

                public static new string CompileAbsoluteAssetPath(string filename)
                {
                    return PC.AssetBundle.CompileAbsoluteAssetPath(filename);
                }

                public static new string CompileUnityAssetDirectory()
                {
                    return PC.AssetBundle.CompileUnityAssetDirectory();
                }

                public static new string CompileUnityAssetPath(string filename)
                {
                    return PC.AssetBundle.CompileUnityAssetPath(filename);
                }

                public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
                {
                    return PC.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                }
            }

            public class Room : Config_Base_Room
            {
                static Room()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }
            }
        }

        public class Vive
        {
            //public Vive()
            //{
            //    NodeType = NodeType.Vive;
            //}

            public class AssetBundle : Config_Base_AssetBundle
            {
                static AssetBundle()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }

                public static new string AssetSubFolder
                {
                    get
                    {
                        return PC.AssetBundle.AssetSubFolder;
                    }
                    set
                    {
                        PC.AssetBundle.AssetSubFolder = value;
                    }
                }

                public static new string CompileAbsoluteAssetDirectory()
                {
                    return PC.AssetBundle.CompileAbsoluteAssetDirectory();
                }

                public static new string CompileAbsoluteAssetPath(string filename)
                {
                    return PC.AssetBundle.CompileAbsoluteAssetPath(filename);
                }

                public static new string CompileUnityAssetDirectory()
                {
                    return PC.AssetBundle.CompileUnityAssetDirectory();
                }

                public static new string CompileUnityAssetPath(string filename)
                {
                    return PC.AssetBundle.CompileUnityAssetPath(filename);
                }

                public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
                {
                    return PC.AssetBundle.CompileResourcesLoadPath(assetNameWithoutExtension);
                }
            }

            public class Room : Config_Base_Room
            {
                static Room()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }
            }
        }

        public class PC
        {
            public class AssetBundle : Config_Base_AssetBundle
            {
                static AssetBundle()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }

                private static string assetSubFolder = Config_Base_AssetBundle.AssetSubFolder + "/AssetBundlesPC";
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
                    return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
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
            }

            public class Room : Config_Base_Room
            {
                static Room()
                {
                    Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
                }
            }
        }


















//        public class AssetBundle
//        {
//            public class Current : Config_Base_AssetBundle
//            {
//                public new static string AssetSubFolder = "ASL/Resources";
//                public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";

//                public Current()
//                {
//                    // Set the node type depending on what platform this is (PC, Android, etc.)
//                    // if PC, NodeType = NodeType.PC;

//                    AssetSubFolder = "ASL/Resources";
//                    switch (NodeType)
//                    {
//                        case NodeType.PC:
//                            BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";
//                            break;
//                        case NodeType.Android:
//                            BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesAndroid";
//                            break;
//                        case NodeType.Hololens:
//                            break;
//                        case NodeType.Kinect:
//                            break;
//                        case NodeType.Oculus:
//                            break;
//                        case NodeType.Vive:
//                            break;
//                        default:
//                            break;
//                    }
//                }

//                public new static string CompileUnityBundleDirectory()
//                {
//                    switch (NodeType)
//                    {
//                        case NodeType.PC:
//                            return PC.CompileUnityBundleDirectory();
//                        case NodeType.Android:
//                            return Android.CompileUnityBundleDirectory();
//                        case NodeType.Hololens:
//                            return Hololens.CompileUnityBundleDirectory();
//                        case NodeType.Kinect:
//                            return Kinect.CompileUnityBundleDirectory();
//                        case NodeType.Oculus:
//                            return Oculus.CompileUnityBundleDirectory();
//                        case NodeType.Vive:
//                            return Vive.CompileUnityBundleDirectory();
//                        default:
//                            throw new System.Exception("Unrecognized platform.");
//                    }
//                }

//                public new static string CompileUnityBundlePath(string filename)
//                {
//                    switch (NodeType)
//                    {
//                        case NodeType.PC:
//                            return PC.CompileUnityBundlePath(filename);
//                        case NodeType.Android:
//                            return Android.CompileUnityBundlePath(filename);
//                        case NodeType.Hololens:
//                            return Hololens.CompileUnityBundlePath(filename);
//                        case NodeType.Kinect:
//                            return Kinect.CompileUnityBundlePath(filename);
//                        case NodeType.Oculus:
//                            return Oculus.CompileUnityBundlePath(filename);
//                        case NodeType.Vive:
//                            return Vive.CompileUnityBundlePath(filename);
//                        default:
//                            throw new System.Exception("Unrecognized platform.");
//                    }
//                }

//                public new static string CompileAbsoluteBundleDirectory()
//                {
//                    switch (NodeType)
//                    {
//                        case NodeType.PC:
//                            return PC.CompileAbsoluteBundleDirectory();
//                        case NodeType.Android:
//                            return Android.CompileAbsoluteBundleDirectory();
//                        case NodeType.Hololens:
//                            return Hololens.CompileAbsoluteBundleDirectory();
//                        case NodeType.Kinect:
//                            return Kinect.CompileAbsoluteBundleDirectory();
//                        case NodeType.Oculus:
//                            return Oculus.CompileAbsoluteBundleDirectory();
//                        case NodeType.Vive:
//                            return Vive.CompileAbsoluteBundleDirectory();
//                        default:
//                            throw new System.Exception("Unrecognized platform.");
//                    }
//                }

//                public new static string CompileAbsoluteBundlePath(string filename)
//                {

//                    switch (NodeType)
//                    {
//                        case NodeType.PC:
//                            return PC.CompileAbsoluteBundlePath(filename);
//                        case NodeType.Android:
//                            return Android.CompileAbsoluteBundlePath(filename);
//                        case NodeType.Hololens:
//                            return Hololens.CompileAbsoluteBundlePath(filename);
//                        case NodeType.Kinect:
//                            return Kinect.CompileAbsoluteBundlePath(filename);
//                        case NodeType.Oculus:
//                            return Oculus.CompileAbsoluteBundlePath(filename);
//                        case NodeType.Vive:
//                            return Vive.CompileAbsoluteBundlePath(filename);
//                        default:
//                            throw new System.Exception("Unrecognized platform.");
//                    }
//                }
//            }

//            public class Android : Config_Base_AssetBundle
//            {
//                public Android()
//                {
//                    NodeType = NodeType.Android;
//                }

//                public new static string AssetSubFolder = "ASL/Resources";
//                public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesAndroid";

//                public new static string CompileUnityBundleDirectory()
//                {
//                    return BundleSubFolder;
//                }
//                public new static string CompileUnityBundlePath(string filename)
//                {
//                    return CompileUnityBundleDirectory() + '/' + filename;
//                }
//                public new static string CompileAbsoluteBundleDirectory()
//                {
//#if UNITY_WSA_10_0
//            return AbsoluteAssetRootFolder;
//#else
//                    return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
//#endif
//                }
//                public new static string CompileAbsoluteBundlePath(string filename)
//                {
//                    return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
//                }
//            }

//            public class Hololens : Config_Base_AssetBundle
//            {
//                public Hololens()
//                {
//                    NodeType = NodeType.Hololens;
//                }
//            }

//            public class Kinect : Config_Base_AssetBundle
//            {
//                public Kinect()
//                {
//                    NodeType = NodeType.Kinect;
//                }
//            }

//            public class Oculus : Config_Base_AssetBundle
//            {
//                public Oculus()
//                {
//                    NodeType = NodeType.Oculus;
//                }
//            }

//            public class Vive : Config_Base_AssetBundle
//            {
//                public Vive()
//                {
//                    NodeType = NodeType.Vive;
//                }
//            }

//            public class PC : Config_Base_AssetBundle
//            {
//                public PC()
//                {
//                    NodeType = NodeType.PC;
//                }
                
//                public new static string AssetSubFolder = "ASL/Resources";
//                public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";

//                public new static string CompileUnityBundleDirectory()
//                {
//                    return "Assets/" + BundleSubFolder;
//                }
//                public new static string CompileUnityBundlePath(string filename)
//                {
//                    return CompileUnityBundleDirectory() + '/' + filename;
//                }
//                public new static string CompileAbsoluteBundleDirectory()
//                {
//#if UNITY_WSA_10_0
//            return AbsoluteAssetRootFolder;
//#else
//                    return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
//#endif
//                }
//                public new static string CompileAbsoluteBundlePath(string filename)
//                {
//                    return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
//                }
//            }

//        }

//        public class Room : Config_Base
//        {
//            public class AssetBundle : Config_Base_AssetBundle
//            {
//                public new static string CompileUnityBundleDirectory(string roomName)
//                {
//                    return "Assets/" + RoomResourceSubFolder;
//                }
//                public new static string CompileUnityBundlePath(string roomName, string filename)
//                {
//                    return CompileUnityBundleDirectory() + '/' + filename;
//                }
//                public new static string CompileAbsoluteBundleDirectory(string roomName)
//                {
//                    return Path.Combine(Directory.GetCurrentDirectory(), RoomResourceSubFolder);
//                }
//                public new static string CompileAbsoluteBundlePath(string roomName, 
//                    string filename)
//                {
//                    return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
//                }
//            }
//        }

//        public static class Ports
//        {
//            public enum Types
//            {
//                Bundle,
//                Bundle_ClientToServer,
//                RoomResourceBundle,
//                RoomResourceBundle_ClientToServer,
//                RoomBundle,
//                RoomBundle_ClientToServer,
//                ClientServerConnection,
//                FindServer
//            }

//            public static int GetPort(Types portType)
//            {
//                switch (portType)
//                {
//                    case Types.Bundle:
//                        return Bundle;
//                    case Types.Bundle_ClientToServer:
//                        return Bundle_ClientToServer;
//                    case Types.RoomResourceBundle:
//                        return RoomResourceBundle;
//                    case Types.RoomResourceBundle_ClientToServer:
//                        return RoomResourceBundle_ClientToServer;
//                    case Types.RoomBundle:
//                        return RoomBundle;
//                    case Types.RoomBundle_ClientToServer:
//                        return RoomBundle_ClientToServer;
//                    case Types.ClientServerConnection:
//                        return ClientServerConnection;
//                    case Types.FindServer:
//                        return FindServer;
//                }

//                return Base;
//            }

//            private static int port = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().Port;
//            public static int Base
//            {
//                get
//                {
//                    return port;
//                }
//                set
//                {
//                    if(value < 64000 && value > 20000)
//                    {
//                        port = value;
//                    }
//                    else
//                    {
//                        Debug.Log("Invalid port chosen. Please select a port between 20000 and 64000");
//                    }
//                }
//            }
//            public static int FindServer
//            {
//                get
//                {
//                    return Base + 1;
//                }
//            }
//            public static int ClientServerConnection
//            {
//                get
//                {
//                    return Base + 2;
//                }
//            }
//            public static int Bundle
//            {
//                get
//                {
//                    return Base + 3;
//                }
//            }
//            public static int Bundle_ClientToServer
//            {
//                get
//                {
//                    return Base + 4;
//                }
//            }
//            public static int RoomResourceBundle
//            {
//                get
//                {
//                    return Base + 5;
//                }
//            }
//            public static int RoomResourceBundle_ClientToServer
//            {
//                get
//                {
//                    return Base + 6;
//                }
//            }
//            public static int RoomBundle
//            {
//                get
//                {
//                    return Base + 7;
//                }
//            }
//            public static int RoomBundle_ClientToServer
//            {
//                get
//                {
//                    return Base + 8;
//                }
//            }
//        }
    }
}