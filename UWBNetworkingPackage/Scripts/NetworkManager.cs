using System;
using Photon;
using UnityEngine;
//using UnityEditor;



namespace UWBNetworkingPackage
{
    /// <summary>
    /// NetworkManager adds the correct Launcher script based on the user selected device (in the Unity Inspector)
    /// </summary>
    // Note: For future improvement, this class should: a) Detect the device and add the launcher automatically; 
    // or b) Only allow user to select one device
    [System.Serializable]
    public class NetworkManager : PunBehaviour
    {
        #region Public Properties

        public bool MasterClient = true;

        // Needed for Room Mesh sending
        [Tooltip("A port number for devices to communicate through. The port number should be the same for each set of projects that need to connect to each other and share the same Room Mesh.")]
        public int Port;

        // Needed for Photon 
        [Tooltip("The name of the room that this project will attempt to connect to. This room must be created by a \"Master Client\".")]
        public string RoomName;
        #endregion

        private const string SCENE_LOADER_NAME = "SceneLoaderObject";
        private ASL.UI.Menus.Networking.SceneVariableSetter globalVariables;
        private ObjectManager objManager;

        /// <summary>
        /// When Awake, NetworkManager will add the correct Launcher script
        /// </summary>
        void Awake()
        {
            GameObject MenuUI = GameObject.Find(SCENE_LOADER_NAME);
            if (MenuUI != null)
            {
                globalVariables = MenuUI.GetComponent<ASL.UI.Menus.Networking.SceneVariableSetter>();
                MasterClient = globalVariables.isMasterClient;
                UWBNetworkingPackage.NodeType platform = globalVariables.platform;
                Config.Start(platform);
                //MasterClient = MenuUI.GetComponent<ASL.UI.Menus.Networking.SceneVariableSetter>().isMasterClient;
            }
            else
            {
#if !UNITY_EDITOR && UNITY_WSA_10_0
                Config.Start(NodeType.Hololens);
#elif !UNITY_EDITOR && UNITY_ANDROID
                Config.Start(NodeType.Tango);
#else
                Config.Start(NodeType.PC);
#endif
            }

            objManager = gameObject.AddComponent<ObjectManager>();
            
            //Preprocessor directives to choose which component is added.  Note, master client still has to be hard coded
            //Haven't yet found a better solution for this

#if !UNITY_WSA_10_0 && !UNITY_ANDROID
            RoomHandler.Start();

            if (MasterClient)
            {
                gameObject.AddComponent<MasterClientLauncher_PC>();
                //new Config.AssetBundle.Current(); // Sets some items
            }
            else
            {
                gameObject.AddComponent<ReceivingClientLauncher_PC>();
                // get logic for setting nodetype appropriately

                // new Config.AssetBundle.Current(); // Sets some items
            }
#elif !UNITY_EDITOR && UNITY_WSA_10_0
            RoomHandler.Start();

            if (MasterClient)
            {
                gameObject.AddComponent<MasterClientLauncher_Hololens>();
            }
            else
            {
                gameObject.AddComponent<ReceivingClientLauncher_Hololens>();
            }
            //gameObject.AddComponent<HoloLensLauncher>();

            //UWB_Texturing.TextManager.Start();

            //// ERROR TESTING REMOVE
            //string[] filelines = new string[4];
            //filelines[0] = "Absolute asset root folder = " + Config_Base.AbsoluteAssetRootFolder;
            //filelines[1] = "Private absolute asset root folder = " + Config_Base.absoluteAssetRootFolder;
            //filelines[2] = "Absolute asset directory = " + Config.AssetBundle.Current.CompileAbsoluteAssetDirectory();
            //filelines[3] = "Absolute bundle directory = " + Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();

            //string filepath = System.IO.Path.Combine(Application.persistentDataPath, "debugfile.txt");
            //System.IO.File.WriteAllLines(filepath, filelines);
#elif UNITY_ANDROID
            bool isTango = true;
            if (isTango)
            {
                //gameObject.AddComponent<TangoLauncher>();
                //Config.Start(NodeType.Tango);
                RoomHandler.Start();
                if (MasterClient)
                {
                    throw new System.Exception("Tango master client not yet implemented! If it is, then update NetworkManager where you see this error message.");
                }
                else { 
                    gameObject.AddComponent<ReceivingClientLauncher_Tango>();
                }
            }
            else
            {
               // Config.Start(NodeType.Android);
                RoomHandler.Start();
                if (MasterClient)
                {
                    throw new System.Exception("Android master client not yet implemented! If it is, then update NetworkManager where you see this error message.");
                }
                else
                {
                    gameObject.AddComponent<ReceivingClientLauncher_Android>();
                }
            }
#else
            RoomHandler.Start();

            if (MasterClient)
            {
                gameObject.AddComponent<MasterClientLauncher_PC>();
                //new Config.AssetBundle.Current(); // Sets some items
            }
            else
            {
                gameObject.AddComponent<ReceivingClientLauncher_PC>();
                // get logic for setting nodetype appropriately

                // new Config.AssetBundle.Current(); // Sets some items
            }
#endif
        }

        protected void OnApplicationQuit()
        {
#if UNITY_WSA_10_0 && !UNITY_EDITOR
            ServerFinder_Hololens.KillThreads();
#else
            ServerFinder.KillThreads();
#endif
        }

        public void Instantiate(GameObject go)
        {
            if(objManager != null)
            {
                objManager.Instantiate(go);
            }
        }

        public void Instantiate(string prefabName)
        {
            if(objManager != null)
            {
                objManager.Instantiate(prefabName);
            }
        }

        public void Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            if(objManager != null)
            {
                objManager.Instantiate(prefabName, position, rotation);
            }
        }

        public void RequestOwnership(GameObject obj, int focuserID)
        {
            if (obj.GetPhotonView() != null)
            {
                obj.GetPhotonView().RPC("Grab", PhotonTargets.Others);
            }
        }

        //-----------------------------------------------------------------------------
        // Legacy Code:

        ///// <summary>
        ///// This is a HoloLens specific method
        ///// This method allows a HoloLens developer to send a Room Mesh when triggered by an event
        ///// This is here because HoloLensLauncher is applied at runtime
        ///// In the HoloLensDemo, this method is called when the phrase "Send Mesh" is spoken and heard by the HoloLens
        ///// </summary>
        //#if UNITY_WSA_10_0
        //        public void HoloSendMesh()
        //        { 
        //            gameObject.GetComponent<MasterClientLauncher_Hololens>().SendMesh();

        //        }
        //#endif
    }
}