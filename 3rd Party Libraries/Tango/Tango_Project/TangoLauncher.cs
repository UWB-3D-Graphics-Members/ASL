using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using GameDevWare.Serialization;
using Tango;
using UnityEngine;

namespace UWBNetworkingPackage {
    public class TangoLauncher : Launcher {
        [PunRPC]
        public override void SendTangoMesh() {
            UpdateMesh();

            photonView.RPC("ReceiveTangoMesh", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
        }

        private void UpdateMesh() {
            var tangoApplication =
                GameObject.Find("Tango Manager")
                    .GetComponent<TangoApplication>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color32> colors = new List<Color32>();
            List<int> triangles = new List<int>();
            tangoApplication.Tango3DRExtractWholeMesh(vertices, normals, colors,
                triangles);
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.colors32 = colors.ToArray();
            mesh.triangles = triangles.ToArray();
            List<Mesh> meshList = new List<Mesh>();
            meshList.Add(mesh);
            
            TangoDatabase.UpdateMesh(meshList);
            Debug.Log("Mesh Updated");
        }
    }
}

