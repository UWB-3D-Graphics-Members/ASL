using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    public class MoveBehavior : MonoBehaviour
    {
        
        public GameObject focusObject;
        private float moveScale = 0.10f;
        private float rotateScale = 15.0f;

        public virtual void Awake()
        {
            GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>().FocusObjectChangedEvent += SetObject;
        }

        private void SetObject(ObjectSelectedEventArgs e)
        {
            focusObject = e.FocusObject;
        }

        public virtual void Up()
        {
            if(focusObject != null)
            {
                focusObject.transform.Translate(Vector3.up * MoveScale);
            }
        }

        public virtual void Down()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.down * MoveScale);
            }
        }

        public virtual void Left()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.left * MoveScale);
            }
        }

        public virtual void Right()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.right * MoveScale);
            }
        }

        public virtual void RotateClockwise()
        {
            if(focusObject != null)
            {
                focusObject.transform.Rotate(Vector3.up, RotateScale);
            }
        }

        public virtual void RotateCounterClockwise()
        {
            if(focusObject != null)
            {
                focusObject.transform.Rotate(Vector3.up, RotateScale * -1);
            }
        }

        public virtual void Drag(Vector3 deltaPosition)
        {
            if(focusObject != null)
            {
                focusObject.transform.Translate(deltaPosition);
            }
        }

        #region Properties
        public float MoveScale
        {
            get
            {
                return moveScale;
            }
            set
            {
                if (value > 0.0f)
                {
                    moveScale = value;
                }
            }
        }

        public float RotateScale
        {
            get
            {
                return rotateScale;
            }
            set
            {
                if(value > 0.0f)
                {
                    rotateScale = value;
                }
            }
        }
#endregion
    }
}