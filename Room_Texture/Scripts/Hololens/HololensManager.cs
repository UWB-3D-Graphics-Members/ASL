using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWB_Texturing
{
    /// <summary>
    /// Handles startup logic of components specific to the Hololens program 
    /// for grabbing textures for the room mesh.
    /// </summary>
    public class HololensManager : MonoBehaviour
    {
        void Start()
        {
            RunRoomTexture();
        }

        void RunRoomTexture()
        {
#if WINDOWS_UWP
//#if UNITY_WSA_10_0
            TextManager.Start();
            CameraManager.Start();
#endif
        }
    }
}