using System;
using System.Collections.Generic;
using System.IO;
using SysDiag = System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkingPackage {
    public static class SimpleMeshSerializerTango {
        /// <summary>
        /// The mesh header consists of two 32 bit integers.
        /// </summary>
        private static int HeaderSize = sizeof(int) * 2;

        /// <summary>
        /// Serializes a list of Mesh objects into a byte array.
        /// </summary>
        /// <param name="meshes">List of Mesh objects to be serialized.</param>
        /// <returns>Binary representation of the Mesh objects.</returns>
        public static byte[] Serialize(IEnumerable<Mesh> meshes) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    foreach (Mesh mesh in meshes)
                    {
                        WriteMesh(writer, mesh);
                    }

                stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }

        /// <summary>
        /// Deserializes a list of Mesh objects from the provided byte array.
        /// </summary>
        /// <param name="data">Binary data to be deserialized into a list of Mesh objects.</param>
        /// <returns>List of Mesh objects.</returns>
        public static IEnumerable<Mesh> Deserialize(byte[] data) {
            List<Mesh> meshes = new List<Mesh>();

            using (MemoryStream stream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    while (reader.BaseStream.Length - reader.BaseStream.Position >= HeaderSize) {
                        meshes.Add(ReadMesh(reader));
                    }
                }
            }

            return meshes;
        }

        /// <summary>
        /// Writes a Mesh object to the data stream.
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="mesh">The Mesh object to be written.</param>
        private static void WriteMesh(BinaryWriter writer, Mesh mesh) {
            SysDiag.Debug.Assert(writer != null);

            // Write the mesh data.
            WriteMeshHeader(writer, mesh.vertexCount, mesh.triangles.Length, mesh.normals.Length, mesh.colors32.Length);
            WriteVertices(writer, mesh.vertices);
            WriteTriangleIndicies(writer, mesh.triangles);
            WriteNormalIndicies(writer, mesh.normals);
            WriteColor32Indicies(writer, mesh.colors32);
        }

        /// <summary>
        /// Reads a single Mesh object from the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <returns>Mesh object read from the stream.</returns>
        private static Mesh ReadMesh(BinaryReader reader) {
            SysDiag.Debug.Assert(reader != null);

            int vertexCount = 0;
            int triangleIndexCount = 0;
            int normalCount = 0;
            int color32Count = 0;

            // Read the mesh data.
            ReadMeshHeader(reader, out vertexCount, out triangleIndexCount, out normalCount, out color32Count);
            Vector3[] vertices = ReadVertices(reader, vertexCount);
            int[] triangleIndices = ReadTriangleIndicies(reader, triangleIndexCount);
            Vector3[] normals = ReadNormalIndicies(reader, normalCount);
            Color32[] color32 = ReadColor32Indicies(reader, color32Count);

            // Create the mesh.
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangleIndices;
            mesh.normals = normals;
            mesh.colors32 = color32;

            mesh.name = "TangoMesh";

            return mesh;
        }

        /// <summary>
        /// Writes a mesh header to the data stream.
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="vertexCount">Count of vertices in the mesh.</param>
        /// <param name="triangleIndexCount">Count of triangle indices in the mesh.</param>
        private static void WriteMeshHeader(BinaryWriter writer, int vertexCount, int triangleIndexCount, int normalCount, int color32Count) {
            SysDiag.Debug.Assert(writer != null);
            writer.Write(vertexCount);
            writer.Write(triangleIndexCount);
            writer.Write(normalCount);
            writer.Write(color32Count);
        }

        /// <summary>
        /// Reads a mesh header from the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="vertexCount">Count of vertices in the mesh.</param>
        /// <param name="triangleIndexCount">Count of triangle indices in the mesh.</param>
        private static void ReadMeshHeader(BinaryReader reader, out int vertexCount, out int triangleIndexCount, out int normalCount, out int color32Count) {
            SysDiag.Debug.Assert(reader != null);

            vertexCount = reader.ReadInt32();
            triangleIndexCount = reader.ReadInt32();
            normalCount = reader.ReadInt32();
            color32Count = reader.ReadInt32();
        }

        /// <summary>
        /// Writes a mesh's vertices to the data stream.
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="vertices">Array of Vector3 structures representing each vertex.</param>
        private static void WriteVertices(BinaryWriter writer, Vector3[] vertices) {
            SysDiag.Debug.Assert(writer != null);

            foreach (Vector3 vertex in vertices) {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
                writer.Write(vertex.z);
            }
        }

        /// <summary>
        /// Reads a mesh's vertices from the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="vertexCount">Count of vertices to read.</param>
        /// <returns>Array of Vector3 structures representing the mesh's vertices.</returns>
        private static Vector3[] ReadVertices(BinaryReader reader, int vertexCount) {
            SysDiag.Debug.Assert(reader != null);

            Vector3[] vertices = new Vector3[vertexCount];

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = new Vector3(reader.ReadSingle(),
                                        reader.ReadSingle(),
                                        reader.ReadSingle());
            }

            return vertices;
        }

        /// <summary>
        /// Writes the vertex indices that represent a mesh's triangles to the data stream
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="triangleIndices">Array of integers that describe how the vertex indices form triangles.</param>
        private static void WriteTriangleIndicies(BinaryWriter writer, int[] triangleIndices) {
            SysDiag.Debug.Assert(writer != null);

            foreach (int index in triangleIndices) {
                writer.Write(index);
            }
        }

        /// <summary>
        /// Reads the vertex indices that represent a mesh's triangles from the data stream
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="triangleIndexCount">Count of indices to read.</param>
        /// <returns>Array of integers that describe how the vertex indices form triangles.</returns>
        private static int[] ReadTriangleIndicies(BinaryReader reader, int triangleIndexCount) {
            SysDiag.Debug.Assert(reader != null);

            int[] triangleIndices = new int[triangleIndexCount];

            for (int i = 0; i < triangleIndices.Length; i++) {
                triangleIndices[i] = reader.ReadInt32();
            }

            return triangleIndices;
        }

        private static void WriteNormalIndicies(BinaryWriter writer, Vector3[] normals) {
            foreach (Vector3 normal in normals) {
                writer.Write(normal.x);
                writer.Write(normal.y);
                writer.Write(normal.z);
            }
        }

        private static Vector3[] ReadNormalIndicies(BinaryReader reader, int normalsCount) {
            if (normalsCount <= 0) return null;

            Vector3[] normalIndices = new Vector3[normalsCount];

            for (int i = 0; i < normalIndices.Length; i++) {
                normalIndices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }

            return normalIndices;
        }

        private static void WriteColor32Indicies(BinaryWriter writer, Color32[] colors32) {
            foreach (var color32 in colors32) {
                writer.Write(color32.r);
                writer.Write(color32.g);
                writer.Write(color32.b);
                writer.Write(color32.a);
            }
        }

        private static Color32[] ReadColor32Indicies(BinaryReader reader, int colorsCount) {
            if (colorsCount <= 0) return null;

            Color32[] color32Indicies = new Color32[colorsCount];

            for (int i = 0; i < color32Indicies.Length; i++) {
                color32Indicies[i] = new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            }

            return color32Indicies;
        }
    }
}
