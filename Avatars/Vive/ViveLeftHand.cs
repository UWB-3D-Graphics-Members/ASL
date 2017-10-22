using System;
using Photon;
using UnityEngine;
using VRTK;

namespace ASL.Avatars.Vive
{
    /// <summary>
    /// A script that is used for Vive Avatar instantiation/tracking. This script instantiates the 
    /// user specified "ViveLeftHandObject" prefab (must be located in the Photon Resources folder), 
    /// and matches its transform and rotation to the Vive Players Camera Rig left controller. 
    /// </summary>
    public class ViveLeftHand : PunBehaviour
    {
        public GameObject ViveLeftHandObject;   // Object to instantiate
                                                // Must be in Photon Resources folder

        private GameObject _cameraRig;          // Reference to the Camera Rig 
                                                // (A Vive prefab provided by SteamVR)
        private GameObject _leftHand;           // Reference to the instance of the instantiated "ViveLeftHandObject"
        private Transform _cameraRigLeftHand;   // Reference to the transform of the Camera Rig's left controller
        private bool _instantiated = false;     // For determining if the "ViveLeftHandObject" was instantiated

        /// <summary>
        /// On start, get reference to the Camera Rig and the Camera Rig's left controller's transform (for tracking) 
        /// </summary>
        private void Start()
        {
            try
            {
                _cameraRig = GameObject.FindGameObjectWithTag("CameraRig");
                _cameraRigLeftHand = _cameraRig.transform.GetChild(0);
            }
            catch (Exception e)
            {
                Debug.Log("Could not find a 'CameraRig' tagged object: " + e.StackTrace);
            }
        }

        /// <summary>
        /// On joined room, instantiate a "ViveLeftHandObject" and store reference to the instance that was 
        /// instantiated
        /// </summary>
        public override void OnJoinedRoom()
        {
            _leftHand = PhotonNetwork.Instantiate(ViveLeftHandObject.name, Vector3.zero, Quaternion.identity, 0);
            //_leftHand.GetComponent<Renderer>().enabled = false;
            _instantiated = true;
        }

        /// <summary>
        /// Called once per frame, if a "ViveLeftHandObject" has been properly instantiated, 
        /// then match its postion and transform to the Camera Rig's left controller's transform
        /// </summary>
        private void Update()
        {
            if (!_instantiated || _cameraRig == null) return;
            _leftHand.transform.position = _cameraRigLeftHand.transform.position;
            _leftHand.transform.rotation = _cameraRigLeftHand.transform.rotation;
        }

        #region Properties
        public Vector3 LeftHandPosition
        {
            get
            {
                return _leftHand.transform.position;
            }
        }
        public Vector3 LeftHandDirection
        {
            get
            {
                return _leftHand.transform.forward;
            }
        }

        public uint ControllerID
        {
            get
            {
                VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(this.gameObject);
                uint controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
                return controllerIndex;
            }
        }
        #endregion
    }
}
