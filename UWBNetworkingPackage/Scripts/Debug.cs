using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class Debug : MonoBehaviour
    {
        public static bool debugging = false;

        public static void Log(string message)
        {
#if !UNITY_EDITOR && UNITY_WSA_10_0
            if (debugging)
            {
                if (UWB_Texturing.TextManager.IsActive)
                {
                    UWB_Texturing.TextManager.SetText(message);
                }
                UnityEngine.Debug.Log(message);
            }
#else
            if(debugging)
                UnityEngine.Debug.Log(message);
#endif
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.Log("ERROR: " + message);
        }

        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}