using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using UnityEngine.Windows.Speech; // Voice Commands
#endif

namespace UWB_Texturing
{
    public enum VoiceCommands
    {
        Start,
        Photo,
        End
    };

    public static class VoiceManager // ERROR TESTING - IS THIS NEEDED? // : MonoBehaviour
    {
#if WINDOWS_UWP
//#if UNITY_WSA_10_0
        public static KeywordRecognizer keywordRecognizer;
        public static Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        public static void Start()
        {
            keywordRecognizer = null;
        }

        public static void EnableVoiceCommand(VoiceCommands command)
        {
            bool enabled = false;

            if (command == VoiceCommands.Start)
            {
                // Command: Begin texture capture protocol 
                // (i.e. start up photocapture process)
                keywords.Add(VoiceKeywords.Word_Start, () =>
                {
                    // ERROR TESTING - 
                    CameraManager.StartTextureCapture();
                });

                enabled = true;
            }
            else if (command == VoiceCommands.Photo)
            {
                // Command: Take texture screenshot
                keywords.Add(VoiceKeywords.Word_Photo, () =>
                {
                    // ERROR TESTING - 
                    CameraManager.CaptureTexture();
                });

                enabled = true;
            }
            else if (command == VoiceCommands.End)
            {
                // Command: End texture capture protocol and clean up
                keywords.Add(VoiceKeywords.Word_End, () =>
                {
                    // ERROR TESTING - 
                    CameraManager.StopTextureCapture();
                });

                enabled = true;
            }

            if (enabled)
            {
                RefreshKeywordRecognizer();
            }
        }

        /*
        public static void EnableVoiceCommand(IEnumerable<Constants.Commands.VoiceCommandsEnum> voiceCommands)
        {
            foreach (var command in voiceCommands)
            {
                EnableVoiceCommand(command);
            }
        }
        */

        public static void DisableVoiceCommand(VoiceCommands command)
        {
            bool disabled = false;

            if (command == VoiceCommands.Start)
            {
                keywords.Remove(VoiceKeywords.Word_Start);
                disabled = true;
            }
            else if (command == VoiceCommands.Photo)
            {
                keywords.Remove(VoiceKeywords.Word_Photo);
                disabled = true;
            }
            else if (command == VoiceCommands.End)
            {
                keywords.Remove(VoiceKeywords.Word_End);
                disabled = true;
            }

            if (disabled)
                RefreshKeywordRecognizer();

            //if (keywords.Count == 0)
            //    keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
        }
        
        public static void RefreshKeywordRecognizer()
        {
            // Erase the old keyword recognizer
            if (keywordRecognizer != null)
            {
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Stop();
                keywordRecognizer.Dispose();
                keywordRecognizer = null;
            }

            // Reboot keyword recognizer with refreshed keywords
            string[] keywordArray = new string[keywords.Keys.Count];
            keywords.Keys.CopyTo(keywordArray, 0);
            keywordRecognizer = new KeywordRecognizer(keywordArray);
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();
        }

        // Handle logic for directing phrase recognition calls to the
        // appropriate method.
        private static void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action action;

            if (keywords.TryGetValue(args.text, out action))
                action.Invoke();
            else
                Debug.Log("Voice command not recognized.");
                // throw new System.Exception("Voice command not recognized.");
                // ERROR TESTING MUST CATCH THIS ERROR APPROPRIATELY
        }
        
        // ERROR TESTING REMOVE
        /*
        void Start()
        {
            initialize();
        }

        // Handles the logic for initializing the keywords that will
        // be added to the Hololens databank. To add more commands, see
        // "initializeCommands" method.
        private void initialize()
        {
            string[] commands = initializeCommands();
            keywordRecognizer = new KeywordRecognizer(commands);
            enableKeywords(commands);
            keywordRecognizer.Start();
        }

        // Initialize commands to be added. This is where additional commands
        // must be inserted.
        private string[] initializeCommands()
        {
            string[] keywordArray = null;

            // Command: Begin texture capture protocol 
            // (i.e. start up photocapture process)
            keywords.Add(RoomTexture.Keyword_BeginRoomTexturing, () =>
            {
                TextureCapture.BeginCapture();
            });

            // Command: End texture capture protocol and clean up
            keywords.Add(RoomTexture.Keyword_EndRoomTexturing, () =>
            {
                TextureCapture.EndCapture();
            });

            // Generate array of all commands
            keywordArray = new string[keywords.Keys.Count];
            int keywordIndex = 0;
            foreach (string key in keywords.Keys)
            {
                keywordArray[keywordIndex++] = key;
            }

            return keywordArray;
        }
        
        public void enableKeywords(string[] keywordArray)
        {
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        }

        public void disableKeywords(string[] keywordArray)
        {
            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
        }

        public void enableVoiceTrigger(string keyword)
        {
            keywordRecognizer.OnPhraseRecognized += 
        }

        public void disableVoiceTrigger(string keyword)
        {

        }
        */
#endif
    }
}