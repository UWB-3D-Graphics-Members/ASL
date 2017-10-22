using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UWBNetworkingPackage
{
    public class OwnableObject : Photon.PunBehaviour
    {
        private int SCENE_VALUE = 0;    // Hidden feature: assigning ownership to '0' makes object a scene object

        // Add object behavior components
        protected void Start()
        {
            //gameObject.AddComponent<ASL.UI.Mouse.OwnershipTransfer>();
        }

        // Fire an event when instantiated
        // Automatically transfer ownership of objects to default scene.
        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            base.OnPhotonInstantiate(info);
            this.gameObject.GetPhotonView().TransferOwnership(SCENE_VALUE);
        }

        [PunRPC]
        public void Grab(PhotonMessageInfo info)
        {
            this.RelinquishOwnership(info.sender.ID);

            //int grabbed = (int)OWNERSHIPSTATE.FAIL;

            //if (this.RequestOwnership(info) == 0)
            //    grabbed = (int)OWNERSHIPSTATE.NOW;

            //return grabbed;
        }

        public bool HasOwnership(PhotonPlayer player)
        {
            if (gameObject.GetComponent<PhotonView>() != null)
            {
                return player.Equals(gameObject.GetComponent<PhotonView>().owner);
            }
            else
            {
                return false;
            }
        }

        public PhotonPlayer GetOwner()
        {
            if (gameObject.GetComponent<PhotonView>() != null)
            {
                return gameObject.GetComponent<PhotonView>().owner;
            }
            else
            {
                return null;
            }
        }

        public void RelinquishOwnership(int newPlayerID)
        {
            // Ignore all items tagged with "room" tag
            if (this.tag.ToUpper().CompareTo("ROOM") != 0)
            {
                if (gameObject.GetPhotonView().owner.Equals(PhotonNetwork.player))
                {
                    photonView.TransferOwnership(newPlayerID);
                }
                else if (gameObject.GetPhotonView().owner.Equals(SCENE_VALUE)
                    && PhotonNetwork.isMasterClient)
                {
                    photonView.RequestOwnership();
                }
                gameObject.GetPhotonView().ownerId = newPlayerID;
            }
        }
    }
}