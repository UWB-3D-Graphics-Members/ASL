using UnityEngine;
using System.Collections;

namespace UWB_Texturing
{
    /// <summary>
    /// Displays text to a transparent panel hovering about a meter in front 
    /// of the Hololens camera. Allows for text to be displayed to the user 
    /// when using the room texturing application. Cannot be attached to a
    /// Unity class, so it must be manually initialized.
    /// </summary>
    public static class TextManager
    {
        public static UnityEngine.UI.Text HUD_Text;
        private static bool isActive = false;
        
        /// <summary>
        /// Publicly accessible hook to start up the TextManager and set 
        /// up its components appropriately. Must be manually called, as
        /// the TextManager class cannot be attached to a Unity object.
        /// </summary>
        public static void Start()
        {
            GameObject panel = GameObject.Find("TextPanel");

            if ((HUD_Text = panel.GetComponent("Text") as UnityEngine.UI.Text) == null)
            {
                HUD_Text = panel.AddComponent<UnityEngine.UI.Text>();
            }
            
            HUD_Text.alignment = TextAnchor.LowerCenter;
            HUD_Text.fontSize = 14;
            HUD_Text.color = Color.red;

            isActive = true;
        }

        /// <summary>
        /// Public hook for changing the text displayed by the TextPanel pointed
        /// to by the TextManager class.
        /// </summary>
        /// <param name="input"></param>
        public static void SetText(string input)
        {
            HUD_Text.text = input;
        }

        public static bool IsActive
        {
            get
            {
                return isActive;
            }
        }
    }
}