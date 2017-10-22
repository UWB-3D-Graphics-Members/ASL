using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK; // RadialMenu

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASL.Manipulation.Controllers.Vive
{

    /// <summary>
    /// NOTE: THIS CLASS MUST BE UPDATED TO SEPARATE MANAGEMENT LOGIC FROM THE CUSTOM
    /// RADIAL MENU LOGIC. THE RADIAL MENU LOGIC MUST BE PLACED WITHIN UI/MENUS/VIVE/CUSTOMRADIALMENU.cs.
    /// </summary>
    public class ControllerUIManager : MonoBehaviour
    {
#if !UNITY_ANDROID
        Transform controllerTip;
        VRTK_RadialMenu radialMenu;
        public Object[] buttons;

        // Use this for initialization
        void Awake()
        {
            if (radialMenu == null)
            {
                radialMenu = transform.parent.Find("RadialMenu/RadialMenuUI/Panel").GetComponent<VRTK_RadialMenu>();
            }

            // Look for controller tip on awake
            if (controllerTip == null)
            {
                controllerTip = transform.parent.transform.Find("Model/tip/attach");
            }
        }

        void Start()
        {
            if (radialMenu == null)
            {
                radialMenu = transform.parent.Find("RadialMenu/RadialMenuUI/Panel").GetComponent<VRTK_RadialMenu>();

                if (radialMenu == null)
                {
                    Debug.LogError("Could not find radial menu in parent");
                }
            }

            // Look for controller tip on start
            if (controllerTip == null)
            {
                controllerTip = transform.parent.transform.Find("Model/tip/attach");
            }

            if (radialMenu != null)
            {
                // ERROR TESTING - REINCORPORATE
                //for (int i = 0; i < buttons.Length; i++)
                //{
                //    Texture2D newTexture = AssetPreview.GetAssetPreview(buttons[i]);
                //    Debug.Log("Create button preview for " + buttons[i].name);
                //    if (newTexture == null)
                //    {
                //        Debug.LogError("Null Texture created for RadialMenu button");
                //    }
                //    else
                //    {
                //        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
                //        radialMenu.buttons[i].ButtonIcon = newSprite;
                //        Object objClone = buttons[i];
                //        radialMenu.buttons[i].OnClick.AddListener(delegate { spawnObj(objClone); });
                //    }
                //}
            }
        }

        void Update()
        {
            // Continue to look for controller tip
            if (controllerTip == null)
            {
                controllerTip = transform.parent.transform.Find("Model/tip/attach");

                if (controllerTip != null)
                {
                    Debug.Log("Controller tip found in parent");
                }
            }
        }

        public void spawnObj(Object obj)
        {
            //GameObject g = GameObject.Instantiate((GameObject)obj);
            PhotonNetwork.Instantiate(obj.name, controllerTip.position, controllerTip.rotation, 0);
        }
#endif
    }
}