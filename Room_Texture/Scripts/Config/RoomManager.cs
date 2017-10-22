using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    public class RoomManager : MonoBehaviour
    {
        // Make a public dropdown list of past room names for easier reference and to avoid errors
        // Use a dictionary of strings and the associated dropdown index

        public string RoomName;

        // Use this for initialization
        void Start()
        {
            if (string.IsNullOrEmpty(RoomName))
            {
                RoomName = Config.RoomObject.GameObjectName;
            }
            InvokeRepeating("SyncRoomName", 5, 5);
        }

        void FixedUpdate()
        {
            if (!RoomName.Equals(Config.RoomObject.GameObjectName))
            {
                Config.RoomObject.GameObjectName = RoomName;
                // Make the directory for this room
                string directoryPath = Config.RoomObject.CompileAbsoluteAssetDirectory(Config.RoomObject.GameObjectName);
                AbnormalDirectoryHandler.CreateDirectory(directoryPath);
            }
        }

        public void SyncDisplayedRoomName()
        {
            RoomName = Config.RoomObject.GameObjectName;
        }

        public static string SyncRoomName()
        {
            string roomName = GameObject.Find("RoomManager").GetComponent<RoomManager>().RoomName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
            return roomName;
        }

        public static string[] GetAllRoomNames()
        {
            List<string> roomNames = new List<string>();

            // ERROR TESTING - Need to gather the room names from some centralized depot?
            foreach (string folderPath in Directory.GetDirectories(Path.Combine(Config_Base.AbsoluteAssetRootFolder, Config_Base.AssetSubFolder)))
            {
                string[] pass1 = folderPath.Split('/');
                string[] pass2 = pass1[pass1.Length - 1].Split('\\');

                string directoryName = pass2[pass2.Length - 1];
                if (!directoryName.StartsWith("_"))
                {
                    roomNames.Add(directoryName);
                }
            }

            return roomNames.ToArray();
        }
    }
}