using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    public static class CustomOrientation
    {
        // Constants
        public static string SupplementaryInfoSeparator = "===";
        public static string PositionID = "MeshPositions";
        public static string RotationID = "MeshRotations";
        
        /// <summary>
        /// Save information associated with a mesh that is required for 
        /// ensuring that the correction position and rotations are applied 
        /// for each mesh generated. Writes all information to the 
        /// SupplementaryOutputFilePath.
        /// 
        /// NOTE: Loading from anything other than a TextAsset is not currently 
        /// supported by the system.
        /// </summary>
        /// <param name="positionArray">
        /// An array of Vector3's that represent the positions of the meshes 
        /// that will be instantiated.
        /// </param>
        /// <param name="rotationArray">
        /// An array of Quaternions that represent the rotations of the meshes
        /// that will be instantiated. (A quaternion uses the first three items 
        /// to formulate the axis of rotation, and the final item to determine 
        /// the degree of rotation around that axis.)
        /// </param>
        public static void Save(Vector3[] positionArray, Quaternion[] rotationArray)
        {
            string roomName = Config.RoomObject.GameObjectName;
            string fileContents = "";

            // Save the positions of instantiated meshes
            fileContents += SupplementaryInfoSeparator;
            fileContents += '\n';
            fileContents += PositionID;
            fileContents += '\n';
            for (int i = 0; i < positionArray.Length; i++)
            {
                fileContents += SupplementaryInfoSeparator;
                fileContents += '\n';
                Vector3 pos = positionArray[i];

                fileContents += Vector3ToString(pos);
            }
            // Save the rotations of instantiated meshes
            fileContents += SupplementaryInfoSeparator;
            fileContents += '\n';
            fileContents += RotationID;
            fileContents += '\n';
            for (int i = 0; i < rotationArray.Length; i++)
            {
                fileContents += SupplementaryInfoSeparator;
                fileContents += '\n';
                Quaternion rot = rotationArray[i];

                fileContents += QuaternionToString(rot);
            }

            // Actually write the calculated string
            File.WriteAllText(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName), fileContents);
        }

        /// <summary>
        /// Load information associated with a mesh that is required for
        /// ensuring that the correct position and rotations are applied 
        /// for each mesh generated from the text file.
        /// </summary>
        /// <param name="fromAsset">
        /// Boolean determining if the passed in TextAsset file will be used.
        /// </param>
        /// <param name="PositionArray">
        /// The array of Vector3's representing positions of meshes.
        /// </param>
        /// <param name="RotationArray">
        /// The array of Quaternions representing rotations of meshes.
        /// </param>
        /// <param name="asset">
        /// The optional loaded Unity TextAsset to load supplementary information from.
        /// </param>
        public static void Load(string[] fileContents, out Vector3[] PositionArray, out Quaternion[] RotationArray)
        {
            DeriveOrientation(fileContents, out PositionArray, out RotationArray);
        }

        /// <summary>
        /// Process the logic of reading the lines of the saved text file 
        /// holding the supplementary information for the room mesh. This 
        /// includes extracting the positions of the meshes and rotations 
        /// of the meshes.
        /// </summary>
        /// <param name="fileContents">
        /// The strings representing the text file holding the supplementary info.
        /// </param>
        /// <param name="positionArray">
        /// The uninitialized array to hold the positions of the matrices.
        /// </param>
        /// <param name="rotationArray">
        /// The uninitialized array to hold the rotations of the matrices.
        /// </param>
        private static void DeriveOrientation(string[] fileContents, out Vector3[] positionArray, out Quaternion[] rotationArray)
        {
            Queue<Vector3> posList = new Queue<Vector3>();
            Queue<Quaternion> rotList = new Queue<Quaternion>();

            bool usePosList = false;
            bool useRotList = false;

            int lineCount = 0;
            while (lineCount < fileContents.Length)
            {
                fileContents[lineCount] = fileContents[lineCount].TrimEnd();

                if (fileContents[lineCount].Contains(CustomMesh.SupplementaryInfoSeparator))
                {
                    ++lineCount;

                    if (fileContents[lineCount].Contains(PositionID))
                    {
                        ++lineCount;
                        usePosList = true;
                        useRotList = false;
                    }
                    else if (fileContents[lineCount].Contains(RotationID))
                    {
                        ++lineCount;
                        usePosList = false;
                        useRotList = true;
                    }
                    else
                    {
                        // update position list
                        if (usePosList)
                        {
                            Vector3 pos = new Vector3();
                            string[] lineContents = fileContents[lineCount].Split(' ');

                            for (int i = 0; i < lineContents.Length; i++)
                            {
                                lineContents[i] = lineContents[i].TrimEnd();
                            }

                            if (!float.TryParse(lineContents[0], out pos.x))
                                pos.x = 0;
                            if (!float.TryParse(lineContents[1], out pos.y))
                                pos.y = 0;
                            if (!float.TryParse(lineContents[2], out pos.z))
                                pos.z = 0;

                            posList.Enqueue(pos);
                        }
                        // update rotation list
                        else //if (useRotList)
                        {
                            Quaternion rot = new Quaternion();
                            string[] lineContents = fileContents[lineCount].Split(' ');

                            for (int i = 0; i < lineContents.Length; i++)
                            {
                                lineContents[i] = lineContents[i].TrimEnd();
                            }

                            if (!float.TryParse(lineContents[0], out rot.x))
                                rot.x = 0;
                            if (!float.TryParse(lineContents[1], out rot.y))
                                rot.y = 0;
                            if (!float.TryParse(lineContents[2], out rot.z))
                                rot.z = 0;
                            if (!float.TryParse(lineContents[3], out rot.w))
                                rot.w = 0;

                            rotList.Enqueue(rot);
                        }
                    }
                }
                else
                {
                    ++lineCount;
                }
            }

            // Make the stuff
            positionArray = posList.ToArray();
            rotationArray = rotList.ToArray();
        }

        #region Helper Methods
        /// <summary>
        /// Create a readable string representing the components of a Vector3. 
        /// Takes the format of "vectorX vectorY vectorZ" in floats.
        /// </summary>
        /// <param name="vec">
        /// The vector to create a readable string from.
        /// </param>
        /// <returns>
        /// A readable string representing the components of a Vector3.
        /// </returns>
        private static string Vector3ToString(Vector3 vec)
        {
            string vecString = vec.x.ToString() + ' ' + vec.y.ToString() + ' ' + vec.z.ToString() + '\n';

            return vecString;
        }

        /// <summary>
        /// Create a readable string representing the components of a Quaternion.
        /// Takes the format of "xAxis yAxis zAxis rotationDegrees".
        /// </summary>
        /// <param name="q">
        /// The quaternion to create a readable string from.
        /// </param>
        /// <returns>
        /// A readable string representing the components of a Quaternion.
        /// </returns>
        private static string QuaternionToString(Quaternion q)
        {
            string qString = q.x.ToString() + ' ' + q.y.ToString() + ' ' + q.z.ToString() + ' ' + q.w.ToString() + '\n';

            return qString;
        }
        #endregion
    }
}