using UnityEngine;
using System.Collections;
using VRTK;

using ASL.Avatars.Vive;

namespace ASL.Manipulation.Controllers.Vive
{
    /// <summary>
    /// Script to be added to the Camera Rig's Right and Left controllers. Adds object instantiation
    /// functionality on touchpad press. 
    /// </summary>
    public class TouchpadBehavior : VRTK_ControllerEvents
    {
        //private ASL.Manipulation.Objects.CreateObject objManager;
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;

        public string PrefabString = "Sphere"; //"Trophy";
        private string ViveAvatarName = "ViveAvatar";

        /// <summary>
        /// Subscribe TouchPad events to be called appropriately.
        /// </summary>
        void Start()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();

            TouchpadPressed += OnTouchpadPressedHandler;
            //objManager = gameObject.GetComponent<ASL.Manipulation.Objects.CreateObject>();
        }

        /// <summary>
        /// When the touchpad is pressed, instantiate a prefab
        /// </summary>
        /// <param name="obj">Reference to the controller that pressed the touchpad</param>
        /// <param name="e">Controller event arguments</param>
        public void OnTouchpadPressedHandler(object obj, ControllerInteractionEventArgs e)
        {
            // Generate a prefab at the origin
            //PhotonNetwork.Instantiate(PrefabString, Vector3.zero, Quaternion.identity, 0);

            // Generate a prefab two feet in front of your face
            Vector3 HeadPos = GameObject.Find(ViveAvatarName).GetComponent<ViveHead>().HeadPosition;
            Vector3 PrefabPos = HeadPos + new Vector3(0, 1, 0);
            //PhotonNetwork.Instantiate(PrefabString, PrefabPos, Quaternion.identity, 0);
            //objManager.CreatePUNObject(PrefabString, PrefabPos, Quaternion.identity);
            objManager.Instantiate(PrefabString, PrefabPos, Quaternion.identity);
        }
    }
}
