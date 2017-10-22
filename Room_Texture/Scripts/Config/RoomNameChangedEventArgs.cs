using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWB_Texturing
{
    public delegate void RoomNameChangedEventHandler(RoomNameChangedEventArgs e);

    public class RoomNameChangedEventArgs : System.EventArgs
    {
        private string oldName;
        private string newName;

        public RoomNameChangedEventArgs(string oldName, string newName)
        {
            this.oldName = oldName ?? string.Empty;
            this.newName = newName ?? string.Empty;
        }

        public new RoomNameChangedEventArgs Empty
        {
            get
            {
                return new RoomNameChangedEventArgs(string.Empty, string.Empty);
            }
        }
        public string OldName
        {
            get
            {
                return oldName;
            }
        }
        public string NewName
        {
            get
            {
                return newName;
            }
        }
    }
}