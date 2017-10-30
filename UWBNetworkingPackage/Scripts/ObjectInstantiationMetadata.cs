using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class ObjectInstantiationMetadata
    {
        public ObjectInfoMetadata ObjectInfo;
        public string PrefabName;
        public System.DateTime InstantiationTime;

        public ObjectInstantiationMetadata(ObjectInfoMetadata objectInfo, string prefabName, System.DateTime currentTime)
        {
            this.ObjectInfo = objectInfo;
            this.PrefabName = prefabName;
            this.InstantiationTime = currentTime;
        }
    }
}