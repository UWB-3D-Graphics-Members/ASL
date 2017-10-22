using System;
using Photon;
using UnityEngine;

namespace ASL.Avatars.Vive
{
    // NOTE: Must come back to class to separate core behavior from PUN behavior.

    /// <summary>
    /// A script that is used for Vive Avatar instantiation. This script instantiates
    /// the user specified "ViveHeadObject" prefab (must be located in the Resources
    /// folder and matches its transform and rotation to the Vive Players Camera Rig
    /// headset.
    /// </summary>
    public class ViveHead : PunBehaviour
    {
        public GameObject ViveHeadObject;   // Object to instantiate
                                            // Must be in Resources folder

        private GameObject _cameraRig;      // Reference to the Camera Rig 
                                            // (A Vive prefab provided by SteamVR)
        private GameObject _head;           // Reference to the instance of the instantiated "ViveHeadObject"
        private Transform _cameraRigHead;   // Reference to the transform of the Camera Rig's headset
        private bool _instantiated = false; // For determining if the "ViveHeadObject" was instantiated

        // Use this for initialization
        private void Start()
        {
            try
            {
                _cameraRig = GameObject.FindGameObjectWithTag("CameraRig");
                _cameraRigHead = _cameraRig.transform.GetChild(2);
            }
            catch (Exception e)
            {
                Debug.Log("Could not find a 'CameraRig' tagged object: " + e.StackTrace);
            }
        }

        /// <summary>
        /// On joined room, instantiate a "ViveHeadObject" and store reference to the instance that was 
        /// instantiated
        /// </summary>
        public override void OnJoinedRoom()
        {
            _head = PhotonNetwork.Instantiate(ViveHeadObject.name, Vector3.zero, Quaternion.identity, 0);
            //_head.GetComponent<Renderer>().enabled = false;
            _instantiated = true;
        }

        /// <summary>
        /// Called once per frame, if a "ViveHeadObject" has been properly instantiated, 
        /// then match its postion and transform to the Camera Rig's headset's transform
        /// </summary>
        private void Update()
        {
            if (!_instantiated || _cameraRig == null) return;
            _head.transform.position = _cameraRigHead.transform.position;
            _head.transform.rotation = _cameraRigHead.transform.rotation;
        }

        #region Properties
        public Vector3 HeadPosition
        {
            get
            {
                return _head.transform.position;
            }
        }
        public Vector3 HeadDirection
        {
            get
            {
                return _head.transform.forward;
            }
        }
        #endregion
    }
}