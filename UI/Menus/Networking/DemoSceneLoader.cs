using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

namespace ASL.UI.Menus.Networking
{
    public class DemoSceneLoader : MonoBehaviour
    {
        public static string levelToLoad = "PC";
        private const string CONNECTION_MENU_NAME = "ConnectionMenu";
        private const string TOGGLE_MASTER_CLIENT = "Toggle_MasterClient";
        private const string DROPDOWN_PLATFORM = "Dropdown_Platform";
        public static string SCENE_DIRECTORY;

        static DemoSceneLoader()
        {
            SCENE_DIRECTORY = Path.Combine(Directory.GetCurrentDirectory(), "ASL/Scenes");
        }

        public void Start()
        {
#if UNITY_WSA_10_0
            Toggle toggle_masterClient = GameObject.Find(CONNECTION_MENU_NAME).GetComponentInChildren<UnityEngine.UI.Toggle>();
            toggle_masterClient.isOn = false;
            toggle_masterClient.interactable = false;
#elif UNITY_ANDROID
            Toggle toggle_masterClient = GameObject.Find(CONNECTION_MENU_NAME).GetComponentInChildren<UnityEngine.UI.Toggle>();
            toggle_masterClient.isOn = false;
            toggle_masterClient.interactable = false;
#else
            Toggle toggle_masterClient = GameObject.Find(CONNECTION_MENU_NAME).GetComponentInChildren<UnityEngine.UI.Toggle>();
            toggle_masterClient.isOn = true;
            toggle_masterClient.interactable = true;
#endif
        }

        public void LoadScene()
        {
            bool isMasterClient = false;
            string levelToLoad = "";
            GameObject ConnectionMenu = GameObject.Find(CONNECTION_MENU_NAME);
            Toggle toggle_MasterClient = GameObject.Find(TOGGLE_MASTER_CLIENT).GetComponent<Toggle>();
            Dropdown dropdown_Platform = GameObject.Find(DROPDOWN_PLATFORM).GetComponent<Dropdown>();
            UWBNetworkingPackage.NodeType platform = dropdown_Platform.GetComponent<PlatformSelector>().platform;

#if UNITY_WSA_10_0
            if (toggle_MasterClient.isOn)
            {
                throw new System.Exception("Master client on Hololens platforms are not currently supported.");
            }
            switch (platform)
            {
                //case UWBNetworkingPackage.NodeType.Hololens:
                //    levelToLoad = "Hololens";
                //    break;
                default:
                    throw new System.Exception("Unsupported platform encountered in scene loader.");
            }
#elif UNITY_ANDROID
            if (toggle_MasterClient.isOn)
            {
                throw new System.Exception("Master client on Hololens platforms are not currently supported.");
            }
            switch (platform)
            {
                case UWBNetworkingPackage.NodeType.Tango:
                    levelToLoad = "Tango";
                    break;
                //case UWBNetworkingPackage.NodeType.Android:
                //    levelToLoad = "Android";
                //    break;
                default:
                    throw new System.Exception("Unsupported platform encountered in scene loader.");
            }
#else
            // PC, Vive, Oculus
            if (toggle_MasterClient.isOn)
            {
                isMasterClient = true;
            }
            switch (platform)
            {
                case UWBNetworkingPackage.NodeType.PC:
                    levelToLoad = "PC";
                    break;
                case UWBNetworkingPackage.NodeType.Vive:
                    levelToLoad = "Vive";
                    break;
                //case UWBNetworkingPackage.NodeType.Oculus:
                //    break;
                //case UWBNetworkingPackage.NodeType.Kinect:
                //    break;
                default:
                    throw new System.Exception("Unsupported platform encountered in scene loader.");
            }
#endif

            SceneVariableSetter globalVariables = transform.gameObject.GetComponent<SceneVariableSetter>();
            globalVariables.isMasterClient = isMasterClient;
            globalVariables.platform = platform;

            SceneManager.LoadScene(levelToLoad);
        }

        public List<string> GetScenesAvailable()
        {
            List<string> scenesAvailable = new List<string>();
            string[] sceneFilepaths = Directory.GetFiles(SCENE_DIRECTORY);
            foreach(string filepath in sceneFilepaths)
            {
                if (Path.GetExtension(filepath).Equals(".unity"))
                {
                    string sceneName = Path.GetFileNameWithoutExtension(filepath);
                    scenesAvailable.Add(sceneName);
                }
            }

            return scenesAvailable;
        }

        public void DisplayScenesAvailable()
        {
            List<string> sceneNames = GetScenesAvailable();
            string output = sceneNames.Count + " scenes found.";
            output += System.Environment.NewLine + "Scenes Available:";
            foreach(string sceneName in sceneNames)
            {
                output += System.Environment.NewLine + "\t" + sceneName;
            }
        }
    }
}