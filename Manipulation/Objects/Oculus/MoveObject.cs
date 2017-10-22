using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using ASL.Adapters.Oculus;

namespace ASL.Manipulation.Objects.Oculus
{
    public class MoveObject : MonoBehaviour
    {
        /// <summary>
        /// Stand for two move modes
        /// </summary>
        private enum Mode
        {
            Yaxis,
            Zaxis
        }


        private bool _selected = false;
        private Mode _mode = Mode.Zaxis;


        /// <summary>
        /// If an object is selected, change the move mode. If no object is selected and the pointer is over the gameobject,
        /// select the gameobject and subscribe to all button clicked events.
        /// </summary>
        public void Select()
        {
            if (_selected)
            {
                if (_mode == Mode.Yaxis)
                {
                    _mode = Mode.Zaxis;
                }
                else
                {
                    _mode = Mode.Yaxis;
                }
            }
            else if (gameObject.GetComponent<VRInteractiveItem>().IsOver)
            {
                gameObject.GetPhotonView().RequestOwnership();
                _selected = true;
                VRInteractiveItem.GameObjectIsMoving = true;
                OVRRemoteEventManager.SelectClicked += Select;
                OVRRemoteEventManager.BackClicked += Confirm;
                OVRRemoteEventManager.UpClicked += Up;
                OVRRemoteEventManager.DownClicked += Down;
                OVRRemoteEventManager.LeftClicked += Left;
                OVRRemoteEventManager.RightClicked += Right;
            }
        }

        /// <summary>
        /// When back button is clicked, confirm the modification and unsubscribe to all button clicking event
        /// </summary>
        public void Confirm()
        {
            _selected = false;
            VRInteractiveItem.GameObjectIsMoving = false;
            OVRRemoteEventManager.SelectClicked -= Select;
            OVRRemoteEventManager.BackClicked -= Confirm;
            OVRRemoteEventManager.UpClicked -= Up;
            OVRRemoteEventManager.DownClicked -= Down;
            OVRRemoteEventManager.LeftClicked -= Left;
            OVRRemoteEventManager.RightClicked -= Right;
        }

        /// <summary>
        /// Move the gameobject up when in Mode.Yaxis. Move the gameobject away when in Mode.Zaxis 
        /// </summary>
        public void Up()
        {
            var position = gameObject.transform.position;
            if (_mode == Mode.Yaxis)
            {
                gameObject.transform.position = new Vector3(position.x, position.y + 0.1f, position.z);
            }
            else
            {
                gameObject.transform.position = new Vector3(position.x, position.y, position.z + 0.1f);
            }
        }

        /// <summary>
        /// Move the gameobject down when in Mode.Yaxis. Move the gameobject toward you when in Mode.Zaxis 
        /// </summary>
        public void Down()
        {
            var position = gameObject.transform.position;
            if (_mode == Mode.Yaxis)
            {
                gameObject.transform.position = new Vector3(position.x, position.y - 0.1f, position.z);
            }
            else
            {
                gameObject.transform.position = new Vector3(position.x, position.y, position.z - 0.1f);
            }
        }

        /// <summary>
        /// Move the gameobject to left.
        /// </summary>
        public void Left()
        {
            var position = gameObject.transform.position;
            gameObject.transform.position = new Vector3(position.x - 0.1f, position.y, position.z);
        }

        /// <summary>
        /// Move the gameobject to right
        /// </summary>
        public void Right()
        {
            var position = gameObject.transform.position;
            gameObject.transform.position = new Vector3(position.x + 0.1f, position.y, position.z);
        }
    }
}