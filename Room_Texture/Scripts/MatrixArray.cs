using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Handles saving and loading of matrix arrays.
    /// </summary>
    public static class MatrixArray
    {
        #region Constants
        public static string WorldToCameraMatrixID = "WorldToCameraMatrices";
        public static string ProjectionMatrixID = "ProjectorMatrices";
        public static string MatrixSeparator = "===";
        public static string LocalToWorldMatrixID = "LocalToWorldMatrices";
        #endregion

        #region Methods
        /// <summary>
        /// Writes the LocalToWorldMatrixArray (matrices translate model/local 
        /// space coordinates to world space coordinates), 
        /// WorldToCameraMatrixArray (matrices translate world space coordinates 
        /// to camera/view space), and ProjectionMatrixArray (matrices translate 
        /// camera/view space coordinates to clip space).
        /// </summary>
        /// <param name="worldToCameraMatrixArray">
        /// Matrix array where matrices translate world space coordinates to 
        /// camera/view space. Each matrix is associated with a different 
        /// camera/texture.
        /// </param>
        /// <param name="projectionMatrixArray">
        /// Matrix array where matrices translate camera/view space coordinates 
        /// to clip space. Each matrix is associated with a different 
        /// camera/texture.
        /// </param>
        /// <param name="localToWorldMatrixArray">
        /// Matrix array where matrices translate local/model space coordinates 
        /// to world space. Each matrix is associated with a different MESH 
        /// (not camera/texture).
        /// </param>
        /// <param name="numPhotosTaken">
        /// Number of photos taken on the Hololens. Used to determine number 
        /// of worldToCamera and projection matrices to extract from corresponding 
        /// matrix arrays (i.e. matrix arrays are presumed to always be the 
        /// maximum size number of matrices wide).
        /// </param>
        public static void SaveMatrixArrays(Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray, int numPhotosTaken)
        {
            string roomName = Config.RoomObject.GameObjectName;

            // Safeguard
            if(numPhotosTaken > worldToCameraMatrixArray.Length
                || numPhotosTaken > projectionMatrixArray.Length)
            {
                numPhotosTaken = (worldToCameraMatrixArray.Length > projectionMatrixArray.Length)
                    ? projectionMatrixArray.Length
                    : worldToCameraMatrixArray.Length;
            }
            
            string fileContents = "";

            // Write WorldToCameraMatrixArray
            fileContents += MatrixSeparator;
            fileContents += '\n';
            fileContents += WorldToCameraMatrixID;
            fileContents += '\n';
            for (int i = 0; i < numPhotosTaken; i++)
            {
                fileContents += MatrixSeparator;
                fileContents += '\n';
                if (worldToCameraMatrixArray[i] != null)
                    fileContents += worldToCameraMatrixArray[i].ToString();
                else
                {
                    // Safeguard against bad input
                    Matrix4x4 tempMat = new Matrix4x4();
                    fileContents += tempMat.ToString();
                }
            }
            // Write ProjectionMatrixArray
            fileContents += MatrixSeparator;
            fileContents += '\n';
            fileContents += ProjectionMatrixID;
            fileContents += '\n';
            for (int i = 0; i < numPhotosTaken; i++)
            {
                fileContents += MatrixSeparator;
                fileContents += '\n';
                if(projectionMatrixArray[i] != null)
                    fileContents += projectionMatrixArray[i].ToString();
                //else
                //{
                //    Matrix4x4 tempMat = new Matrix4x4();
                //    fileContents += tempMat.ToString();
                //}
            }
            // Write LocalToWorldMatrixArray
            fileContents += MatrixSeparator;
            fileContents += '\n';
            fileContents += LocalToWorldMatrixID;
            fileContents += '\n';
            for(int i = 0; i < localToWorldMatrixArray.Length; i++)
            { 
                fileContents += MatrixSeparator;
                fileContents += '\n';
                Matrix4x4 ltw = localToWorldMatrixArray[i];

                if (ltw != null)
                    fileContents += ltw.ToString();
            }

            // Actually write text file
            File.WriteAllText(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName), fileContents);
        }

        public static bool LoadMatrixArrays_AssetsStored(out Matrix4x4[] WorldToCameraMatrixArray, out Matrix4x4[] ProjectionMatrixArray, out Matrix4x4[] LocalToWorldMatrixArray)
        {
            string roomName = Config.RoomObject.GameObjectName;

            string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
            Debug.Log("Matrix array filepath = " + matrixArrayFilepath);
            //if (!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(),roomName)))
            if(!File.Exists(matrixArrayFilepath))
            {
                WorldToCameraMatrixArray = null;
                ProjectionMatrixArray = null;
                LocalToWorldMatrixArray = null;
                return false;
            }

            Debug.Log("Matrix array file found");

            string[] fileLines = File.ReadAllLines(matrixArrayFilepath);

            Debug.Log(fileLines[0]);
            Debug.Log(fileLines[1]);
            Debug.Log(fileLines[2]);
            Debug.Log(fileLines[3]);
            Debug.Log(fileLines[4]);

            DeriveMatrixArrays(fileLines, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            return true;
        }

        /// <summary>
        /// Load matrix arrays from a TextAsset.
        /// </summary>
        /// <param name="fromAsset">
        /// Simple bool to determine whether the TextAsset should be read from 
        /// or not.
        /// </param>
        /// <param name="WorldToCameraMatrixArray">
        /// Matrix array where matrices translate world space coordinates to 
        /// camera/view space. Each matrix is associated with a different 
        /// camera/texture.
        /// </param>
        /// <param name="ProjectionMatrixArray">
        /// Matrix array where matrices translate camera/view space coordinates 
        /// to clip space. Each matrix is associated with a different 
        /// camera/texture.
        /// </param>
        /// <param name="LocalToWorldMatrixArray">
        /// Matrix array where matrices translate local/model space coordinates 
        /// to world space. Each matrix is associated with a different MESH 
        /// (not camera/texture).
        /// </param>
        /// <param name="asset">
        /// The TextAsset to derive the matrix arrays from.
        /// </param>
        public static void LoadMatrixArrays_FromAssetBundle(TextAsset asset, out Matrix4x4[] WorldToCameraMatrixArray, out Matrix4x4[] ProjectionMatrixArray, out Matrix4x4[] LocalToWorldMatrixArray)
        {
            DeriveMatrixArrays(SplitTextAsset(asset), out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            //if (fromAsset)
            //{
            //    if(asset != null)
            //    {
            //        DeriveMatrixArrays(SplitTextAsset(asset), out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);
            //    }
            //    else
            //    {
            //        WorldToCameraMatrixArray = null;
            //        ProjectionMatrixArray = null;
            //        LocalToWorldMatrixArray = null; 
            //    }
            //}
            //else
            //{
            //    // ERROR TESTING - Make it so that you can load directly from a file as well.
            //    WorldToCameraMatrixArray = null;
            //    ProjectionMatrixArray = null;
            //    LocalToWorldMatrixArray = null;
            //}
        }

        /// <summary>
        /// Splits a TextAsset by the newline character, returning an array of 
        /// the lines stored in the TextAsset.
        /// </summary>
        /// <param name="textAsset">
        /// The TextAsset to be split.
        /// </param>
        /// <returns>
        /// An array of strings representing an array of the lines of the 
        /// TextAsset passed in.
        /// </returns>
        private static string[] SplitTextAsset(TextAsset textAsset)
        {
            return textAsset.text.Split('\n');
        }

        /// <summary>
        /// Handles the actual logic of extracting the matrix arrays from text 
        /// file lines.
        /// </summary>
        /// <param name="fileLines">
        /// The array of strings representing the lines of the text file used 
        /// to store the matrix arrays.
        /// </param>
        /// <param name="WorldToCameraMatrixArray">
        /// Matrix array where matrices translate world space coordinates to 
        /// camera/view space. Each matrix is associated with a different 
        /// camera/texture.
        /// </param>
        /// <param name="ProjectionMatrixArray">
        /// Matrix array where matrices translate camera/view space coordinates 
        /// to clip space. Each matrix is associated with a different 
        /// camera/texture.
        /// </param>
        /// <param name="LocalToWorldMatrixArray">
        /// Matrix array where matrices translate local/model space coordinates 
        /// to world space. Each matrix is associated with a different MESH 
        /// (not camera/texture).
        /// </param>
        public static void DeriveMatrixArrays(string[] fileLines, out Matrix4x4[] WorldToCameraMatrixArray, out Matrix4x4[] ProjectionMatrixArray, out Matrix4x4[] LocalToWorldMatrixArray)
        {
            // Create lists to store the matrices extracted so the resulting 
            // matrix arrays are of the correct size.
            List<Matrix4x4> worldToCamList = new List<Matrix4x4>();
            List<Matrix4x4> projList = new List<Matrix4x4>();
            List<Matrix4x4> localToWorldList = new List<Matrix4x4>();

            // Use booleans to determine what kind of item a line represents
            bool useWorldToCamList = false;
            bool useProjList = false;
            bool useLocalToWorldList = false;

            //Debug.Log("Matrix array file found. # of fileLines = " + fileLines.Length);

            int lineCount = 0;
            while (lineCount < fileLines.Length)
            {
                //Debug.Log("Line = " + fileLines[lineCount]);

                fileLines[lineCount] = fileLines[lineCount].TrimEnd();

                // ID a matrix separator and look at the next line(s) to 
                // determine what is written
                if (fileLines[lineCount].Contains(MatrixSeparator))
                {

                    //Debug.Log("Matrix separator found on line " + lineCount);

                    ++lineCount;

                    if (fileLines[lineCount].Contains(WorldToCameraMatrixID))
                    {
                        // WorldToCameraMatrix found
                        ++lineCount;
                        useWorldToCamList = true;
                        useProjList = false;
                        useLocalToWorldList = false;

                        //Debug.Log("WorldToCam matrix found");
                    }
                    else if (fileLines[lineCount].Contains(ProjectionMatrixID))
                    {
                        // ProjectionMatrix found
                        ++lineCount;
                        useProjList = true;
                        useWorldToCamList = false;
                        useLocalToWorldList = false;

                        //Debug.Log("Projection matrix found");
                    }
                    else if (fileLines[lineCount].Contains(LocalToWorldMatrixID))
                    {
                        // LocalToWorldMatrix found
                        ++lineCount;
                        useLocalToWorldList = true;
                        useWorldToCamList = false;
                        useProjList = false;

                        //Debug.Log("LocalToWorld matrix found");
                    }
                    else
                    {
                        //Debug.Log("Assuming matrix line. Line = " + fileLines[lineCount]);

                        // Extract the actual 4x4 matrix
                        Matrix4x4 mat = new Matrix4x4();
                        for (int i = 0; i < 4; i++)
                        {
                            string[] lineContents = fileLines[lineCount].Split('\t');

                            for (int j = 0; j < lineContents.Length; j++)
                            {
                                lineContents[j] = lineContents[j].TrimEnd();
                                float num;
                                if (!float.TryParse(lineContents[j], out num))
                                {
                                    num = 0;
                                }

                                mat[i, j] = num;
                            }
                            ++lineCount;
                        }

                        //Debug.Log("Matrix read. fileLine = " + lineCount);

                        // Tack the matrix onto the appropriate list
                        if (useWorldToCamList)
                        {
                            worldToCamList.Add(mat);
                        }
                        else if (useProjList)
                        {
                            projList.Add(mat);
                        }
                        else if (useLocalToWorldList)
                        {
                            localToWorldList.Add(mat);
                        }
                    }
                }
                else
                {
                    ++lineCount;
                }
            }

            // Convert all matrix lists to matrix arrays
            WorldToCameraMatrixArray = worldToCamList.ToArray();
            ProjectionMatrixArray = projList.ToArray();
            LocalToWorldMatrixArray = localToWorldList.ToArray();

            //Debug.Log("Immediately after processing, WorldToCam length = " + WorldToCameraMatrixArray.Length);
            //Debug.Log("Immediately after processing, WorldToCam list length = " + WorldToCameraMatrixArray.Length);
        }

        #endregion
    }
}