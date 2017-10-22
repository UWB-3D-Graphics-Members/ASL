using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//Imports to interact with Camera
#if WINDOWS_UWP
using System.Runtime.InteropServices;
using Windows.Media.Devices;
#endif

#if WINDOWS_UWP
//#if UNITY_WSA_10_0
using Windows.Storage;
using System.Threading.Tasks;
using System;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.VR.WSA.Input;
#endif

namespace UWB_Texturing {
    /// <summary>
    /// Takes photos though the built-in Hololens cameras and stores matrices 
    /// identifying locations of textures. Saves information for retrieval. 
    /// Assumes inclusion of SpatialMapping prefab in scene object hierarchy.
    /// </summary>
    public static class CameraManager {
//#if WINDOWS_UWP
#if UNITY_WSA_10_0
        /// <summary>
        /// Output messages stored to be used in debug logs, textual feedback, or exceptions.
        /// </summary>
        public static class Messages
        {
            /// <summary>
            /// Hololens: Tell user how to initialize overall texture-capturing process.
            /// </summary>
            public static string StartPrompt = "Say \"" + VoiceKeywords.Word_Start + "\" to begin texturing process.";
            /// <summary>
            /// Hololens: Tell user that the previous hololens room texture files were cleared.
            /// </summary>
            public static string FilesCleared = "Previous room texture files cleared. All .txt and .png files deleted.";
            /// <summary>
            /// Hololens: Tell user how to take photos when photo-taking is available.
            /// </summary>
            public static string PhotoPrompt = "Say \"" + VoiceKeywords.Word_Photo + "\" to take photo, or \"" + VoiceKeywords.Word_End + "\" to stop.";
            /// <summary>
            /// Hololens: Tell user that they're taking photos too quickly.
            /// </summary>
            public static string PhotoAlreadyProcessing = "Already processing texture...";
            /// <summary>
            /// Hololens: Tell user that the Hololens is already ending the texturing process.
            /// </summary>
            public static string PhotoCaptureAlreadyEnding = "Already ending texturing process.";
            /// <summary>
            /// Hololens: Tell user that the photo is being taken.
            /// </summary>
            public static string PhotoProcessStart = "Taking picture...";
            /// <summary>
            /// Hololens: Tell user that matrix capture for a picture failed.
            /// </summary>
            public static string MatrixFail = "Matrix capture failed. Texture not saved. " + PhotoPrompt;
            /// <summary>
            /// Hololens: Tell user that cleanup process for texture capture 
            /// has started, and that photo-taking is no longer available.
            /// </summary>
            public static string EndingPhotoCapture = "Stopping texture capture...";
            /// <summary>
            /// Hololens: Tell user that texture capture has completely ended. 
            /// Use after cleanup processes are already finished.
            /// </summary>
            public static string PhotoCaptureEnded = "Texture capture ended. Items saved in Hololens for manual transfer.";
            /// <summary>
            /// Hololens: Tell user that texture capture has ended due to maximum number of texture photos taken.
            /// </summary>
            public static string MaxPhotosTaken = "Maximum # of textures reached." + PhotoCaptureEnded;

            public static string AppendNumPhotosTaken()
            {
                return " " + currentPhoto + "/" + MaxPhotoNum;
            }
        }

        #region Fields

        // General
        public static int MaxPhotoNum = 30; // Max # of texture pictures to take
        public static string FolderPath = Application.persistentDataPath;

        // Data storage
        public static Matrix4x4[] WorldToCameraMatrixArray;
        public static Matrix4x4[] ProjectionMatrixArray;
        public static Matrix4x4[] LocalToWorldMatrixArray;
        public static Vector3[] PositionArray;
        public static Quaternion[] RotationArray;
        public static Mesh[] MeshArray;
        
        // Camera Settings
        private static CameraParameters m_CameraParameters;
        public static bool lockCameraSettings = true; // Camera exposure and white balance
        private static Resolution m_CameraResolution;
        private static Resolution m_downgradedCameraResolution;
        private static Texture2D m_Texture;
        
        // Photo Capture Fields
        private static bool isCapturingPhoto = false;
        private static bool isEndingPhotoCapture = false;
        private static PhotoCapture photoCaptureObj;
        private static int currentPhoto = 0;

        #endregion

