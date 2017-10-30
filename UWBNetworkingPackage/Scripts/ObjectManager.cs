using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class ObjectManager : MonoBehaviour
    {
        #region Fields
        private string resourceFolderPath;

        private List<GameObject> nonSyncItems;
        #endregion

        #region Methods
        public void Awake()
        {
            PhotonNetwork.OnEventCall += OnEvent;
            resourceFolderPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "ASL/Resources");
            
            SetNonAutoSyncItems();
        }

        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            GameObject go = Instantiate(prefabName);
            go.transform.position = position;
            go.transform.rotation = rotation;

            return go;
        }

        public GameObject Instantiate(GameObject go)
        {
            if (PhotonNetwork.connectedAndReady)
            {
                go = HandleLocalLogic(go);
                RaiseInstantiateEventHandler(go);

                return go;
            }
            else
            {
                Debug.LogError("Photon network not yet connected. State = " + PhotonNetwork.connectionState);
                return null;
            }
        }

        // Emulates PUN object creation across the PUN network
        public GameObject Instantiate(string prefabName)
        {
            if (PhotonNetwork.connectedAndReady)
            {
                GameObject localObj = InstantiateLocally(prefabName);

                if (localObj != null)
                {
                    RaiseInstantiateEventHandler(localObj);
                }
                return localObj;
            }
            else
            {
                Debug.LogError("Photon Network not yet connected. State = " + PhotonNetwork.connectionState);
                return null;
            }
        }

        // This function exists to be called when a gameobject is destroyed 
        // since OnDestroy callbacks are local to the GameObject being destroyed
        public void DestroyObject(string objectName, int[] viewIDs)
        {
            GameObject go = LocateObjectToDestroy(objectName, viewIDs[0]);
            RaiseDestroyObjectEventHandler(objectName, viewIDs);
            HandleLocalDestroyLogic(go, viewIDs);
        }

        public void ForceSyncScene(int otherPlayerID)
        {
            RaiseSyncSceneEventHandler(otherPlayerID, true);
        }
        
        #region Helper Functions
#region Non Sync Items
        private void SetNonAutoSyncItems()
        {
            nonSyncItems = new List<GameObject>();
            List<GameObject> nonSyncGOs = new List<GameObject>();

            foreach(RoomManager roomManager in GameObject.FindObjectsOfType<RoomManager>())
            {
                nonSyncGOs.Add(roomManager.gameObject);
            }
            foreach(ASL.Manipulation.Objects.ObjectInteractionManager objInteractionManager in GameObject.FindObjectsOfType<ASL.Manipulation.Objects.ObjectInteractionManager>())
            {
                nonSyncGOs.Add(objInteractionManager.gameObject);
            }
            foreach(NetworkManager networkManager in GameObject.FindObjectsOfType<NetworkManager>())
            {
                nonSyncGOs.Add(networkManager.gameObject);
            }
            foreach(ASL.Adapters.PUN.RPCManager rpcManager in GameObject.FindObjectsOfType<ASL.Adapters.PUN.RPCManager>())
            {
                nonSyncGOs.Add(rpcManager.gameObject);
            }
            foreach(ObjectManager objManager in GameObject.FindObjectsOfType<ObjectManager>())
            {
                nonSyncGOs.Add(objManager.gameObject);
            }
            foreach(Camera cam in GameObject.FindObjectsOfType<Camera>())
            {
                nonSyncGOs.Add(cam.gameObject);
            }

            nonSyncItems = RefineNonSyncGOList(nonSyncGOs);
        }

        private List<GameObject> RefineNonSyncGOList(IEnumerable<GameObject> goList)
        {
            List<GameObject> refinedGOList = new List<GameObject>();

            foreach(GameObject go in goList)
            {
                if (!refinedGOList.Contains(go))
                {
                    refinedGOList.Add(go);
                }
            }

            return refinedGOList;
        }
#endregion

        private GameObject InstantiateLocally(string prefabName)
        {
            bool connected = PhotonNetwork.connectedAndReady;
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            // safeguard
            if (!connected)
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
                return null;
            }

            // retrieve PUN object from cache
            GameObject prefabGo;
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return null;
            }
#if UNITY_EDITOR
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(prefabGo) as GameObject;
#else
            GameObject go = GameObject.Instantiate(prefabGo);
