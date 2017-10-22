using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage {
    public static class Menu {
#if UNITY_EDITOR && !UNITY_WSA_10_0
        #region Instantiate
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/Current Room/GameObject", false, 0)]
        public static void InstantiateRoom()
        {
            //UWB_Texturing.Menu.InstantiateRoom();
            MenuHandler.InstantiateRoom();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/All Rooms/GameObjects", false, 0)]
        public static void InstantiateAllRooms()
        {
            //UWB_Texturing.Menu.InstantiateAllRooms();
            MenuHandler.InstantiateAllRooms();
        }
        #endregion

        #region Prefabs
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/Current Room/Prefab", false, 0)]
        public static void InstantiateRoomPrefab()
        {
            //UWB_Texturing.Menu.InstantiateRoomPrefab();
            MenuHandler.InstantiateRoomPrefab();
        }
        
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/All Rooms/Prefabs", false, 0)]
        public static void InstantiateAllRoomPrefabs()
        {
            //UWB_Texturing.Menu.InstantiateAllRoomPrefabs();
            MenuHandler.InstantiateAllRoomPrefabs();
        }
        #endregion

        #region Delete
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/GameObject", false, 0)]
        public static void RemoveRoomObject()
        {
            //UWB_Texturing.Menu.RemoveRoomObject();
            MenuHandler.RemoveRoomObject();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/GameObject", false, 0)]
        public static void RemoveAllRoomObjects()
        {
            //UWB_Texturing.Menu.RemoveAllRoomObjects();
            MenuHandler.RemoveAllRoomObjects();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Prefab", false, 0)]
        public static void RemoveRoomPrefab()
        {
            //UWB_Texturing.Menu.RemoveRoomPrefab();
            MenuHandler.RemoveRoomPrefab();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Prefab", false, 0)]
        public static void RemoveAllRoomPrefabs()
        {
            //UWB_Texturing.Menu.RemoveAllRoomPrefabs();
            MenuHandler.RemoveAllRoomPrefabs();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Resources", false, 0)]
        public static void RemoveRoomResources()
        {
            //UWB_Texturing.Menu.RemoveRoomResources();
            MenuHandler.RemoveRoomResources();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Resources", false, 0)]
        public static void RemoveAllRoomResources()
        {
            //UWB_Texturing.Menu.RemoveAllRoomResources();
            MenuHandler.RemoveAllRoomResources();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Raw Info", false, 0)]
        public static void RemoveRoomRawInfo()
        {
            //UWB_Texturing.Menu.RemoveRoomRawInfo();
            MenuHandler.RemoveRoomRawInfo();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Raw Info", false, 0)]
        public static void RemoveAllRoomRawInfo()
        {
            //UWB_Texturing.Menu.RemoveAllRoomRawInfo();
            MenuHandler.RemoveAllRoomRawInfo();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Everything", false, 0)]
        public static void RemoveEverything()
        {
            //UWB_Texturing.Menu.RemoveEverything();
            MenuHandler.RemoveEverything();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Everything", false, 0)]
        public static void RemoveAllEverything()
        {
            //UWB_Texturing.Menu.RemoveAllEverything();
            MenuHandler.RemoveAllEverything();
        }
#endregion

        #region Bundle
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Pack/Current Room", false, 0)]
        public static void PackRoomResourceBundle()
        {
            //UWB_Texturing.Menu.PackRoomResourceBundle();
            MenuHandler.PackRoomResourceBundle();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Pack/All Rooms", false, 0)]
        public static void PackAllRoomResourceBundles()
        {
            //UWB_Texturing.Menu.PackAllRoomResourceBundles();
            MenuHandler.PackAllRoomResourceBundles();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Pack/Current Room", false, 0)]
        public static void PackRoomBundle()
        {
            //UWB_Texturing.Menu.PackRoomBundle();
            MenuHandler.PackRoomBundle();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Pack/All Rooms", false, 0)]
        public static void PackAllRoomBundles()
        {
            //UWB_Texturing.Menu.PackAllRoomBundles();
            MenuHandler.PackAllRoomBundles();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Unpack/Current Room", false, 0)]
        public static void UnpackRoomResourceBundle()
        {
            //UWB_Texturing.Menu.UnpackRoomResourceBundle();
            MenuHandler.UnpackRoomResourceBundle();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Unpack/All Rooms", false, 0)]
        public static void UnpackAllRoomResourceBundles()
        {
            //UWB_Texturing.Menu.UnpackAllRoomResourceBundles();
            MenuHandler.UnpackAllRoomResourceBundles();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Unpack/Current Room", false, 0)]
        public static void UnpackRoomBundle()
        {
            //UWB_Texturing.Menu.UnpackRoomBundle();
            MenuHandler.UnpackRoomBundle();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Unpack/All Rooms", false, 0)]
        public static void UnpackAllRoomBundles()
        {
            //UWB_Texturing.Menu.UnpackAllRoomBundles();
            MenuHandler.UnpackAllRoomBundles();
        }

        //[UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Unpack/All Rooms", false, 0)]
        //public static void ExportRoomBundleToMasterClient()
        //{
            
        //}
        #endregion











        //[UnityEditor.MenuItem("ASL/Room Texture/Pack Raw Resources (Offline)", false, 0)]
        //public static void PackRawResourcesBundle()
        //{
        //    MenuHandler.PackRawResourcesBundle();
        //}

        //[UnityEditor.MenuItem("ASL/Room Texture/Pack Room Bundle (Offline)", false, 0)]
        //public static void PackRoomBundle()
        //{
        //    MenuHandler.PackRoomBundle();
        //}

        ///// <summary>
        ///// Exports to master client.
        ///// </summary>
        //[UnityEditor.MenuItem("ASL/Room Texture/Export Room Resources", false, 0)]
        //public static void ExportRawResources() {
        //    MenuHandler.ExportRawResources(PhotonNetwork.masterClient.ID);
        //}

        /////// <summary>
        /////// Processes room resources to generate final room, room prefab, and appropraite bundle.
        /////// </summary>
        ////[UnityEditor.MenuItem("ASL/Room Texture/Generate Room", false, 0)]
        ////public static void ProcessRoomResources()
        ////{
        ////    MenuHandler.ProcessRoomResources();
        ////}

        //[UnityEditor.MenuItem("ASL/Room Texture/Export Room", false, 0)]
        //public static void ExportFinalRoom() {
        //    MenuHandler.ExportRoom(PhotonNetwork.masterClient.ID);
        //}

        #region Verified
        [UnityEditor.MenuItem("ASL/Room Texture/Process All Rooms")]
        public static void ProcessAllRooms()
        {
            MenuHandler.ProcessAllRooms();
        }
#endregion
#endif
    }
}
