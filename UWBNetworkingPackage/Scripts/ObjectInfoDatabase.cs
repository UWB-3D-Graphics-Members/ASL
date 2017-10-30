using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public static class ObjectInfoDatabase
    {
        public static Dictionary<string, ObjectInfoMetadata> ObjectDatabase;
        private static System.DateTime lastUpdate;

        static ObjectInfoDatabase()
        {
            ObjectDatabase = new Dictionary<string, ObjectInfoMetadata>();
            lastUpdate = System.DateTime.MinValue;
        }

        public static void Add(GameObject go)
        {
            string objectName = go.name;

            int ownerID = 0;
            PhotonView view = go.GetComponent<PhotonView>();
            if(view != null)
            {
                ownerID = view.ownerId;
            }
            ObjectInfoMetadata objectInfo = new ObjectInfoMetadata(go, ownerID);

            if (ObjectDatabase.ContainsKey(objectName))
            {
                ObjectDatabase.Remove(objectName);
            }
            ObjectDatabase.Add(objectName, objectInfo);

            lastUpdate = System.DateTime.Now;
        }

        public static void Remove(GameObject go)
        {
            if (ObjectDatabase.ContainsKey(go.name))
            {
                ObjectDatabase.Remove(go.name);
                lastUpdate = System.DateTime.Now;
            }
        }

        public static ObjectInfoMetadata Get(string objectName)
        {
            if (ObjectDatabase.ContainsKey(objectName))
            {
                return ObjectDatabase[objectName];
            }
            else
            {
                return null;
            }
        }

        public static bool Contains(string objectName)
        {
            return ObjectDatabase.ContainsKey(objectName);
        }

        public static System.DateTime UpdateTime
        {
            get
            {
                return lastUpdate;
            }
        }

        public static bool Empty
        {
            get
            {
                return ObjectDatabase.Count < 1;
            }
        }

        public static int Count
        {
            get
            {
                return ObjectDatabase.Count;
            }
        }
    }
}