#endif
            go.name = prefabGo.name;

            HandleLocalLogic(go);
            RegisterObjectCreation(go, prefabName);

            return go;
        }

        private bool RetrieveFromPUNCache(string prefabName, out GameObject prefabGo)
        {
            bool UsePrefabCache = true;

            if (!UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out prefabGo))
            {
                //List<string> prefabFolderPossibilities = new List<string>();
                prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                if (prefabGo == null)
                {
                    string directory = resourceFolderPath;
                    //directory = ConvertToResourcePath(directory);
                    prefabGo = ResourceDive(prefabName, directory);
                }
                if (UsePrefabCache)
                {
                    PhotonNetwork.PrefabCache.Add(prefabName, prefabGo);
                }
            }

            return prefabGo != null;
        }

        private GameObject HandleLocalLogic(GameObject go)
        {
            go = HandlePUNStuff(go);
            go = SynchCustomScripts(go);

            return go;
        }

        private GameObject HandlePUNStuff(GameObject go)
        {
            return HandlePUNStuff(go, null);
        }

        private GameObject HandlePUNStuff(GameObject go, int[] viewIDs)
        {
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            if(viewIDs == null)
            {
                SetViewIDs(ref go);
            }
            else
            {
                SynchViewIDs(go, viewIDs);
            }

            AddressFinalPUNSynch(go);

            return go;
        }

#region PUN Stuff
        private GameObject ResourceDive(string prefabName, string directory)
        {
            string resourcePath = ConvertToResourcePath(directory, prefabName);
            GameObject prefabGo = (GameObject)Resources.Load(resourcePath, typeof(GameObject));

            if (prefabGo == null)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string dir in subdirectories)
                {
                    prefabGo = ResourceDive(prefabName, dir);
                    if (prefabGo != null)
                    {
                        break;
                    }
                }
            }

            return prefabGo;
        }

        private string ConvertToResourcePath(string directory, string prefabName)
        {
            string resourcePath = directory.Substring(directory.IndexOf("Resources") + "Resources".Length);
            if (resourcePath.Length > 0)
            {
                resourcePath = resourcePath.Substring(1) + '/' + prefabName;
                resourcePath.Replace('\\', '/');
            }
            else
            {
                resourcePath = prefabName;
            }

            return resourcePath;
            //return string.Join("/", directory.Split('\\')) + "/" + prefabName;
        }

        private GameObject AttachPhotonViews(GameObject go)
        {
            //NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            //Debug.Log("Attempting to attach photon view");

            // Generate and attach Photon Views
            if (go.GetComponent<PhotonView>() == null)
            {
                //Debug.Log("Photon view not found...attaching...");

                PhotonView pv = go.AddComponent<PhotonView>();
                //pv.viewID = PhotonNetwork.AllocateViewID();
                //networkingPeer.RegisterPhotonView(pv);
                pv.synchronization = ViewSynchronization.UnreliableOnChange;

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    PhotonView childPV = child.AddComponent<PhotonView>();
                    //childPV.viewID = PhotonNetwork.AllocateViewID();
                    childPV.synchronization = ViewSynchronization.UnreliableOnChange;
                }
            }

            return go;
        }

        private GameObject AttachPhotonTransformViews(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            if (go.GetComponent<UWBPhotonTransformView>() == null)
            {
                UWBPhotonTransformView ptv = go.AddComponent<UWBPhotonTransformView>();
                ptv.enableSyncPos();
                ptv.enableSyncRot();
                ptv.enableSyncScale();

                PhotonView view = go.GetComponent<PhotonView>();
                if (view.ObservedComponents == null)
                {
                    view.ObservedComponents = new List<Component>();
                }
                view.ObservedComponents.Add(ptv);

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    UWBPhotonTransformView childPTV = child.AddComponent<UWBPhotonTransformView>();
                    childPTV.enableSyncPos();
                    childPTV.enableSyncRot();
                    childPTV.enableSyncScale();

                    PhotonView childView = child.GetComponent<PhotonView>();
                    if (childView.ObservedComponents == null)
                    {
                        childView.ObservedComponents = new List<Component>();
                    }
                    childView.ObservedComponents.Add(childPTV);
                }
            }

            return go;
        }

        private GameObject SetViewIDs(ref GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            PhotonView[] views = new PhotonView[go.GetPhotonViewsInChildren().Length];
            views[0] = go.GetComponent<PhotonView>();
            for (int i = 0; i < go.transform.childCount; i++)
            {
                views[i + 1] = go.transform.gameObject.GetComponent<PhotonView>();
            }

            //Debug.Log("Found " + views.Length + " photon views in " + go.name + " object and its children");
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++) // ignore the main gameobject
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
                //Debug.Log("Allocated an id of " + viewIDs[i]);
                views[i].viewID = viewIDs[i];
                views[i].instantiationId = viewIDs[i];
                //Debug.Log("Assigning view id of " + viewIDs[i] + ", so now the view id is " + go.GetPhotonView().viewID + " for gameobject " + go.name);
                networkingPeer.RegisterPhotonView(views[i]);
            }

            return go;
        }

        private void SynchViewIDs(GameObject go, int[] viewIDs)
        {
            PhotonView[] PVs = go.GetPhotonViewsInChildren();
            for (int i = 0; i < PVs.Length; i++)
            {
                PVs[i].viewID = viewIDs[i];
            }
            if (viewIDs != null && viewIDs.Length > 0)
            {
                go.GetPhotonView().instantiationId = viewIDs[0];
            }
        }
        
        private void AddressFinalPUNSynch(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            // Send to others, create info
            //Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);
            RaiseEventOptions options = new RaiseEventOptions();
            bool isGlobalObject = false;
            options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].didAwake = false;
                //photonViews[i].viewID = 0; // why is this included in the original?

                //photonViews[i].prefix = objLevelPrefix;
                photonViews[i].prefix = networkingPeer.currentLevelPrefix;
                //photonViews[i].instantiationId = instantiationId;
                photonViews[i].instantiationId = go.GetComponent<PhotonView>().viewID;
                photonViews[i].isRuntimeInstantiated = true;
                //photonViews[i].instantiationDataField = incomingInstantiationData;
                photonViews[i].instantiationDataField = null;

                photonViews[i].didAwake = true;
                //photonViews[i].viewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
                //photonViews[i].viewID = viewIDs[i];
                photonViews[i].viewID = go.GetPhotonViewsInChildren()[i].viewID;
            }

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(PhotonNetwork.player, PhotonNetwork.ServerTimestamp, null), SendMessageOptions.DontRequireReceiver);

            // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
            //return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);
        }

