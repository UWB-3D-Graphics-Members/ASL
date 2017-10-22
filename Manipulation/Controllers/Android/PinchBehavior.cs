using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects.Android;

namespace ASL.Manipulation.Controllers.Android
{
    public class PinchBehavior : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_EDITOR
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
        private ZoomBehavior zoomBehavior;

        public void Awake()
        {
            zoomBehavior = gameObject.GetComponent<ZoomBehavior>();
        }

        public void Handle(Touch[] touchInfos)
        {
            zoomBehavior.Zoom(touchInfos);
        }
#endif
    }
}