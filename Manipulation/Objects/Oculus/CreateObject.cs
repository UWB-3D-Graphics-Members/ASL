using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Adapters.Oculus;
using VRStandardAssets.Utils;

namespace ASL.Manipulation.Objects.Oculus
{
    /// <summary>
    /// When select is clicked, this object will create a cube at (2, 0, 2)
    /// </summary>
    public class CreateObject : MonoBehaviour
    {
        /// <summary>
        /// When enable, subscribe to SelectClicked event
        /// </summary>
        void OnEnable()
        {
            OVRRemoteEventManager.SelectClicked += Create;
        }

        /// <summary>
        /// When disable, unsubscribe to SelectClicked event
        /// </summary>
        void OnDisable()
        {
            OVRRemoteEventManager.SelectClicked -= Create;
        }

        /// <summary>
        /// When SelectClicked event happen, if you are not pointing at a game object and you haven't select a gameobject, you
        /// will instantiate a cube.
        /// </summary>
        public void Create()
        {
            Debug.Log("Select On Click");
            if (VRInteractiveItem.CurrentGameObject == null && !VRInteractiveItem.GameObjectIsMoving)
            {
                PhotonNetwork.Instantiate("Cube", new Vector3(2, 0, 2), Quaternion.identity, 0);
            }
        }
    }
}