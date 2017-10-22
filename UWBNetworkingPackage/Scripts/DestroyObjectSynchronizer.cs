using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// This class exists solely so that the networking component can trigger 
    /// necessary methods when an object in the scene is destroyed.
    /// </summary>
    public class DestroyObjectSynchronizer : MonoBehaviour
    {
        private const string NAME_NETWORKMANAGER = "NetworkManager";
        private ObjectManager objManager;

        public void Awake()
        {
            objManager = GameObject.Find(NAME_NETWORKMANAGER).GetComponent<ObjectManager>();
        }

        public void OnDestroy()
        {
            string objName = this.gameObject.name;
            PhotonView view = gameObject.GetComponent<PhotonView>();
            if(view != null)
            {
                PhotonView[] views = gameObject.GetPhotonViewsInChildren();
                int[] viewIDs = new int[views.Length];
                for(int i = 0; i < views.Length; i++)
                {
                    viewIDs[i] = views[i].viewID;
                }

                //int viewID = view.viewID;
                objManager.DestroyObject(this.name, viewIDs);
            }
        }
    }
}