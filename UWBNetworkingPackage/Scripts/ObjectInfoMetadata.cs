using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class ObjectInfoMetadata
    {
        public string ObjectName;
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Rotation;
        public UnityEngine.Bounds BoundingBox;

        // PUN Stuff
        public int OwnerID;

        public ObjectInfoMetadata(GameObject go, int ownerID)
        {
            this.ObjectName = go.name;
            this.Position = go.transform.position;
            this.Rotation = go.transform.rotation;
            if (go.GetComponent<MeshFilter>() != null)
            {
                Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
                if (mesh == null)
                {
                    mesh = go.GetComponent<MeshFilter>().mesh;
                }

                if (mesh != null)
                {
                    this.BoundingBox = mesh.bounds;
                }
            }
            else
            {
                this.BoundingBox = new Bounds();
            }

            this.OwnerID = (ownerID < 1) ? 0 : ownerID; // associate object with scene
        }
    }
}