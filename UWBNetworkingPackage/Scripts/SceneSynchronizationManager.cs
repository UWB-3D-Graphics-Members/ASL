using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// This class is currently empty, but is intended as a placeholder for 
    /// future logic that handles scene synchronization beyond the basic 
    /// object synchronization encapsulated in NetworkManager's ObjectManager. 
    /// 
    /// Examples of this logic may be restricting kinds of nodes that may 
    /// join/sync to a given room/configuration, saving/loading scenes, 
    /// checkpointing work in case of network disconnections/crashes, and 
    /// reconciliation of items that conflicting scene synchronization records 
    /// (i.e. master client synchronization is from an earlier checkpoint than 
    /// the joining client or a disconnection caused a branching update tree 
    /// that needs to be resolved).
    /// </summary>
    public class SceneSynchronizationManager : MonoBehaviour
    {
        private ObjectManager objManager;

        public void Awake()
        {
            objManager = GameObject.FindObjectOfType<ObjectManager>();
        }

        // Insert logic here
    }
}