#endregion

#region PUN Event Stuff
        private void RaiseInstantiateEventHandler(GameObject go)
        {
            //Debug.Log("Attempting to raise event for instantiation");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;

            byte[] content = new byte[2];
            ExitGames.Client.Photon.Hashtable instantiateEvent = new ExitGames.Client.Photon.Hashtable();
#if UNITY_EDITOR
            string prefabName = UnityEditor.PrefabUtility.GetPrefabParent(go).name;
#else
            //string prefabName = go.name;
            string prefabName = ObjectInstantiationDatabase.GetPrefabName(go);
#endif
            instantiateEvent[(byte)0] = prefabName;

            if (go.transform.position != Vector3.zero)
            {
                instantiateEvent[(byte)1] = go.transform.position;
            }

            if (go.transform.rotation != Quaternion.identity)
            {
                instantiateEvent[(byte)2] = go.transform.rotation;
            }
            
            int[] viewIDs = ExtractPhotonViewIDs(go);
            instantiateEvent[(byte)3] = viewIDs;

            if (peer.currentLevelPrefix > 0)
            {
                instantiateEvent[(byte)4] = peer.currentLevelPrefix;
            }

            instantiateEvent[(byte)5] = PhotonNetwork.ServerTimestamp;
            instantiateEvent[(byte)6] = go.GetPhotonView().instantiationId;

            //RaiseEventOptions options = new RaiseEventOptions();
            //options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            //Debug.Log("All items packed. Attempting to literally raise event now.");

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            PhotonNetwork.RaiseEvent(ASLEventCode.EV_INSTANTIATE, instantiateEvent, true, options);

            //peer.OpRaiseEvent(EV_INSTANTIATE, instantiateEvent, true, null);
        }
        
        private void RaiseSyncSceneEventHandler(int otherPlayerID, bool forceSync)
        {
            if (PhotonNetwork.isMasterClient || forceSync)
            {
                List<GameObject> ASLObjectList = GrabAllASLObjects();

                NetworkingPeer peer = PhotonNetwork.networkingPeer;
                foreach (GameObject go in ASLObjectList)
                {
                    if (nonSyncItems.Contains(go))
                    {
                        continue;
                    }

                    ExitGames.Client.Photon.Hashtable syncSceneData = new ExitGames.Client.Photon.Hashtable();

                    syncSceneData[(byte)0] = otherPlayerID;

#if UNITY_EDITOR
                    string prefabName = UnityEditor.PrefabUtility.GetPrefabParent(go).name;
#else
                    //string prefabName = go.name;
                    string prefabName = ObjectInstantiationDatabase.GetPrefabName(go);
#endif
                    UnityEngine.Debug.Log("Prefab name = " + prefabName);
                    syncSceneData[(byte)1] = prefabName;

                    if (go.transform.position != Vector3.zero)
                    {
                        syncSceneData[(byte)2] = go.transform.position;
                    }

                    if (go.transform.rotation != Quaternion.identity)
                    {
                        syncSceneData[(byte)3] = go.transform.rotation;
                    }

                    int[] viewIDs = ExtractPhotonViewIDs(go);
                    syncSceneData[(byte)4] = viewIDs;

                    if (peer.currentLevelPrefix > 0)
                    {
                        syncSceneData[(byte)5] = peer.currentLevelPrefix;
                    }

                    syncSceneData[(byte)6] = PhotonNetwork.ServerTimestamp;
                    syncSceneData[(byte)7] = go.GetPhotonView().instantiationId;

                    //RaiseEventOptions options = new RaiseEventOptions();
                    //options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

                    //Debug.Log("All items packed. Attempting to literally raise event now.");

                    RaiseEventOptions options = new RaiseEventOptions();
                    options.Receivers = ReceiverGroup.Others;
                    PhotonNetwork.RaiseEvent(ASLEventCode.EV_SYNCSCENE, syncSceneData, true, options);
                }
            }
        }

        private void RaiseDestroyObjectEventHandler(string objectName, int[] viewIDs)
        {
            //Debug.Log("Attempting to raise event for destruction of object");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;
            
            ExitGames.Client.Photon.Hashtable destroyObjectEvent = new ExitGames.Client.Photon.Hashtable();
            //string objectName = go.name;
            destroyObjectEvent[(byte)0] = objectName;

            // need the viewID
            
            destroyObjectEvent[(byte)1] = viewIDs;

            destroyObjectEvent[(byte)2] = PhotonNetwork.ServerTimestamp;

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            PhotonNetwork.RaiseEvent(ASLEventCode.EV_DESTROYOBJECT, destroyObjectEvent, true, options);
        }
        
        private void OnEvent(byte eventCode, object content, int senderID)
        {
            Debug.Log("OnEvent method triggered.");

            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log(string.Format("Custom OnEvent for CreateObject: {0}", eventCode.ToString()));
            }

            if (eventCode.Equals(ASLEventCode.EV_INSTANTIATE))
            {
                RemoteInstantiate((ExitGames.Client.Photon.Hashtable)content);
            }
            else if (eventCode.Equals(ASLEventCode.EV_DESTROYOBJECT))
            {
                RemoteDestroyObject((ExitGames.Client.Photon.Hashtable)content);
            }
            else if (eventCode.Equals(ASLEventCode.EV_JOIN))
            {
                RaiseSyncSceneEventHandler(senderID, false);
            }
            else if (eventCode.Equals(ASLEventCode.EV_SYNCSCENE))
            {
                HandleSyncSceneEvent((ExitGames.Client.Photon.Hashtable)content);
            }
        }

        private void RemoteInstantiate(ExitGames.Client.Photon.Hashtable eventData)
        {
            string prefabName = (string)eventData[(byte)0];
            Vector3 position = Vector3.zero;
            if (eventData.ContainsKey((byte)1))
            {
                position = (Vector3)eventData[(byte)1];
            }
            Quaternion rotation = Quaternion.identity;
            if (eventData.ContainsKey((byte)2))
            {
                rotation = (Quaternion)eventData[(byte)2];
            }

            int[] viewIDs = (int[])eventData[(byte)3];
            if (eventData.ContainsKey((byte)4))
            {
                uint currentLevelPrefix = (uint)eventData[(byte)4];
            }

            int serverTimeStamp = (int)eventData[(byte)5];
            int instantiationID = (int)eventData[(byte)6];

            InstantiateLocally(prefabName, viewIDs, position, rotation);
        }
        
        private void InstantiateLocally(string prefabName, int[] viewIDs, Vector3 position, Quaternion rotation)
        {
            GameObject prefabGo;
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return;
            }

