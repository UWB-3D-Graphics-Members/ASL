using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Menus.Networking
{
    public class PlatformSelector : MonoBehaviour
    {
        public UWBNetworkingPackage.NodeType platform;

        // Use this for initialization
        void Start()
        {
            List<string> hololensOptions = GetHololensOptions();
            List<string> androidOptions = GetAndroidOptions();
            List<string> PCOptions = GetPCOptions();
            List<string> accountedForOptions = new List<string>();
            accountedForOptions.AddRange(hololensOptions);
            accountedForOptions.AddRange(androidOptions);
            accountedForOptions.AddRange(PCOptions);

            UnityEngine.UI.Dropdown dropdown = gameObject.GetComponent<UnityEngine.UI.Dropdown>();
            dropdown.ClearOptions();

#if UNITY_WSA_10_0
            dropdown.AddOptions(hololensOptions);
#elif UNITY_ANDROID
            dropdown.AddOptions(androidOptions);
#else
            dropdown.AddOptions(PCOptions);
#endif
            DisplayRestrictedOptions();
            DisplayUnaccountedForOptions(accountedForOptions);

            // Set platform
            string platformString = dropdown.options[dropdown.value].text;
            platform = (UWBNetworkingPackage.NodeType)(System.Enum.Parse(typeof(UWBNetworkingPackage.NodeType), platformString));
        }

        // Update is called once per frame
        void Update()
        {
            // Set platform
            UnityEngine.UI.Dropdown dropdown = gameObject.GetComponent<UnityEngine.UI.Dropdown>();
            string platformString = dropdown.options[dropdown.value].text;
            platform = (UWBNetworkingPackage.NodeType)(System.Enum.Parse(typeof(UWBNetworkingPackage.NodeType), platformString));
        }
        
        public List<string> GetHololensOptions()
        {
            List<string> restrictedOptions = GetRestrictedOptions();

            List<string> hololensOptions = new List<string>();
            hololensOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.Hololens));

            foreach (string option in restrictedOptions)
            {
                if (hololensOptions.Contains(option))
                {
                    hololensOptions.Remove(option);
                }
            }

            return hololensOptions;
        }

        public List<string> GetAndroidOptions()
        {
            List<string> restrictedOptions = GetRestrictedOptions();

            List<string> androidOptions = new List<string>();
            androidOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.Android));
            androidOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.Tango));

            foreach (string option in restrictedOptions)
            {
                if (androidOptions.Contains(option))
                {
                    androidOptions.Remove(option);
                }
            }

            return androidOptions;
        }

        public List<string> GetPCOptions()
        {
            List<string> restrictedOptions = GetRestrictedOptions();

            List<string> PCOptions = new List<string>();
            PCOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.PC));
            PCOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.Vive));
            PCOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.Oculus));
            PCOptions.Add(System.Enum.GetName(typeof(UWBNetworkingPackage.NodeType), UWBNetworkingPackage.NodeType.Kinect));

            foreach (string option in restrictedOptions)
            {
                if (PCOptions.Contains(option))
                {
                    PCOptions.Remove(option);
                }
            }

            return PCOptions;
        }

        public List<string> GetRestrictedOptions()
        {
            List<string> restrictedOptions = new List<string>();
            restrictedOptions.Add(UWBNetworkingPackage.NodeType.Kinect.ToString());
            restrictedOptions.Add(UWBNetworkingPackage.NodeType.Oculus.ToString());
            restrictedOptions.Add(UWBNetworkingPackage.NodeType.Android.ToString());

            return restrictedOptions;
        }

        public void DisplayRestrictedOptions()
        {
            List<string> restrictedOptions = GetRestrictedOptions();

            foreach (string option in restrictedOptions) {
                UnityEngine.Debug.LogWarning("ASL does not currently support " + option);
            }
        }

        public void DisplayUnaccountedForOptions(List<string> optionsAccountedFor)
        {
            string[] platformOptions = System.Enum.GetNames(typeof(UWBNetworkingPackage.NodeType));

            foreach (string option in platformOptions) {
                if (!optionsAccountedFor.Contains(option))
                {
                    UnityEngine.Debug.LogWarning("ASL does not account for " + option);
                }
            }
        }

        public List<string> RestrictedOptions
        {
            get
            {
                return GetRestrictedOptions();
            }
        }
    }
}