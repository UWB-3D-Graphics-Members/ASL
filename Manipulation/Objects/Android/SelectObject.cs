using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects.Android
{
    public class SelectObject : MonoBehaviour
    {
        private ObjectInteractionManager objManager;

        public void Awake()
        {
            objManager = gameObject.GetComponent<ObjectInteractionManager>();
        }

        public GameObject Select(Touch touchInfo)
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            Vector3 tapPos = touchInfo.position;
            Ray ray = cam.ScreenPointToRay(tapPos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else
            {
                GameObject camera = GameObject.Find("Main Camera");
                if (camera != null)
                {
                    return camera;
                }
                else
                {
                    Debug.LogError("Cannot find camera object. Selecting null object.");
                    return null;
                }
            }
        }

        public void Unselect(Touch touchInfo)
        {
            objManager.Focus(null, PhotonNetwork.player.ID);
        }
    }
}