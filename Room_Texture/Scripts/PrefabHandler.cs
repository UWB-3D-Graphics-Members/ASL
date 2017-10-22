using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWB_Texturing
{
    public class PrefabHandler : MonoBehaviour
    {
        public static class Messages
        {
            public static string GameObjectDoesNotExist = "Room prefab generation failed. Does Room object exist in the scene object hierarchy?";
            public static string PrefabDoesNotExist = "Room prefab does not exist.";
        }
        
        public static void CreateRoomPrefab(GameObject obj)
        {
#if UNITY_EDITOR
            if (obj != null)
            {
                //AssetDatabase.CreateAsset(obj, "Assets/Room Texture/Resources/Test.obj");
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                //string roomName = Config.RoomObject.GameObjectName;
                string roomName = obj.name ?? Config.RoomObject.GameObjectName;

                if (!Directory.Exists(Config.Prefab.CompileAbsoluteAssetDirectory(roomName)))
                {
                    //Directory.CreateDirectory(Config.Prefab.CompileAbsoluteAssetDirectory());
                    AbnormalDirectoryHandler.CreateDirectory(Config.Prefab.CompileAbsoluteAssetDirectory(roomName));
                    Debug.Log("Prefab folder created: " + Config.Prefab.CompileAbsoluteAssetDirectory(roomName));
                }
                //var emptyPrefab = PrefabUtility.CreateEmptyPrefab(CrossPlatformNames.Prefab.CompileCompatibleOutputFolder() + '/' + CrossPlatformNames.Prefab.Filename);
                //PrefabUtility.ReplacePrefab()
                //PrefabUtility.CreatePrefab(CrossPlatformNames.Prefab.CompileCompatibleOutputFolder() + '/' + CrossPlatformNames.Prefab.Filename, AssetDatabase.Find)

                //PrefabUtility.CreatePrefab(Config.Prefab.CompileUnityAssetDirectory(roomName) + '/' + Config.Prefab.CompileFilename(), obj); // CompileCompatibleOutputFolder
                PrefabUtility.CreatePrefab(Config.Prefab.CompileUnityAssetPath(Config.Prefab.CompileFilename(), roomName), obj);
                //PrefabUtility.CreatePrefab(CrossPlatformNames.Prefab.OutputFilepath, obj);
                
                Debug.Log("Room prefab generated at " + Config.Prefab.CompileUnityAssetDirectory(roomName));
                //Debug.Log("Room path = " + CrossPlatformNames.Prefab.OutputFilepath);
                //Debug.Log("Room path = " + Config.Prefab.CompileUnityAssetDirectory(roomName) + '/' + Config.Prefab.CompileFilename()); // CompileCompatibleOutputFolder
                Debug.Log("Room path = " + Config.Prefab.CompileUnityAssetPath(Config.Prefab.CompileFilename(), roomName));
            }
            else
            {
                //UnityEngine.Debug.Log(Messages.GameObjectDoesNotExist);

                //string originalRoomName = Config.RoomObject.GameObjectName;

                //Config.RoomObject.GameObjectName = obj.name;
                //BundleHandler.InstantiateRoom(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), obj.name));
                BundleHandler.InstantiateRoom(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), Config.RoomObject.GameObjectName));

                CreateRoomPrefab(GameObject.Find(Config.RoomObject.GameObjectName));

                BundleHandler.RemoveRoomObject(Config.RoomObject.GameObjectName);

                //Config.RoomObject.GameObjectName = originalRoomName;
            }

            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static void CreateAllRoomPrefabs()
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach(string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                GameObject room = GameObject.Find(roomName);
                CreateRoomPrefab(room);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        private static void CreatePrefab(string gameObjectName)
        {
            GameObject obj = GameObject.Find(gameObjectName);
            CreateRoomPrefab(obj);
        }

        public static void DeletePrefab(string roomName)
        {
            //string absoluteFilepath = Path.Combine(Config.Prefab.CompileAbsoluteAssetDirectory(roomName), Config.Prefab.CompileFilename());
            string absoluteFilepath = Config.Prefab.CompileAbsoluteAssetPath(Config.Prefab.CompileFilename(), roomName);
            if (File.Exists(absoluteFilepath))
            {
                File.Delete(absoluteFilepath);
            }
            else
            {
                UnityEngine.Debug.Log(Messages.PrefabDoesNotExist);
            }
            //absoluteFilepath = Path.Combine(CrossPlatformNames.Prefab.AbsoluteOutputFolder, CrossPlatformNames.Prefab.StandaloneRoom.Filename);
            //File.Delete(absoluteFilepath);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void DeletePrefab(string[] roomNames)
        {
            string originalRoomName = Config.RoomObject.GameObjectName;

            foreach(string roomName in roomNames)
            {
                Config.RoomObject.GameObjectName = roomName;
                DeletePrefab(roomName);
            }

            Config.RoomObject.GameObjectName = originalRoomName;
        }

        public static void DeleteAllPrefabs()
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            DeletePrefab(roomNames);
        }
    }
}