#if UNITY_EDITOR
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(prefabGo) as GameObject;
#else
            GameObject go = GameObject.Instantiate(prefabGo);
#endif
            go.name = prefabGo.name;

            HandleLocalLogic(go, viewIDs);
            go.transform.position = position;
            go.transform.rotation = rotation;

            RegisterObjectCreation(go, prefabName);
        }

        private GameObject HandleLocalLogic(GameObject go, int[] viewIDs)
        {
            go = HandlePUNStuff(go, viewIDs);
            go = SynchCustomScripts(go);

            return go;
        }

        private void RemoteDestroyObject(ExitGames.Client.Photon.Hashtable eventData)
        {
            string objectName = (string)eventData[(byte)0];
            int[] viewIDs = (int[])eventData[(byte)1];
            int timeStamp = (int)eventData[(byte)2];

            // Handle destroy logic
            GameObject go = LocateObjectToDestroy(objectName, viewIDs[0]);
            HandleLocalDestroyLogic(go, viewIDs);
        }
        
        private void HandleLocalDestroyLogic(GameObject go, int[] viewIDs)
        {
            HandleLocalDestroyLogic(go);
            foreach(int viewID in viewIDs)
            {
                PhotonNetwork.networkingPeer.photonViewList.Remove(viewID);
                PhotonNetwork.UnAllocateViewID(viewID);
            }
        }
        
        private void HandleLocalDestroyLogic(GameObject go)
        {
            if (go != null)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    HandleLocalDestroyLogic(go.transform.GetChild(i).gameObject);
                }

                PhotonView view = go.GetComponent<PhotonView>();
                if (view != null)
                {
                    // Clear up the local view and delete it from the registration list
                    //PhotonNetwork.networkingPeer.LocalCleanPhotonView(view);

                    view.removedFromLocalViewList = true;
                }

                GameObject.Destroy(go);

                RegisterObjectDeletion(go, go.name);
            }
        }

        private void HandleSyncSceneEvent(ExitGames.Client.Photon.Hashtable eventData)
        {
            int targetPlayerID = (int)eventData[(byte)0];
            if (PhotonNetwork.player.ID == targetPlayerID)
            {
                string prefabName = (string)eventData[(byte)1];
                Vector3 position = Vector3.zero;
                if (eventData.ContainsKey((byte)2))
                {
                    position = (Vector3)eventData[(byte)2];
                }
                Quaternion rotation = Quaternion.identity;
                if (eventData.ContainsKey((byte)3))
                {
                    rotation = (Quaternion)eventData[(byte)3];
                }

                int[] viewIDs = (int[])eventData[(byte)4];
                if (eventData.ContainsKey((byte)5))
                {
                    uint currentLevelPrefix = (uint)eventData[(byte)5];
                }

                int serverTimeStamp = (int)eventData[(byte)6];
                int instantiationID = (int)eventData[(byte)7];

                InstantiateLocally(prefabName, viewIDs, position, rotation);
            }
        }
