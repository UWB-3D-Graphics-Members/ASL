using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public static class ObjectInstantiationDatabase
    {
        public static Dictionary<string, List<KeyValuePair<ObjectInstantiationMetadata, GameObject>>> ObjectDatabase;
        public static Dictionary<GameObject, string> PrefabLookupTable;
        private static System.DateTime lastUpdate;

        static ObjectInstantiationDatabase()
        {
            //ObjectDatabase = new Dictionary<string, ObjectInstantiationMetadata>();
            ObjectDatabase = new Dictionary<string, List<KeyValuePair<ObjectInstantiationMetadata, GameObject>>>();
            PrefabLookupTable = new Dictionary<GameObject, string>();
            lastUpdate = System.DateTime.MinValue;
        }

        public static void Add(string prefabName, GameObject go)
        {
            string objectName = go.name;

            int ownerID = 0;
            PhotonView view = go.GetComponent<PhotonView>();
            if (view != null)
            {
                ownerID = view.ownerId;
            }
            ObjectInfoMetadata objectInfo = new ObjectInfoMetadata(go, ownerID);

            System.DateTime currentTime = System.DateTime.Now;

            ObjectInstantiationMetadata instantiationMetadata = new ObjectInstantiationMetadata(objectInfo, prefabName, currentTime);
            
            if (!ObjectDatabase.ContainsKey(objectName))
            {
                ObjectDatabase.Add(objectName, new List<KeyValuePair<ObjectInstantiationMetadata, GameObject>>());
            }

            ObjectDatabase[objectName].Add(new KeyValuePair<ObjectInstantiationMetadata, GameObject>(instantiationMetadata, go));
            PrefabLookupTable.Add(go, prefabName);

            lastUpdate = System.DateTime.Now;
        }

        public static bool Remove(GameObject go)
        {
            if(go == null)
            {
                return false;
            }
            else if (ObjectDatabase.ContainsKey(go.name))
            {
                bool removed = false;

                string objectName = go.name;
                List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> valuePairList = ObjectDatabase[objectName];
                foreach(KeyValuePair<ObjectInstantiationMetadata, GameObject> valuePair in valuePairList)
                {
                    if (valuePair.Value.Equals(go))
                    {
                        ObjectDatabase[objectName].Remove(valuePair);
                        removed = true;
                        if (PrefabLookupTable.ContainsKey(go))
                        {
                            PrefabLookupTable.Remove(go);
                        }
                        lastUpdate = System.DateTime.Now;
                        break;
                    }
                }

                return removed;
            }
            else
            {
                return true;
            }
        }

        public static bool Remove(GameObject go, string goName)
        {
            if (string.IsNullOrEmpty(goName))
            {
                return false;
            }
            else if(go != null)
            {
                return Remove(go);
            }
            else if (ObjectDatabase.ContainsKey(goName))
            {
                bool removed = false;
                List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> valuePairList = ObjectDatabase[goName];
                foreach (KeyValuePair<ObjectInstantiationMetadata, GameObject> valuePair in valuePairList)
                {
                    if (valuePair.Value == null)
                    {
                        ObjectDatabase[goName].Remove(valuePair);
                        removed = true;
                        lastUpdate = System.DateTime.Now;
                    }
                }

                if (PrefabLookupTable.ContainsKey(null))
                {
                    PrefabLookupTable.Remove(null);
                }

                return removed;
            }
            else
            {
                return true;
            }
        }

        public static ObjectInstantiationMetadata Get(GameObject go)
        {
            if (go != null)
            {
                string objectDatabaseKey = go.name;
                if (ObjectDatabase.ContainsKey(objectDatabaseKey))
                {
                    List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> pairList = ObjectDatabase[objectDatabaseKey];
                    foreach (KeyValuePair<ObjectInstantiationMetadata, GameObject> pair in pairList)
                    {
                        if (pair.Value.Equals(go))
                        {
                            return pair.Key;
                        }
                    }
                }

                UnityEngine.Debug.LogError("Object Instantiation metadata not found!");
                return null;
            }
            else
            {
                return null;
            }
        }

        public static List<ObjectInstantiationMetadata> GetAll(string objectName)
        {
            List<ObjectInstantiationMetadata> metadataList = new List<ObjectInstantiationMetadata>();
            if (ObjectDatabase.ContainsKey(objectName))
            {
                List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> pairList = ObjectDatabase[objectName];
                foreach(KeyValuePair<ObjectInstantiationMetadata, GameObject> pair in pairList)
                {
                    metadataList.Add(pair.Key);
                }
            }

            return metadataList;
        }

        public static string GetPrefabName(GameObject go)
        {
            if(go != null)
            {
                if (PrefabLookupTable.ContainsKey(go))
                {
                    return PrefabLookupTable[go];
                }
            }

            UnityEngine.Debug.LogError("GameObject prefab name not found.");
            return null;
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
    }
}