using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects.Android
{
    public class ZoomBehavior : MonoBehaviour
    {
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
        public float perspectiveZoomSpeed = 0.25f;
        public float orthographicZoomSpeed = 0.5f;

        public void Zoom(Touch[] touchInfos)
        {
            if(touchInfos.Length == 2)
            {
                Touch touchZero = touchInfos[0];
                Touch touchOne = touchInfos[1];

                Vector2 touchZeroPrev = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrev = touchOne.position - touchOne.deltaPosition;

                float startingMagnitude = (touchZeroPrev - touchOnePrev).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float delta = startingMagnitude - currentMagnitude;

                Camera cam = GameObject.FindObjectOfType<Camera>();
                if(cam != null)
                {
                    cam.fieldOfView += delta * perspectiveZoomSpeed;
                    cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, .1f, 179.9f);
                }
            }
        }
    }
}