#endregion
        
        private GameObject LocateObjectToDestroy(string objectName, int viewID)
        {
            GameObject objectToDestroy = null;
            GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
            foreach(GameObject go in goArray)
            {
                if (go.name.Equals(objectName))
                {
                    PhotonView view = go.GetComponent<PhotonView>();
                    if(view != null)
                    {
                        if(viewID == view.viewID)
                        {
                            objectToDestroy = go;
                            break;
                        }
                    }
                }
            }

            return objectToDestroy;
        }

        private GameObject SynchCustomScripts(GameObject go)
        {
            go.AddComponent<UWBNetworkingPackage.OwnableObject>();
            go.AddComponent<UWBNetworkingPackage.DestroyObjectSynchronizer>();

            return go;
        }

        private int[] ExtractPhotonViewIDs(GameObject go)
        {
            PhotonView[] views = go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++)
            {
                viewIDs[i] = views[i].viewID;
            }

            return viewIDs;
        }
        
        private List<GameObject> GrabAllASLObjects()
        {
            List<GameObject> goList = new List<GameObject>();

            PhotonView[] views = GameObject.FindObjectsOfType<PhotonView>();
            foreach (PhotonView view in views)
            {
                GameObject go = view.gameObject;
                if (!goList.Contains(go))
                {
                    goList.Add(go);
                }
            }

            return goList;
        }

        #region Instantiation Database
        private void RegisterObjectCreation(GameObject go, string prefabName)
        {
            ObjectInstantiationDatabase.Add(prefabName, go);
        }

        private void RegisterObjectDeletion(GameObject go, string goName)
        {
            ObjectInstantiationDatabase.Remove(go, goName);
        }
#endregion
        #endregion
        #endregion
    }
}