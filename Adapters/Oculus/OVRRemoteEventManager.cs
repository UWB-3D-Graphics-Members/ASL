using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using ASL.Manipulation.Objects.Oculus;

namespace ASL.Adapters.Oculus
{
    public class OVRRemoteEventManager : MonoBehaviour
    {
        #region Delegate

        public delegate void SelectOnClick();

        public delegate void BackOnClick();

        public delegate void UpOnClick();

        public delegate void DownOnClick();

        public delegate void LeftOnClick();

        public delegate void RightOnClick();

        #endregion

        #region Events

        public static event SelectOnClick SelectClicked;
        public static event BackOnClick BackClicked;
        public static event UpOnClick UpClicked;
        public static event DownOnClick DownClicked;
        public static event LeftOnClick LeftClicked;
        public static event RightOnClick RightClicked;

        #endregion

        /// <summary>
        /// Generate event when specified buttons are clicked.
        /// </summary>
        void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                if (VRInteractiveItem.CurrentGameObject != null)
                {
                    VRInteractiveItem.CurrentGameObject.GetComponent<MoveObject>().Select();
                }
                else if (SelectClicked != null)
                {
                    SelectClicked();
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                if (BackClicked != null)
                {
                    BackClicked();
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.DpadUp))
            {
                if (UpClicked != null)
                {
                    UpClicked();
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.DpadDown))
            {
                if (DownClicked != null)
                {
                    DownClicked();
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.DpadLeft))
            {
                if (LeftClicked != null)
                {
                    LeftClicked();
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.DpadRight))
            {
                if (RightClicked != null)
                {
                    RightClicked();
                }
            }
        }
    }
}