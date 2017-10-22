using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using ASL.Manipulation.Objects;

using ASL.Avatars.Vive;

namespace ASL.Manipulation.Controllers.Vive
{
    public class TipTracker : VRTK_ControllerEvents
    {
        //private ASL.Manipulation.Objects.CreateObject objManager;
        private ObjectInteractionManager objManager;
        private ViveHead viveHead;
        private ViveLeftHand leftHand;
        private ViveRightHand rightHand;
        public GameObject leftSelectedObject;
        public GameObject rightSelectedObject;
        
        public void Start()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();

            TriggerPressed += Select;
            TriggerReleased += Unselect;

            GameObject avatar = GameObject.Find("ViveAvatar");
            if(avatar == null)
            {
                Debug.LogError("Cannot find Vive avatar. Does it exist in the scene?");
            }

            viveHead = avatar.GetComponent<ViveHead>();
            leftHand = avatar.GetComponent<ViveLeftHand>();
            rightHand = avatar.GetComponent<ViveRightHand>();

            leftSelectedObject = null;
            rightSelectedObject = null;
        }

        protected override void Update()
        {
            base.Update();

            if(leftSelectedObject != null)
            {
                Vector3 translation = leftHand.LeftHandPosition + leftHand.LeftHandDirection * 2;
                leftSelectedObject.transform.Translate(translation);
            }
            if(rightSelectedObject != null)
            {
                Vector3 translation = rightHand.RightHandPosition + rightHand.RightHandDirection * 2;
                rightSelectedObject.transform.Translate(translation);
            }
        }

        public void Select(object sender, ControllerInteractionEventArgs e)
        {
            bool isLeftController;
            GameObject controllerAvatar = GetViveControllerAvatar(e.controllerReference, out isLeftController);

            Transform controllerTip = SetTipTransform(controllerAvatar);
            Vector3 tipPos = controllerTip.position;
            Ray tipRay = new Ray(tipPos, controllerTip.forward);
            RaycastHit hit;
            Physics.Raycast(tipRay, out hit);
            if (hit.collider != null)
            {
                if (isLeftController)
                {
                    leftSelectedObject = hit.collider.gameObject;
                }
                else
                {
                    rightSelectedObject = hit.collider.gameObject;
                }
            }

            //objManager.GetComponent<ObjectInteractionManager>().RequestOwnership(hit.collider.gameObject, PhotonNetwork.player.ID);
            objManager.RequestOwnership(hit.collider.gameObject, PhotonNetwork.player.ID);
        }

        public void Unselect(object sender, ControllerInteractionEventArgs e)
        {
            bool isLeftController;
            GameObject controllerAvatar = GetViveControllerAvatar(e.controllerReference, out isLeftController);
            //objManager.GetComponent<ObjectInteractionManager>().Focus(null, PhotonNetwork.player.ID);
            objManager.Focus(null, PhotonNetwork.player.ID);
            if (isLeftController)
            {
                leftSelectedObject = null;
            }
            else
            {
                rightSelectedObject = null;
            }
        }

        public GameObject GetViveControllerAvatar(VRTK_ControllerReference controllerReference, out bool isLeftController)
        {
            uint controllerIndex = controllerReference.index;
            GameObject controllerAvatar = null;
            isLeftController = false;
            if (GameObject.Find("ViveLeftHand").GetComponent<ViveLeftHand>().ControllerID == controllerIndex)
            {
                isLeftController = true;
                controllerAvatar = GameObject.Find("ViveLeftHand");
            }
            else if (GameObject.Find("ViveRightHand").GetComponent<ViveLeftHand>().ControllerID == controllerIndex)
            {
                isLeftController = false;
                controllerAvatar = GameObject.Find("ViveRightHand");
            }

            return controllerAvatar;
        }

        public Transform SetTipTransform(GameObject controllerObj)
        {
            return controllerObj.transform.Find("Model/tip/attach");
        }
    }
}