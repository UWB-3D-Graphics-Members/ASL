using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    public delegate void ObjectSelectedEventHandler(ObjectSelectedEventArgs e);

    public class ObjectSelectedEventArgs : System.EventArgs
    {
        private int ownerID;
        private GameObject focusObject;
        private int focuserID;

        public ObjectSelectedEventArgs(GameObject focusObject, int ownerID, int focuserID)
        {
            this.ownerID = ownerID;
            this.focusObject = focusObject;
            this.focuserID = focuserID;
        }

        public new ObjectSelectedEventArgs Empty
        {
            get
            {
                return new ObjectSelectedEventArgs(null, 0, 0);
            }
        }

        public int OwnerID
        {
            get
            {
                return ownerID;
            }
        }
        public GameObject FocusObject
        {
            get
            {
                return focusObject;
            }
        }
        public int FocuserID
        {
            get
            {
                return focuserID;
            }
        }
    }
}