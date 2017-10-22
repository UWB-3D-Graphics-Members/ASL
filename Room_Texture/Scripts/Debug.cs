using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWB_Texturing
{
    public class Debug : MonoBehaviour
    {
        public static bool debugging = false;

        public static void Log(string message)
        {
            if(debugging)
                UnityEngine.Debug.Log(message);
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.Log("ERROR: " + message);
        }
    }
}