        #region Methods

        #region Main Methods
        /// <summary>
        /// Initialization method. Must be called manually since this class 
        /// isn't attachable to a Unity object to make it auto-initiate. 
        /// Initializes variables, arrays, the camera, and enables the voice 
        /// command that triggers letting the camera take pictures for 
        /// texturing. Sets display text.
        /// </summary>
        public static void Start() {
            // Initialize variables
            WorldToCameraMatrixArray = new Matrix4x4[MaxPhotoNum];
            ProjectionMatrixArray = new Matrix4x4[MaxPhotoNum];
            // Avoid potential null exceptions w/ future code changes
            // by using temporary new matrix arrays
            LocalToWorldMatrixArray = new Matrix4x4[1];
            PositionArray = new Vector3[1];
            RotationArray = new Quaternion[1];
            MeshArray = new Mesh[1];

            // Failsafe for Hololens camera and texture initialization
            // Default assumed of 1280 pixels x 720 pixels
            m_downgradedCameraResolution = new Resolution();
            m_CameraParameters = new CameraParameters();
            m_CameraResolution = new Resolution();
            m_CameraResolution.width = 1280;
            m_CameraResolution.height = 720;
            m_Texture = new Texture2D(1280, 720);

            // Run accurate Hololens camera and texture initialization
            InitCamera();
            m_Texture = new Texture2D(CameraResolution.width, CameraResolution.height);
            
            // Enable voice command that triggers texturing process
            VoiceManager.EnableVoiceCommand(VoiceCommands.Start);
            TextManager.SetText(Messages.StartPrompt);
        }
    
        /// <summary>
        /// Publicly available hook for externally/internally triggering texture 
        /// capture process.
        /// </summary>
        public static void StartTextureCapture()
        {
            // Boot up camera
            PhotoCapture.CreateAsync(false, OnCreatedPhotoCaptureObject);
        }
        
        /// <summary>
        /// Preps the Hololens webcam for taking pictures, and calls 
        /// OnStartPhotoMode when a webcam object is created and primed
        /// to continue the texture capture process.
        /// </summary>
        /// <param name="captureObject">
        /// The PhotoCapture object that handles the instantiation of the Hololens camera.
        /// </param>
        static void OnCreatedPhotoCaptureObject(PhotoCapture captureObject)
        {
            photoCaptureObj = captureObject;
            ClearRoomTextureFiles();
            photoCaptureObj.StartPhotoModeAsync(m_CameraParameters, OnStartPhotoMode);
        }

        /// <summary>
        /// Called when the Hololens webcam is created and primed. Triggers
        /// the enabling of voice commands for taking photos or ending
        /// camera mode. Removes ability to erroneously start up texturing 
        /// process while the texturing process is underway. Sets the displayed 
        /// text.
        /// </summary>
        /// <param name="result">
        /// The information/image saved from the PhotoCapture's photo capture.
        /// </param>
        static void OnStartPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            VoiceManager.EnableVoiceCommand(VoiceCommands.Photo);
            VoiceManager.EnableVoiceCommand(VoiceCommands.End);
            VoiceManager.DisableVoiceCommand(VoiceCommands.Start);
            TextManager.SetText(Messages.PhotoPrompt + Messages.AppendNumPhotosTaken());
        }
        
        /// <summary>
        /// Publicly available hook for externally/internally triggering 
        /// texture capture process. Accompanying orientation data will 
        /// be stored, but not saved until the Hololens texturing 
        /// process is manually ended.
        /// </summary>
        public static void CaptureTexture()
        {
            if (isCapturingPhoto)
            {
                TextManager.SetText(Messages.PhotoAlreadyProcessing);
            }
            else {
                isCapturingPhoto = true;
                TextManager.SetText(Messages.PhotoProcessStart);
                photoCaptureObj.TakePhotoAsync(OnPhotoCaptured);
            }
        }

