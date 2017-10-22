using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASL.Adapters.PUN
{
    // Reference PhotonEditor.cs for the stuff that this class derives private
    // methods from
    public class RPCManager : MonoBehaviour
    {
        public List<string> RPCList;

        public static class Text
        {
            public static string IsNotRPC = "The passed RPC name is not an RPC.";
            public static class Titles
            {
                public static string RefreshRPCList = "PUN RPC List";
            }
            public static class Messages
            {
                public static string RefreshRPCList = "Refresh RPC List?";
            }
            public static class OK
            {
                public static string RefreshRPCList = "Yes";
            }
            public static class Cancel
            {
                public static string RefreshRPCList = "No";
            }
        }

        #region Methods
        public void Awake()
        {
            RefreshDisplayedRPCList();
        }

        public void FixedUpdate()
        {
#if UNITY_EDITOR
            // Make sure not to dynamically change RPC methods while any of 
            // the clients are running! Will cause a desync issue, most likely
            if (!EditorApplication.isPlaying)
            {
                RefreshRPCList();
                RefreshDisplayedRPCList();
            }
#endif
        }

        // Cannot reference PhotonEditor class (may be due to assembly linkage?) -> If you can figure out a way to reference this class, USE THEIR METHODS INSTEAD OF THE LOGIC WHICH WAS COPY PASTED
        public static void RefreshRPCList()
        {
#if UNITY_EDITOR
            //if(EditorUtility.DisplayDialog(Text.Titles.RefreshRPCList, Text.Messages.RefreshRPCList, Text.OK.RefreshRPCList))
            //{
                ClearRpcList();
                UpdateRpcList();
            //}
#else
            ClearRpcList();
            UpdateRpcList();
#endif
        }

        public static bool IsAnRPC(string RPCName)
        {
            if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(RPCName))
            {
                return true;
            }
            else
            {
                Debug.LogWarning(Text.IsNotRPC + "(" + RPCName + ")");
                return false;
            }
        }

        #region Private Methods
        private static System.Type[] GetAllSubTypesInScripts(System.Type aBaseClass)
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in AS)
            {
                // this skips all but the Unity-scripted assemblies for RPC-list creation. You could remove this to search all assemblies in project
                if (!A.FullName.StartsWith("Assembly-"))
                {
                    // Debug.Log("Skipping Assembly: " + A);
                    continue;
                }

                //Debug.Log("Assembly: " + A.FullName);
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(aBaseClass))
                    {
                        result.Add(T);
                    }
                }
            }
            return result.ToArray();
        }

        private static void UpdateRpcList()
        {
            List<string> additionalRpcs = new List<string>();
            HashSet<string> currentRpcs = new HashSet<string>();

            var types = GetAllSubTypesInScripts(typeof(MonoBehaviour));

            int countOldRpcs = 0;
            foreach (var mono in types)
            {
                MethodInfo[] methods = mono.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (MethodInfo method in methods)
                {
                    bool isOldRpc = false;
#pragma warning disable 618
                    // we let the Editor check for outdated RPC attributes in code. that should not cause a compile warning
                    if (method.IsDefined(typeof(RPC), false))
                    {
                        countOldRpcs++;
                        isOldRpc = true;
                    }
#pragma warning restore 618

                    if (isOldRpc || method.IsDefined(typeof(PunRPC), false))
                    {
                        currentRpcs.Add(method.Name);

                        if (!additionalRpcs.Contains(method.Name) && !PhotonNetwork.PhotonServerSettings.RpcList.Contains(method.Name))
                        {
                            additionalRpcs.Add(method.Name);
                        }
                    }
                }
            }

            if (additionalRpcs.Count > 0)
            {
                //// LIMITS RPC COUNT
                //if (additionalRpcs.Count + PhotonNetwork.PhotonServerSettings.RpcList.Count >= byte.MaxValue)
                //{
                //    if (currentRpcs.Count <= byte.MaxValue)
                //    {
                //        bool clearList = EditorUtility.DisplayDialog(CurrentLang.IncorrectRPCListTitle, CurrentLang.IncorrectRPCListLabel, CurrentLang.RemoveOutdatedRPCsLabel, CurrentLang.CancelButton);
                //        if (clearList)
                //        {
                //            PhotonNetwork.PhotonServerSettings.RpcList.Clear();
                //            PhotonNetwork.PhotonServerSettings.RpcList.AddRange(currentRpcs);
                //        }
                //        else
                //        {
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        EditorUtility.DisplayDialog(CurrentLang.FullRPCListTitle, CurrentLang.FullRPCListLabel, CurrentLang.SkipRPCListUpdateLabel);
                //        return;
                //    }
                //}
#if UNITY_EDITOR
                additionalRpcs.Sort();
                Undo.RecordObject(PhotonNetwork.PhotonServerSettings, "Update PUN RPC-list");
                PhotonNetwork.PhotonServerSettings.RpcList.AddRange(additionalRpcs);
                SaveSettings();
#endif
            }

            if (countOldRpcs > 0)
            {
                ConvertRpcAttribute("");

                //bool convertRPCs = EditorUtility.DisplayDialog(CurrentLang.RpcFoundDialogTitle, CurrentLang.RpcFoundMessage, CurrentLang.RpcReplaceButton, CurrentLang.RpcSkipReplace);
                //if (convertRPCs)
                //{
                //    PhotonConverter.ConvertRpcAttribute("");
                //}
            }
        }

        private static void ClearRpcList()
        {
            PhotonNetwork.PhotonServerSettings.RpcList.Clear();

            //bool clearList = EditorUtility.DisplayDialog(CurrentLang.PUNNameReplaceTitle, CurrentLang.PUNNameReplaceLabel, CurrentLang.RPCListCleared, CurrentLang.CancelButton);
            //if (clearList)
            //{
            //    PhotonNetwork.PhotonServerSettings.RpcList.Clear();
            //    Debug.LogWarning(CurrentLang.ServerSettingsCleanedWarning);
            //}
        }

        private static void SaveSettings()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(PhotonNetwork.PhotonServerSettings);
#endif
        }

        ///  default path: "Assets"
        private static void ConvertRpcAttribute(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }

            List<string> scripts = GetScriptsInFolder(path);
            foreach (string file in scripts)
            {
                string text = File.ReadAllText(file);
                string textCopy = text;
                if (file.EndsWith("PhotonConverter.cs"))
                {
                    continue;
                }

                text = text.Replace("[RPC]", "[PunRPC]");
                text = text.Replace("@RPC", "@PunRPC");

                if (!text.Equals(textCopy))
                {
                    File.WriteAllText(file, text);
                    Debug.Log("Converted RPC to PunRPC in: " + file);
                }
            }
        }

        private static List<string> GetScriptsInFolder(string folder)
        {
            List<string> scripts = new List<string>();

            try
            {
                scripts.AddRange(Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories));
                scripts.AddRange(Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories));
                scripts.AddRange(Directory.GetFiles(folder, "*.boo", SearchOption.AllDirectories));
            }
            catch (System.Exception ex)
            {
                Debug.Log("Getting script list from folder " + folder + " failed. Exception:\n" + ex.ToString());
            }

            return scripts;
        }

        private void RefreshDisplayedRPCList()
        {
            string[] rpcArray = new string[PhotonNetwork.PhotonServerSettings.RpcList.Count];
            PhotonNetwork.PhotonServerSettings.RpcList.CopyTo(rpcArray);
            RPCList = new List<string>(rpcArray);
        }
#endregion
        #endregion
    }
}