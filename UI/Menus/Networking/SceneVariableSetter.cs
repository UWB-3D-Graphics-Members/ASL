using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Menus.Networking
{
    public class SceneVariableSetter : MonoBehaviour
    {
        public bool isMasterClient = false;
        public UWBNetworkingPackage.NodeType platform;

        public void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}