        /// <summary>
        /// Grab worldToCamera matrix and projection matrix from the Hololens 
        /// camera for correct texture placement, clip and save the texture as 
        /// an image, then properly clean up (dispose of) the PhotoCaptureFrame 
        /// storing relevant image data.
        /// </summary>
        /// <param name="result">
        /// Information about the success of taking the picture.
        /// </param>
        /// <param name="photoCaptureFrame">
        /// The information/image associated with the picture taken.
        /// </param>
        static void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            // After the first photo, we want to lock in the current exposure and white balance settings.
            if (lockCameraSettings && currentPhoto == 0)
            {
                //#if WINDOWS_UWP
#if WINDOWS_UWP
        //#if UNITY_WSA_10_0
                unsafe{
            //This is how you access a COM object.
            VideoDeviceController vdm = (VideoDeviceController)Marshal.GetObjectForIUnknown(photoCaptureObj.GetUnsafePointerToVideoDeviceController());
            //Locks current exposure for all future images
            vdm.ExposureControl.SetAutoAsync(false); //Figureout how to better handle the Async
            //Locks the current WhiteBalance for all future images
            vdm.WhiteBalanceControl.SetPresetAsync(ColorTemperaturePreset.Fluorescent);
            
            //vdm.WhiteBalanceControl.SetPresetAsync(ColorTemperaturePreset.Manual); 
        }
#endif
            }

            // Grab appropriate matrices, and write them to the local storage 
            // arrays at the correct index
            Matrix4x4 worldToCameraMatrix;
            Matrix4x4 projectionMatrix;
            bool matricesExtracted = ExtractCameraMatrices(photoCaptureFrame, out worldToCameraMatrix, out projectionMatrix);
            if (matricesExtracted)
            {
                WriteMatricesToArrays(worldToCameraMatrix, projectionMatrix, currentPhoto);

                // Set up local class texture to save as a picture/texture - Hololens camera requires BGRA32 format
                m_Texture = new Texture2D(m_CameraParameters.cameraResolutionWidth, m_CameraParameters.cameraResolutionHeight, TextureFormat.BGRA32, false);
                photoCaptureFrame.UploadImageDataToTexture(m_Texture);
                m_Texture = ClipTexture(m_Texture);
                m_Texture.wrapMode = TextureWrapMode.Clamp;
                SaveTexture();
                
                // Reset displayed message to remind user how to take photos or end texturing process
                TextManager.SetText(Messages.PhotoPrompt + Messages.AppendNumPhotosTaken());
            }
            else
            {
                TextManager.SetText(Messages.MatrixFail);
            }
            // Clean up camera memory
            photoCaptureFrame.Dispose();

