using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects.Android;

namespace ASL.Manipulation.Controllers.Android
{
    public class TapBehavior : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_EDITOR
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
        private SelectObject selectBehavior;
        private MoveBehavior moveBehavior;

        private Vector3 touchBeginPosition;
        private Vector3 touchEndPosition;

        public void Awake()
        {
            objManager = gameObject.GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
            selectBehavior = gameObject.GetComponent<SelectObject>();
        }

        public void Handle(Touch touchInfo)
        {
            switch (touchInfo.phase)
            {
                case TouchPhase.Began:
                    touchBeginPosition = touchInfo.position;
                    break;
                case TouchPhase.Stationary:
                    GameObject go = selectBehavior.Select(touchInfo);
                    objManager.RequestOwnership(go, PhotonNetwork.player.ID);
                    break;
                case TouchPhase.Moved:
                    moveBehavior.Drag(touchInfo.deltaPosition);
                    break;
                case TouchPhase.Ended:
                    touchEndPosition = touchInfo.position;
                    selectBehavior.Unselect(touchInfo);
                    break;
            }
        }

        #region Properties
        public Vector3 TouchBegin
        {
            get
            {
                return touchBeginPosition;
            }
        }
        public Vector3 TouchEnd
        {
            get
            {
                return touchEndPosition;
            }
        }
#endregion
#endif
    }
}