            // Automatically shut down the operation if the maximum number of 
            // textures is reached.
            if(currentPhoto >= MaxPhotoNum)
            {
                StopTextureCapture();
            }
        }
        
        /// <summary>
        /// Publicly available method to end the Hololens texturing process. 
        /// Sets the displayed text.
        /// </summary>
        public static void StopTextureCapture()
        {
            if (isEndingPhotoCapture)
            {
                TextManager.SetText(Messages.PhotoCaptureAlreadyEnding);
            }
            else
            {
                isEndingPhotoCapture = true;
                TextManager.SetText(Messages.EndingPhotoCapture);
                photoCaptureObj.StopPhotoModeAsync(OnStoppedPhotoMode);
            }
        }

        /// <summary>
        /// Handles the logic of stopping the active webcam mode for the Hololens. 
        /// Disposes and nulls out the webcam object and disables voice commands. 
        /// Re-enables the start voice command to restart the process of taking 
        /// room textures if the user desires.
        /// </summary>
        /// <param name="result">
        /// The success of stopping the OnStoppedPhotoMode.
        /// </param>
        static void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            // Disables the ability to take pictures, and cleans up/disposes of
            // the photo capture object.
            VoiceManager.DisableVoiceCommand(VoiceCommands.Photo);
            photoCaptureObj.Dispose();
            photoCaptureObj = null;
            
            // Sets locally stored matrices and supplementary info required for
            // correct texture placement, then saves them along with the mesh
            SetLocalToWorldMatrices(); // Must be called before SaveMatrixArrays
            SetMeshSupplementaryInfo();
            string roomName = Config.RoomObject.GameObjectName;
            MatrixArray.SaveMatrixArrays(WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray, currentPhoto);
            CustomMesh.SaveMesh(MeshArray);
            CustomOrientation.Save(PositionArray, RotationArray);
            // ERROR TESTING - REMOVE // CustomMesh.SaveSupplementaryInfo(PositionArray, RotationArray);

            // Disable the ability to end the photo capture process
            VoiceManager.DisableVoiceCommand(VoiceCommands.End);
            VoiceManager.EnableVoiceCommand(VoiceCommands.Start);
            isEndingPhotoCapture = false;


            TextManager.SetText("Finished!");

            if (currentPhoto < MaxPhotoNum)
            {
                TextManager.SetText(Messages.PhotoCaptureEnded);
            }
            else
            {
                TextManager.SetText(Messages.MaxPhotosTaken);
            }
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Get a Resolution that has a width and height that are the highest 
        /// power of 2 equal to or less than the passed in Resolution's width 
        /// and height. Useful for getting Resolution that works with Texture2DArray, 
        /// which accepts resolutions ONLY if they have a width and height that 
        /// is a power of 2.
        /// </summary>
        /// <param name="textureResolution"></param>
        /// <returns></returns>
        private static Resolution DowngradeTextureResolution(Resolution textureResolution)
        {
            Resolution res = new Resolution();

            int width = 1;
            int height = 1;

            while(width * 2 <= textureResolution.width)
            {
                width *= 2;
            }
            while (height * 2 <= textureResolution.height)
            {
                height *= 2;
            }

            res.width = width;
            res.height = height;
            res.refreshRate = textureResolution.refreshRate;

            Debug.Log("Downgraded tex in method: width = " + width + "; height = " + height);

            return res;
        }

        /// <summary>
        /// Returns true if the width and height of the resolution are a power 
        /// of 2.
        /// </summary>
        /// <param name="textureResolution"></param>
        /// <returns></returns>
        private static bool CheckIfTexture2DArrayCompatible(Resolution textureResolution)
        {
            Resolution correctRes = DowngradeTextureResolution(textureResolution);

            if (correctRes.width == textureResolution.width
                && correctRes.height == textureResolution.height)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Generate a raw Texture2DArray of size MaxPhotoNum, 
        /// using the UtilizedResolution dimensions.
        /// </summary>
        /// <returns></returns>
        private static Texture2DArray CreateTexture2DArray()
        {
            return new Texture2DArray(UtilizedResolution.width, UtilizedResolution.height, MaxPhotoNum, TextureFormat.DXT5, false);
        }

        /// <summary>
        /// Publicly accessible helper function designed to clear all room texture 
        /// files from the Hololens folder to help with a fresh start for the 
        /// texturing process. Sets displayed text.
        /// 
        /// NOTE: This will delete ALL files with the ".txt" and ".png" file 
        /// extensions.
        /// </summary>
        public static void ClearRoomTextureFiles()
        {
            Queue<string> filesToDelete = new Queue<string>();

            string[] files = Directory.GetFiles(FolderPath);
            string[] extensionsToMark = new string[2];
            extensionsToMark[0] = ".png";
            extensionsToMark[1] = ".txt";

            // Mark the files to be deleted
            foreach (string file in files)
            {
                foreach (string extension in extensionsToMark)
                {
                    if (Path.GetExtension(file).Equals(extension))
                    {
                        filesToDelete.Enqueue(file);
                    }
                }
            }

            // Delete the files
            while (filesToDelete.Count > 0)
            {
                string fileToDelete = filesToDelete.Dequeue();
                Directory.Delete(fileToDelete);
            }

            // Tell user that texturing files were removed
            TextManager.SetText(Messages.FilesCleared);
        }

        /// <summary>
        /// Run logic for initializing Hololens web camera for taking pictures 
        /// for texturing the room mesh.
        /// </summary>
        private static void InitCamera()
        {
#if UNITY_WSA_10_0
            // Set camera resolution (documented default of 1280 x 720)
            List<Resolution> resolutions = new List<Resolution>(PhotoCapture.SupportedResolutions);
            CameraResolution = resolutions[0];

            // Set other camera parameters
            m_CameraParameters = new CameraParameters(WebCamMode.PhotoMode);
#endif
            m_CameraParameters.cameraResolutionWidth = CameraResolution.width;
            m_CameraParameters.cameraResolutionHeight = CameraResolution.height;
            m_CameraParameters.hologramOpacity = 0.0f;
#if UNITY_WSA_10_0
            m_CameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
#endif

            // Set the Texture2DArray-compatible texture settings
            UtilizedResolution = DowngradeTextureResolution(CameraResolution);

            // Create texture array
            CreateTexture2DArray();
        }

        /// <summary>
        /// Extract the worldToCamera matrix (i.e. V of the MVP matrix 
        /// traditionally used to translate model coordinates to world 
        /// coordinates to camera coordinates), and the projection matrix 
        /// (i.e. P of the MVP matrix traditionally used to translate 
        /// camera/view coordinates to clip space).
        /// </summary>
        /// <param name="photoCaptureFrame">
        /// The PhotoCaptureFrame from which the matrices are to be derived 
        /// from.
        /// </param>
        /// <param name="worldToCameraMatrix">
        /// The matrix translating Hololens room mesh world coordinates to 
        /// view/camera space.
        /// </param>
        /// <param name="projectionMatrix">
        /// The matrix translating view/camera space coordinates to clip space.
        /// </param>
        private static bool ExtractCameraMatrices(PhotoCaptureFrame photoCaptureFrame, out Matrix4x4 worldToCameraMatrix, out Matrix4x4 projectionMatrix)
        {
            bool success = true;

            Matrix4x4 cameraToWorldMatrix;
            if (photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix))
            {
                worldToCameraMatrix = cameraToWorldMatrix.inverse;
            }
            else
            {
                TextManager.SetText(Messages.MatrixFail);
                worldToCameraMatrix = new Matrix4x4();
                success = false;
            }

            if (!photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix))
            {
                TextManager.SetText(Messages.MatrixFail);
                projectionMatrix = new Matrix4x4();
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Write the passed in matrices at the appropriate index in the 
        /// corresponding locally stored matrix arrays (WorldToCameraMatrixArray 
        /// & ProjectionMatrixArray).
        /// </summary>
        /// <param name="worldToCameraMatrix">
        /// The matrix translating Hololens room mesh world coordinates to 
        /// view/camera space.
        /// </param>
        /// <param name="projectionMatrix">
        /// The matrix translating view/camera space coordinates to clip space.
        /// </param>
        /// <param name="arrayIndex">
        /// The index of the locally stored matrix arrays that these matrices 
        /// must be written at.
        /// </param>
        private static void WriteMatricesToArrays(Matrix4x4 worldToCameraMatrix, Matrix4x4 projectionMatrix, int arrayIndex)
        {
            WorldToCameraMatrixArray[arrayIndex] = worldToCameraMatrix;
            ProjectionMatrixArray[arrayIndex] = projectionMatrix;
        }
        
        /// <summary>
        /// Make a copy of the passed-in texture that will be clipped down 
        /// to UtilizedResolution's dimensions. Contains no mipmapping and 
        /// returned image will be in RGBA32 format. RGBA32 format is one of 
        /// only a few formats accepted for manipulating or creating textures 
        /// in Unity. Resulting texture will focus on the center of the image, 
        /// and clip the outer edges. Original texture remains unchanged.
        /// </summary>
        /// <param name="tex">
        /// Texture to make a clipped copy of.
        /// </param>
        /// <returns>
        /// Clipped texture copy. Original unaffected by function call.
        /// </returns>
        public static Texture2D ClipTexture(Texture2D tex)
        {
            // Set up a blank texture to overwrite with the clipped texture. Unity requires format of RGBA32.
            Texture2D newTex = new Texture2D(UtilizedResolution.width, UtilizedResolution.height, TextureFormat.RGBA32, false);
            newTex.wrapMode = tex.wrapMode;
            newTex.name = tex.name;

            // Determine at which pixel coordinate to start copying from
            int startingX = (tex.width >= UtilizedResolution.width) ? (int)((tex.width - UtilizedResolution.width) / 2.0f) : 0;
            int startingY = (tex.height >= UtilizedResolution.height) ? (int)((tex.height - UtilizedResolution.height) / 2.0f) : 0;
            Color[] pix = tex.GetPixels(startingX, startingY, UtilizedResolution.width, UtilizedResolution.height);

            // Copy over designated pixels
            newTex.SetPixels(pix);
            newTex.Apply();
            return newTex;
        }

        /// <summary>
        /// Save the stored texture to the Hololens, and update the # used to 
        /// ID/track the texture. Also turns isCapturingPhoto to false.
        /// </summary>
        private static void SaveTexture()
        {
            byte[] texBytes = m_Texture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(FolderPath, Config.Images.CompileFilename(currentPhoto)), texBytes);
            currentPhoto += 1; // Increment photo counter (for naming/ID purposes)
            isCapturingPhoto = false;
        }

        /// <summary>
        /// Stores information of the mesh and related to the mesh, which 
        /// is necessary for correct translation/rotation of the room mesh 
        /// object on other devices. Information is stored in local arrays 
        /// (PositionArray, RotationArray, and MeshArray).
        /// 
        /// NOTE: Gathers information from the SpatialMapping object in the 
        /// Unity scene object hierarchy. If SpatialMapping object is missing 
        /// or operates differently since creation of this function, it may 
        /// have unexpected behavior or fail.
        /// </summary>
        private static void SetMeshSupplementaryInfo()
        {
            Queue<Vector3> positionList = new Queue<Vector3>();
            Queue<Quaternion> rotationList = new Queue<Quaternion>();
            Queue<Mesh> meshList = new Queue<Mesh>();

            Transform[] transforms = GameObject.Find("SpatialMapping").GetComponentsInChildren<Transform>();

            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].GetComponent<MeshRenderer>() == null ||
                    transforms[i].gameObject.GetComponent<MeshFilter>() == null ||
                    transforms[i].gameObject.GetComponent<MeshFilter>().sharedMesh == null)
                {
                    continue;
                }

                meshList.Enqueue(transforms[i].gameObject.GetComponent<MeshFilter>().sharedMesh);
                positionList.Enqueue(transforms[i].gameObject.transform.position);
                rotationList.Enqueue(transforms[i].gameObject.transform.rotation);
            }

            PositionArray = positionList.ToArray();
            RotationArray = rotationList.ToArray();
            MeshArray = meshList.ToArray();
        }

        /// <summary>
        /// Stores information of submeshes' local vertex positions of the 
        /// mesh stored in the Hololens. These are used to coordinate with the 
        /// mesh's overall position and determine correct texture/vertex 
        /// placement. Stored locally in an array (LocalToWorldMatrixArray).
        /// 
        /// NOTE: Gathers information from the SpatialMapping object in the 
        /// Unity scene object hierarchy. If SpatialMapping object is missing 
        /// or operates differently since creation of this function, it may 
        /// have unexpected behavior or fail.
        /// </summary>
        private static void SetLocalToWorldMatrices()
        {
            Transform[] transforms = GameObject.Find("SpatialMapping").GetComponentsInChildren<Transform>();
            Queue<Matrix4x4> localToWorldQueue = new Queue<Matrix4x4>();

            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].GetComponent<MeshRenderer>() == null ||
                    transforms[i].gameObject.GetComponent<MeshFilter>() == null ||
                    transforms[i].gameObject.GetComponent<MeshFilter>().sharedMesh == null)
                {
                    continue;
                }

                localToWorldQueue.Enqueue(transforms[i].gameObject.GetComponent<MeshRenderer>().localToWorldMatrix);
            }

            LocalToWorldMatrixArray = localToWorldQueue.ToArray();
        }

        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Get the Hololens webcam's Resolution used.
        /// </summary>
        public static Resolution CameraResolution
        {
            get
            {
                return m_CameraResolution;
            }
            private set
            {
                bool resSupported = false;
                foreach(Resolution res in PhotoCapture.SupportedResolutions)
                {
                    if (res.Equals(value))
                    {
                        resSupported = true;
                        break;
                    }
                }

                if (resSupported)
                {
                    m_CameraResolution = value;
                    UtilizedResolution = DowngradeTextureResolution(m_CameraResolution);
                }
                else
                    throw new System.Exception("Resolution not supported for Hololens.");
            }
        }

        /// <summary>
        /// Get the highest Texture2DArray-compatible Resolution.
        /// </summary>
        public static Resolution UtilizedResolution
        {
            get
            {
                return m_downgradedCameraResolution;
            }
            private set
            {
                if (CheckIfTexture2DArrayCompatible(value))
                {
                    m_downgradedCameraResolution = value;
                }
                else
                {
                    throw new System.Exception("Resolution is not of a power of 2.");
                }
            }
        }
        #endregion
#endif
    }
}