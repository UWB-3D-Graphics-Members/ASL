using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tango;
using System;

public class ControlManager : MonoBehaviour
{
    //used to hold a reference to the selected object (USED ACROSS ALL CONTROL MODES)
    private GameObject selectedItem;

    //used to hold refercen to the selected objects original color
    private Color originalObjectColor;

    //transparent object material color
    public Material transparentObjectColor;
    //reference for original material for swapping
    Material originalMaterial;

    //Enums for camera types
    private const int ARCameraEnum = 0;
    private const int VRCameraEnum = 1;
    private bool isARMode = true;
    private bool isVRMode = false;

    //reference to the cameras
    public GameObject VRCamera;
    public GameObject ARCamera;

    //Enums for control type
    public const int threeDTouchControlMode = 0; // 3D Touch input mode
    public const int IVICenterMode = 1; //integrated view input (center control) mode (IVI)
    public const int HOMERSMode = 2; //HOMER-S Control mode
    public const int hybridMode = 3; //hybrid Control mode as described by Marzo
    public const int modifiedIVICenterMode = 4; // modified integrated view input method (MODIVI)

    //boolean flags for control types
    private bool isthreeDTouchControlMode = false;
    private bool isIVICenterControlMode = false;
    private bool isHOMERControlMode = false;
    private bool isHybridMode = false;
    private bool isModifiedIVIMode = false;

    //boolean flags for object manipulations
    private bool objectIsSelected = false;

    //Reference to display Text for displaying information when user looks over objects with IVI and MODIVI control methods
    public Text infoDisplay;

    //Reference to debug output text to know what selection mode is currently being selected
    public Text currentSelectionDisplay;

    //Reference to the gaze reticle for usage during IVI and MODIVI mode selection
    public GameObject centerGazeReticle;

    //line objects to display wehen objects are selected during certain control modes
    GameObject xAxisLine;
    GameObject yAxisLine;
    GameObject zAxisLine;
    bool isXAxisVisible = false;
    bool isYAxisVisible = false;
    bool isZAxisVisible = false;
    float defaultLineWidth = 0.003f;
    float currentLineWidth = 0.005f;
    float defaultLineLength = 5f;
    float currentLineLength = 1f;

    //used for multiple touch controls
    RaycastHit touchHit;
    Vector3 objCenter;
    Vector3 touchPosition;
    Vector3 offset;

    //timer variables to differentiate fast tap to long touch
    float touchTimeThreshold = 0.2f;
    float touchTime = 0;
    Touch initialTouch;

    //USED FOR 3DTOUCH CONTROLS------------------------------------------
    //3DTouchEnums
    int threeDTouchClearAllModes = -1;
    int threeDTouchTranslationMode = 0;
    int threeDTouchRotationMode = 1;

    bool isThreeDTouchTranslationMode = false;
    bool isThreeDTouchRotationMode = false;
    float horizontalAngle = 0;
    float verticleAngle = 0;
    bool isFacingLeft = false;
    bool isFacingRight = false;
    bool isFacingUp = false;
    bool isFacingBack = false;
    bool isFacingForward = false;
    float screenToWorldRatio = 25600f; //using the height of the phone resolution
    float swipeThreshhold = 10f;

    float pinchTurnRatio = Mathf.PI / 2;
    float minTurnAngle = 0;
    float pinchRatio = 1;
    float minPinchDistance = 0;
    float panRatio = 1;
    float minPanDistance = 0;

    float turnAngleDelta = 0;
    float turnAngle = 0;
    float pinchDistanceDelta = 0;
    float pinchDistance = 0;

    public GameObject threeDTouchControlPanel;
    bool isThreeDTouchPanelVisible = false;
    public Text threeDTouchModeDisplay;

    //used to keep in touch with the global cordinate system,
    //this object is never rotated so it keeps aligned with world cordinate system
    public GameObject worldCordinateSystem; 
    //END OF 3DTOUCH CONTROL VARIABLES---------------------------------------------------------

    //USED FOR HOMER-S CONTROLS----------------------------------------------------------------
    //ENUMS for HOMER-S Controls
    int homerSClearAllModes = -1;
    int homerSTranslationMode = 0;
    int homerSRotationMode = 1;

    bool isHomersTranslationMode = false;
    bool isHomersRotationMode = false;
    Vector3 lockPos; //used to lock position during rotation mode
    Quaternion lockRot; //used to lock rotation during translation mode
    //END OF HOMER-S CONTROL VARIABLES---------------------------------------------------------


    //USED FOR HYBRID CONTROLS-----------------------------------------------------------------
    bool recentlySelected = false;
    bool forceUnlock = false;
    bool translateOnly = false;
    GameObject zPlane;
    bool prevFrameWas2Touch = false;
    //END OF HYBRID CONTROL VARIABLES-----------------------------------------------------------

    //USED FOR MODIFIED IVI CONTROLS------------------------------------------------------------
    float minTimeToBeCountedAsHold = 0.18f; //1f represents one second
    float acumTime = 0; //timer to keep track of time for double taps
    bool isLongHold = false;
    bool firstTouchActivated = false;

    //default 1.5 meters, any object 1.5 meters or beyond will snap to front of user on double tap
    float maxDistFromUserInMeters = 1.5f; 
    float objectDistanceFromCamera = 1.25f; 
    //END OF MODIFIED IVI CONTROLS--------------------------------------------------------------

    //the touch screen space of the mobile device 
    //use this to avoid touch activatation when user accidentally touches edge of screen
    private Rect activeTouchSpace = new Rect(150, 150, Screen.width - 300, Screen.height - 300);

    // Use this for initialization
    void Start()
    {
        //Set the VR camera to off so that it doesn't conflict with the AR camera
        VRCamera.SetActive(false);

        //Clear the information display area
        infoDisplay.text = "";

        //create z-plane for hybrid controls
        zPlane = new GameObject();

        //launch the program with 3DTouch controls
        this.SwitchControlMode(threeDTouchControlMode);

        //set the 3DTouchControlMode panel to off until a user selects an object
        threeDTouchControlPanel.SetActive(false);
        threeDTouchModeDisplay.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //IF the controls are set to 3D touch control modes---------------------------------------------------------------------------
        if (isthreeDTouchControlMode)
        {
            //string holder for object that was touched
            string touchObject;

            //go through all finger touches on the screen
            for (int i = 0; i < Input.touchCount; i++) 
            {
                //make sure the user did not accidentally touch the edge of the screen by 
                //checking space that is deemed "touchable" on the screen
                if (activeTouchSpace.Contains(Input.GetTouch(i).position))
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began) //user's initial contacts the screen with finger
                    {
                        touchTime = Time.time; //used for differentiating a fast tap from a long tap
                        initialTouch = Input.GetTouch(i); //used to hold the start position of the touch
                    }

                    //update the finger position for calculations if the user paused finger movement in middle of swipe
                    if (Input.GetTouch(i).phase == TouchPhase.Stationary)
                    {
                        initialTouch = Input.GetTouch(i);
                    }

                    if (Input.GetTouch(i).phase == TouchPhase.Moved) //user finger is on screen, and it's moved
                    {
                        if (objectIsSelected) //if an object is already selected by the user to move
                        {
                            //keep track of world cordinate systems old spot and rotation to shift it back to after math calculations
                            Quaternion original = new Quaternion(worldCordinateSystem.transform.rotation.x, worldCordinateSystem.transform.rotation.y,
                                worldCordinateSystem.transform.rotation.z, worldCordinateSystem.transform.rotation.w);
                            Vector3 oldSpot = new Vector3(worldCordinateSystem.transform.position.x, worldCordinateSystem.transform.position.y, 
                                worldCordinateSystem.transform.position.z);

                            //shift world cordinate system to make proper math calculations
                            worldCordinateSystem.transform.position = selectedItem.transform.position;

                            float horzPosShift = (initialTouch.position.x - Input.GetTouch(0).position.x) / 200;
                            float vertPosShift = (initialTouch.position.y - Input.GetTouch(0).position.y) / 200;

                            selectedItem.transform.transform.parent = worldCordinateSystem.transform;

                            if (isThreeDTouchTranslationMode)
                            {
                                //recalculate the shift in translation mode
                                horzPosShift = ((initialTouch.position.x - Input.GetTouch(0).position.x) * 200) / (screenToWorldRatio * 250);
                                vertPosShift = ((initialTouch.position.y - Input.GetTouch(0).position.y) * 200) / (screenToWorldRatio * 250);

                                //these axes are drawn during late update and they are drawn accordingly based on the
                                //direction the user is looking
                                if (isXAxisVisible && isYAxisVisible) //x is the horz,  y is the vert
                                {
                                    if (isFacingBack) //user is facing the front of the object looking in the same direction as world.back
                                    {//swiping left makes the object move right in world cordinate
                                        selectedItem.transform.position = new Vector3(selectedItem.transform.position.x + horzPosShift,
                                            selectedItem.transform.position.y - vertPosShift, selectedItem.transform.position.z);
                                    }
                                    else //user facing the back of object looking in the same directio as world.forward
                                    {//swiping left makes the object move left in the world cordinate system
                                        selectedItem.transform.position = new Vector3(selectedItem.transform.position.x - horzPosShift,
                                            selectedItem.transform.position.y - vertPosShift, selectedItem.transform.position.z);
                                    }
                                }
                                else if (isZAxisVisible && isYAxisVisible) //z is the horz, y is the vert
                                {
                                    if (isFacingLeft) //user is looking at the object in the direction of world.left
                                    {
                                        selectedItem.transform.position = new Vector3(selectedItem.transform.position.x,
                                            selectedItem.transform.position.y - vertPosShift, selectedItem.transform.position.z - horzPosShift);
                                    }
                                    else //user is looking at the object in the direction of world.right
                                    {
                                        selectedItem.transform.position = new Vector3(selectedItem.transform.position.x,
                                            selectedItem.transform.position.y - vertPosShift, selectedItem.transform.position.z + horzPosShift);
                                    }
                                }
                                else if (isXAxisVisible && isZAxisVisible) //user is looking up at the object or looking down at the object
                                {
                                    //check for the actual direction of the camera
                                    float angleFromForward = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.forward);
                                    float angleFromRight = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.right);
                                    float angleFromLeft = Vector3.Angle(ARCamera.transform.forward, -worldCordinateSystem.transform.right);
                                    float angleFromBack = Vector3.Angle(ARCamera.transform.forward, -worldCordinateSystem.transform.forward);

                                    if (isFacingUp)
                                    {
                                        //check for facing mostly forward only
                                        if ((angleFromForward <= angleFromRight) && (angleFromForward <= angleFromLeft) && (angleFromForward <= angleFromBack))
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x - horzPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z + vertPosShift);
                                        }
                                        //mostly facing the right in world space
                                        else if ((angleFromRight <= angleFromForward) && (angleFromRight <= angleFromLeft) && (angleFromRight <= angleFromBack)) 
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x + vertPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z + horzPosShift);
                                        }
                                        //mostly facing the left in world space
                                        else if ((angleFromLeft <= angleFromForward) && (angleFromLeft <= angleFromRight) && (angleFromLeft <= angleFromBack))
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x - vertPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z - horzPosShift);
                                        }
                                        else //the back is the most smallest angle, which means user is mostly facing the back in world space
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x + horzPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z - vertPosShift);
                                        }
                                    }
                                    else //facing down
                                    {
                                        //check for facing mostly forward only
                                        if ((angleFromForward <= angleFromRight) && (angleFromForward <= angleFromLeft) && (angleFromForward <= angleFromBack))
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x - horzPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z - vertPosShift);
                                        }
                                        //mostly facing the right in world space
                                        else if ((angleFromRight <= angleFromForward) && (angleFromRight <= angleFromLeft) && (angleFromRight <= angleFromBack)) 
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x - vertPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z + horzPosShift);
                                        }
                                        //mostly facing the left in world space
                                        else if ((angleFromLeft <= angleFromForward) && (angleFromLeft <= angleFromRight) && (angleFromLeft <= angleFromBack))
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x + vertPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z - horzPosShift);
                                        }
                                        else //the back is the most smallest angle, which means user is mostly facing the back in wolrd space
                                        {
                                            selectedItem.transform.position = new Vector3(selectedItem.transform.position.x + horzPosShift,
                                                selectedItem.transform.position.y, selectedItem.transform.position.z + vertPosShift);
                                        }
                                    }
                                }
                                else
                                {
                                    UnityEngine.Debug.Log("No Possible combination of visible axes of rotation not found, axes not displayed properly");
                                }                                
                            }
                            else if (isThreeDTouchRotationMode)
                            {
                                if (isXAxisVisible && isYAxisVisible) //x is the horz,  y is the vert
                                {
                                    if (isFacingBack) //user is facing the front of the object looking in the same direction as world.back
                                    {//swiping left makes the object rotate right in world cordinate
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift)) //do the verticle rotation because verticle finger movement is larger
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, vertPosShift);
                                        }
                                        else //do the horizontal rotation because verticle finger movement is larger
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                        }
                                    }
                                    else //user facing the back of object looking in the same directio as world.forward
                                    {//swiping left makes the object rotate left in the world cordinate system 
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift)) //do the verticle rotation because verticle finger movement is larger
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -vertPosShift);
                                        }
                                        else //do the horizontal rotation because verticle finger movement is larger
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                        }
                                    }
                                }
                                else if (isZAxisVisible && isYAxisVisible) //z is the horz, y is the vert
                                {
                                    if (isFacingLeft)
                                    {
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -vertPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                        }
                                    }
                                    else
                                    {
                                        if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, vertPosShift);
                                        }
                                    }
                                }
                                else if (isXAxisVisible && isZAxisVisible) 
                                {
                                    //check for the actual direction of the camera
                                    float angleFromForward = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.forward);
                                    float angleFromRight = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.right);
                                    float angleFromLeft = Vector3.Angle(ARCamera.transform.forward, -worldCordinateSystem.transform.right);
                                    float angleFromBack = Vector3.Angle(ARCamera.transform.forward, -worldCordinateSystem.transform.forward);

                                    if (isFacingUp) //rotate on x axis is transfrom.right, rotate on y is transform.up, z axis is transform.forward
                                    {
                                        //check for facing mostly forward only
                                        if ((angleFromForward <= angleFromRight) && (angleFromForward <= angleFromLeft) && (angleFromForward <= angleFromBack))
                                        {
                                            if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -vertPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -horzPosShift);
                                            }

                                        }
                                        //mostly facing the right in world space
                                        else if ((angleFromRight <= angleFromForward) && (angleFromRight <= angleFromLeft) && (angleFromRight <= angleFromBack)) 
                                        {
                                            if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -horzPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, vertPosShift);
                                            }
                                        }
                                        //mostly facing the left in world space
                                        else if ((angleFromLeft <= angleFromForward) && (angleFromLeft <= angleFromRight) && (angleFromLeft <= angleFromBack))
                                        {
                                            if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, horzPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -vertPosShift);
                                            }
                                        }
                                        else //the back is the most smallest angle, which means user is mostly facing the back in world space
                                        {
                                            if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, vertPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, horzPosShift);
                                            }
                                        }
                                    }
                                    else //facing down
                                    {
                                        //check for facing mostly forward only
                                        if ((angleFromForward <= angleFromRight) && (angleFromForward <= angleFromLeft) && (angleFromForward <= angleFromBack))
                                        {
                                            if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -vertPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, +horzPosShift);
                                            }
                                        }
                                        //mostly facing the right in world space
                                        else if ((angleFromRight <= angleFromForward) && (angleFromRight <= angleFromLeft) && (angleFromRight <= angleFromBack)) 
                                        {
                                            if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, horzPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, vertPosShift);
                                            }
                                        }
                                        //mostly facing the left in world space
                                        else if ((angleFromLeft <= angleFromForward) && (angleFromLeft <= angleFromRight) && (angleFromLeft <= angleFromBack))
                                        {
                                            if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -horzPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -vertPosShift);
                                            }
                                        }
                                        else //the back is the most smallest angle, which means user is mostly facing the back in world space
                                        {
                                            if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, vertPosShift);
                                            }
                                            else
                                            {
                                                worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -horzPosShift);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    UnityEngine.Debug.Log("Possible combinations of visible axes of rotation not found");  
                                } //end of checking for an actual swipe
                            }//end of checking if it's actually 3Dtouch control mode

                            //restore the worldcordinatesystem back to original place and rotation for later calculatons
                            selectedItem.transform.parent = null;
                            worldCordinateSystem.transform.rotation = original;
                            worldCordinateSystem.transform.position = oldSpot;
                        }//end of checking for object selected or not
                    }//end of checking for finger.moved


                    if (Input.GetTouch(i).phase == TouchPhase.Ended) //user lifts up their finger
                    {
                        //user did a tap and not a long touch
                        if (Time.time - touchTime <= touchTimeThreshold) 
                        {
                            //cast from camera to the position of the finger touch on screen
                            Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                            if (Physics.SphereCast(touchRay, 0.01f, out touchHit)) //if user touchs something with finger on the screen
                            {
                                if (!objectIsSelected) //if the user doesn't have an object already selected
                                {
                                    //get a reference to the object the user touched
                                    touchObject = touchHit.collider.gameObject.name;
                                    selectedItem = GameObject.Find(touchObject);

                                    //if the object the user touched was actually a game object that can be moved
                                    if (selectedItem.tag.Equals("gameObject"))
                                    {
                                        //show the manipulation panel for user to select the proper mode they would like to use
                                        threeDTouchControlPanel.SetActive(true);
                                        isThreeDTouchPanelVisible = true;
                                        
                                        //position the manipulation panel just to the right of where the user touched
                                        threeDTouchControlPanel.transform.position = Camera.main.WorldToScreenPoint(touchHit.point);

                                        if (Input.GetTouch(i).position.x > (Screen.width / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x - threeDTouchControlPanel.GetComponent<RectTransform>().rect.width,
                                                threeDTouchControlPanel.transform.position.y, threeDTouchControlPanel.transform.position.z);
                                        }
                                        else //(touchHit.point.y < (Screen.height / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x + threeDTouchControlPanel.GetComponent<RectTransform>().rect.width,
                                                threeDTouchControlPanel.transform.position.y, threeDTouchControlPanel.transform.position.z);    
                                        }

                                        if (Input.GetTouch(i).position.y > (Screen.height / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x,
                                                threeDTouchControlPanel.transform.position.y - threeDTouchControlPanel.GetComponent<RectTransform>().rect.height, threeDTouchControlPanel.transform.position.z);
                                        }
                                        else //(touchHit.point.y < (Screen.height / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x,
                                                threeDTouchControlPanel.transform.position.y + threeDTouchControlPanel.GetComponent<RectTransform>().rect.height, threeDTouchControlPanel.transform.position.z);
                                        }




                                        //turn off gravity on the object
                                        selectedItem.GetComponent<Rigidbody>().useGravity = false;
                                        //lock the constraints on the object
                                        selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                        //set the object selected flag accordingly
                                        objectIsSelected = true;
                                    }
                                    else //user did not select an actual gameobject and could be touching the actual mesh of the world
                                    {
                                        selectedItem = null;
                                    }
                                }
                                else //user already has an object selected, so they want to un-select it
                                {
                                    //make sure the user has already selected a proper object manipulation method before un-selecting
                                    if (!isThreeDTouchPanelVisible) 
                                    {
                                        //grab a reference to the object the user touched
                                        string tempObjectName = touchHit.collider.gameObject.name;
                                        GameObject tempObject = GameObject.Find(tempObjectName);

                                        if (tempObject == selectedItem) //if the user selects the same object that was already selected
                                        {
                                            this.SwitchThreeDTouchControlMode(threeDTouchClearAllModes); //clear all modes and make them false
                                            
                                            //reset line variables back to default 
                                            currentLineWidth = defaultLineWidth;
                                            currentLineLength = defaultLineLength;

                                            //reset the object to the way it was 
                                            //turn on gravity
                                            selectedItem.GetComponent<Rigidbody>().useGravity = true;
                                            //Unlock the constraints 
                                            selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                            //deselect the object and have it go back to interacting with the game world
                                            selectedItem = null;
                                            objectIsSelected = false;
                                        }
                                    } //end of checking if multitouch controlpanel is visible
                                }//end of checking user has an object selcted
                            }//end of sphere cast
                        }//end of checking touch time
                    }//end of touch phase ended
                }//end of checking in active space
            }//end for loop for touches
        } //end of multi touch control mode-------------------------------------------------------------------------------------------

        
        //RAYCASTING USED FOR CONTROL METHODS THAT REQUIRE RAYCATING FROM CAMERA CENTER
        //Raycast hit to see what the user is looking at
        RaycastHit hit;
        Ray ray;
        if (isARMode)
        {
            //create new ray in the directon the user is looking based on the AR Camera
            ray = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
        }
        else
        {
            //create new ray in the directon the user is looking based on the VR Camera
            ray = new Ray(VRCamera.transform.position, VRCamera.transform.forward);
        }
        
        //from the camera to the object
        if (Physics.Raycast(ray, out hit))
        {
            string objectName = hit.collider.gameObject.name;

            //-------------------------------------------------------integrated view input control mode-----------------------------
            if (isIVICenterControlMode)
            {
                for (int i = 0; i < Input.touchCount; i++) //check for any user touches on the screen
                {
                    //user touched the screen within bounds, 
                    //This is to double check te user did not accidentally touch the edge of the screen
                    if (activeTouchSpace.Contains(Input.GetTouch(i).position)) 
                    {
                        //user just touched the screen with finger
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            //check if the user has an object already selected
                            if (!objectIsSelected)
                            {
                                UnityEngine.Debug.Log("object selected");

                                //get reference to object
                                selectedItem = GameObject.Find(objectName);

                                //check if the user has actually 
                                if (selectedItem.tag.Equals("gameObject"))
                                {
                                    //set the name of the display text to the object name
                                    infoDisplay.text = objectName;

                                    //lock the object to the AR camera so it move accordingly
                                    selectedItem.transform.parent = ARCamera.transform;
                                    selectedItem.GetComponent<Rigidbody>().isKinematic = true;

                                    //turn off gravity
                                    selectedItem.GetComponent<Rigidbody>().useGravity = false;

                                    //Unlock the constraints 
                                    selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                    //set boolean flag so the objct will be moved during late update
                                    objectIsSelected = true;
                                }
                                else
                                {
                                    selectedItem = null;
                                    objectIsSelected = false;
                                }
                            } //end of object is selected
                        } // end of touch phase begin

                        if (Input.GetTouch(i).phase == TouchPhase.Ended) //when the user lifts up their finger off the screen
                        {
                                //user already has an object selected, so we assume they want to set the object down
                                UnityEngine.Debug.Log("object is DE-selected");

                            //delete lines if they are visible
                            if (xAxisLine != null)
                            {
                                GameObject.Destroy(xAxisLine);
                                xAxisLine = null;
                                isXAxisVisible = false;
                            }
                            if (yAxisLine != null)
                            {
                                GameObject.Destroy(yAxisLine);
                                yAxisLine = null;
                                isYAxisVisible = false;
                            }
                            if (zAxisLine != null)
                            {
                                GameObject.Destroy(zAxisLine);
                                zAxisLine = null;
                                isZAxisVisible = false;
                            }

                            //unparent the object so it doesn't move with the tango device anymore
                            selectedItem.transform.parent = null;
                            selectedItem.GetComponent<Rigidbody>().isKinematic = false;

                            //Unlock the constraints
                            selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                            //turn on gravity
                            selectedItem.GetComponent<Rigidbody>().useGravity = true;

                            //desyn the object to be in place and no longer moving with the camera and the device
                            selectedItem = null;

                            objectIsSelected = false;
                        } //end of touch.ended                    
                    } //end of checking for active touch space
                } //end of checking for touches
            }//---------------------------------------------------END OF integrated view input control mode-----------------------------
            else
            {
                //final check that the phone control modes are not set correctly
                if ((!isthreeDTouchControlMode) && (!isHOMERControlMode) && (!isModifiedIVIMode) && (!isHybridMode))
                {
                    UnityEngine.Debug.Log("ERROR during USING CONTROLS, Controls not properly set");
                }
            }
        }
        else //the ray cast from center to screen did not hit anything
        {
            infoDisplay.text = ""; //set the display text blank
        }


        //BEGINNING OF HOMER-S MODE----------------------------------------------------------------------------------------------------
        if (isHOMERControlMode)
        {
            //string holder for object that was touched
            string touchObject;
            //go through all finger touches on the screen
            for (int i = 0; i < Input.touchCount; i++)
            {
                //make sure the user did not accidentally touch the edge of the screen
                if (activeTouchSpace.Contains(Input.GetTouch(i).position))
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        touchTime = Time.time; //used for differentiating a fast tap from a long tap
                    }

                    if (Input.GetTouch(i).phase == TouchPhase.Ended) //user lifts up their finger
                    {
                        if (Time.time - touchTime <= touchTimeThreshold) //user did do a tap and not a long touch
                        {
                            //cast from camera to the position of the finger touch on screen
                            Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                            if (Physics.SphereCast(touchRay, 0.01f, out touchHit)) //if user touches something with finger
                            {
                                if (!objectIsSelected) //if the user doesn't have an object already selected
                                {
                                    //get a reference to the object the user touched
                                    touchObject = touchHit.collider.gameObject.name;
                                    selectedItem = GameObject.Find(touchObject);

                                    if (selectedItem.tag.Equals("gameObject"))
                                    {
                                        //show the manipulation panel for user to select the proper mode they would like to use
                                        threeDTouchControlPanel.SetActive(true);
                                        isThreeDTouchPanelVisible = true;
                                        threeDTouchControlPanel.transform.position = Camera.main.WorldToScreenPoint(touchHit.point);

                                        if (Input.GetTouch(i).position.x > (Screen.width / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x - threeDTouchControlPanel.GetComponent<RectTransform>().rect.width,
                                                threeDTouchControlPanel.transform.position.y, threeDTouchControlPanel.transform.position.z);
                                        }
                                        else //(touchHit.point.y < (Screen.height / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x + threeDTouchControlPanel.GetComponent<RectTransform>().rect.width,
                                                threeDTouchControlPanel.transform.position.y, threeDTouchControlPanel.transform.position.z);
                                        }

                                        if (Input.GetTouch(i).position.y > (Screen.height / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x,
                                                threeDTouchControlPanel.transform.position.y - threeDTouchControlPanel.GetComponent<RectTransform>().rect.height, threeDTouchControlPanel.transform.position.z);
                                        }
                                        else //(touchHit.point.y < (Screen.height / 2))
                                        {
                                            threeDTouchControlPanel.transform.position = new Vector3(threeDTouchControlPanel.transform.position.x,
                                                threeDTouchControlPanel.transform.position.y + threeDTouchControlPanel.GetComponent<RectTransform>().rect.height, threeDTouchControlPanel.transform.position.z);
                                        }

                                        //turn off gravity
                                        selectedItem.GetComponent<Rigidbody>().useGravity = false;
                                        //lock the constraints 
                                        selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                        //set the object selected flag accordingly
                                        objectIsSelected = true;
                                    }
                                    else
                                    {
                                        selectedItem = null;
                                    }
                                }
                                else //user already has an object selected
                                {
                                    if (!isThreeDTouchPanelVisible)
                                    {
                                        //grab a reference to the object the user touched
                                        string tempObjectName = touchHit.collider.gameObject.name;
                                        GameObject tempObject = GameObject.Find(tempObjectName);

                                        if (tempObject == selectedItem) //if the user selects the already selected object and not another one
                                        {
                                            selectedItem.transform.parent = null;
                                            selectedItem.GetComponent<Rigidbody>().isKinematic = false;

                                            this.SwitchHomerTouchControlMode(homerSClearAllModes); //clear all modes and make them false
                                            currentLineWidth = defaultLineWidth;
                                            currentLineLength = defaultLineLength;

                                            //reset the object to the way it was 
                                            //turn on gravity
                                            selectedItem.GetComponent<Rigidbody>().useGravity = true;

                                            //Unlock the constraints 
                                            selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                            selectedItem = null;
                                            objectIsSelected = false;
                                        }
                                    } //end of checking if multitouch controlpanel is visible
                                }//end of checking user has an object selcted
                            }//end of sphere cast
                        }//end of checking touch time
                    }//end of touch phase ended
                }//end of checking in active space
            }//end for loop for touches
        }//END OF HOMER-S CONTROLS----------------------------------------------------------------------------------------------------

        //BEGIN OF HYBRID CONTROL MODES-----------------------------------------------------------------------------------------------
        if (isHybridMode)
        {
            //string holder for object that was touched
            string touchObject;
            //go through all finger touches on the screen
            for (int i = 0; i < Input.touchCount; i++)
            {
                //make sure the user did not accidentally touch the edge of the screen
                if (activeTouchSpace.Contains(Input.GetTouch(i).position))
                {
                    if ((Input.GetTouch(i).phase == TouchPhase.Began) && (Input.touchCount == 1))
                    {
                        touchTime = Time.time; //used for differentiating a fast tap from a long tap
                        initialTouch = Input.GetTouch(i);
                        //cast from camera to the position of the finger touch on screen
                        Ray firstTouchRay = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                        if (Physics.SphereCast(firstTouchRay, 0.01f, out touchHit)) //if user touches something with finger
                        {
                            //get a reference to the object the user touched
                            touchObject = touchHit.collider.gameObject.name;
                            GameObject tempItem = GameObject.Find(touchObject);

                            if (!objectIsSelected)
                            {
                                if (tempItem.tag.Equals("gameObject"))
                                {
                                    selectedItem = tempItem;
                                    //draw the y line cast going down
                                    //create lines to cast out from object denoting proper axes of control
                                    yAxisLine = new GameObject();
                                    yAxisLine.AddComponent<LineRenderer>();
                                    LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                                    yline.material = new Material(Shader.Find("Standard"));
                                    yline.material.color = Color.green;
                                    yline.startWidth = defaultLineWidth;
                                    yline.endWidth = defaultLineWidth;
                                    currentLineLength = defaultLineLength;

                                    //isYAxisVisible = true;
                                    //CAST SHADOW-------------------------------

                                    objectIsSelected = true;
                                    recentlySelected = true;
                                }
                            }
                            else //object is already selected
                            {
                                if (tempItem == selectedItem)
                                {
                                    recentlySelected = false;
                                    translateOnly = true;
                                    //forceUnlock = true;
                                    //lock the object to the AR camera so it move accordingly
                                    selectedItem.transform.parent = ARCamera.transform;
                                    selectedItem.GetComponent<Rigidbody>().isKinematic = true;

                                    //unlock the constraints
                                    selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                    //turn off gravity
                                    selectedItem.GetComponent<Rigidbody>().useGravity = false;
                                }
                            }
                        }
                        else//user it just touching the screen and not the object
                        { }
                    }//end of touch begin


                    //update the touch position
                    if (Input.GetTouch(i).phase == TouchPhase.Stationary && (Input.touchCount == 1))
                    {
                        initialTouch = Input.GetTouch(i);
                    }



                    if (Input.GetTouch(i).phase == TouchPhase.Ended && (Input.touchCount == 1))
                    {
                        //user did a tap and not a long touch
                        if (Time.time - touchTime <= touchTimeThreshold) 
                        {
                            //cast from camera to the position of the finger touch on screen
                            Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                            if (Physics.SphereCast(touchRay, 0.01f, out touchHit)) //if user touches something with finger
                            {
                                //grab a reference to the object the user touched
                                string tempObjectName = touchHit.collider.gameObject.name;
                                GameObject tempObject = GameObject.Find(tempObjectName);

                                if (recentlySelected) //check that we don't deselect the same touch we touched the object
                                {
                                    recentlySelected = false;
                                }
                                else //object is already selected, so unselect it
                                {
                                    if (tempObject == selectedItem) //if the user selects the already selected object and not another one
                                    {
                                        //delete the y axis line
                                        if (yAxisLine != null)
                                        {
                                            GameObject.Destroy(yAxisLine);
                                            yAxisLine = null;
                                            isYAxisVisible = false;
                                        }

                                        isXAxisVisible = false;
                                        isZAxisVisible = false;

                                        //unparent the object so it doesn't move with the tango device anymore
                                        selectedItem.transform.parent = null;
                                        selectedItem.GetComponent<Rigidbody>().isKinematic = false;

                                        //Unlock the constraints
                                        selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                        //turn on gravity
                                        selectedItem.GetComponent<Rigidbody>().useGravity = true;

                                        //clear all used flags
                                        selectedItem = null;
                                        objectIsSelected = false;
                                        translateOnly = false;
                                        forceUnlock = false;
                                    }
                                }//end of object selected
                            }//end of sphere cast
                        }//end of time check for tap
                        else
                        {
                            if ((forceUnlock || translateOnly) && (selectedItem != null))
                            {
                                //unparent the object so it doesn't move with the tango device anymore
                                selectedItem.transform.parent = null;
                                selectedItem.GetComponent<Rigidbody>().isKinematic = false;
                                forceUnlock = false;
                                translateOnly = false;
                            }
                        }
                    }//end touch.ended
                }//end of checking for touch in active space
            }//end of checking all touches
        }//END OF HYBRID CONTROL MODE--------------------------------------------------------------------------------------------------


        //BEGIN OF MY MODIFIED IVI CONTROL MODES-----------------------------------------------------------------------------------------------
        if (isModifiedIVIMode)
        {
            RaycastHit centerHit;
            Ray centerRay;
            if (isARMode)
            {
                //create new ray in the directon the user is looking based on the AR Camera
                centerRay = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
            }
            else
            {
                //create new ray in the directon the user is looking based on the VR Camera
                centerRay = new Ray(VRCamera.transform.position, VRCamera.transform.forward);
            }

            //raycast from the camera to the object
            if (Physics.Raycast(ray, out centerHit))
            {
                string objectName = centerHit.collider.gameObject.name;

                //get reference to the object selected
                selectedItem = GameObject.Find(objectName);

                //check if the user has actually touched a game object
                if (selectedItem.tag.Equals("gameObject"))
                {
                    //set the name of the display text to the object name
                    infoDisplay.text = objectName;

                    for (int i = 0; i < Input.touchCount; i++) //check for any user touches on the screen
                    {
                        //user touched the screen within bounds, 
                        //This is to double check te user did not accidentally touch the edge of the screen
                        if (activeTouchSpace.Contains(Input.GetTouch(i).position))
                        {
                            //user just touched the screen with finger
                            if ((Input.GetTouch(i).phase == TouchPhase.Began) && (Input.touchCount == 1))
                            {
                                //acumTime += Input.GetTouch(0).deltaTime;
                                acumTime += Time.deltaTime;
                                //set the code for it to be invisible
                                originalMaterial = selectedItem.GetComponent<MeshRenderer>().material;
                                //get initial touch position
                                initialTouch = Input.GetTouch(i);
                            }//end of touch phase begin


                            if (Input.GetTouch(i).phase == TouchPhase.Stationary)
                            {
                                //update the touch position
                                 initialTouch = Input.GetTouch(i);
                            }

                            if (Input.GetTouch(i).phase == TouchPhase.Stationary || Input.GetTouch(i).phase == TouchPhase.Moved)
                            {
                                //count the timer
                                acumTime += Time.deltaTime;
                                
                                //Long tap, so it's a hold
                                if (acumTime >= minTimeToBeCountedAsHold)
                                {
                                    //set the boolean to denote it was a long hold
                                    isLongHold = true;

                                    //reset the touch incase the user was in the middle of double of tapping and changed thier mind
                                    firstTouchActivated = false;

                                    //check if the user has an object already selected
                                    if (objectIsSelected == false)
                                    {
                                        UnityEngine.Debug.Log("object selected");

                                        //check if the user has actually touched a game object
                                        if (selectedItem.tag.Equals("gameObject"))
                                        {
                                            //set the name of the display text to the object name
                                            infoDisplay.text = objectName;

                                            //lock the object to the AR camera so it move accordingly
                                            selectedItem.transform.parent = ARCamera.transform;
                                            selectedItem.GetComponent<Rigidbody>().isKinematic = true;

                                            //grab a copy of the original color of the object
                                            originalObjectColor = new Color(selectedItem.GetComponent<MeshRenderer>().material.color.r,
                                                selectedItem.GetComponent<MeshRenderer>().material.color.g,
                                                selectedItem.GetComponent<MeshRenderer>().material.color.b,
                                                selectedItem.GetComponent<MeshRenderer>().material.color.a);

                                            //set the code for it to be invisible
                                            selectedItem.GetComponent<MeshRenderer>().material = transparentObjectColor;

                                            //set the object transparent so you can see through it 
                                            selectedItem.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, .1f);

                                            //turn off gravity
                                            selectedItem.GetComponent<Rigidbody>().useGravity = false;

                                            //Unlock the constraints 
                                            selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                            //set boolean flag so the objct will be moved during late update
                                            objectIsSelected = true;
                                        }
                                    } //end of object is selected
                                }//end of checking if hold or tap
                            } //end of touch stationary


                            if (Input.GetTouch(i).phase == TouchPhase.Ended) //when the user lifts up their finger off the screen
                            {
                                if (isLongHold)
                                {
                                    isLongHold = false;

                                    //user already has an object selected, so we assume they want to set the object down
                                    UnityEngine.Debug.Log("object is DE-selected");

                                    //Recolor the shader to the way it was
                                    selectedItem.GetComponent<MeshRenderer>().material.color = originalObjectColor;

                                    //delete lines if they are visible
                                    if (xAxisLine != null)
                                    {
                                        GameObject.Destroy(xAxisLine);
                                        xAxisLine = null;
                                        isXAxisVisible = false;
                                    }
                                    if (yAxisLine != null)
                                    {
                                        GameObject.Destroy(yAxisLine);
                                        yAxisLine = null;
                                        isYAxisVisible = false;
                                    }
                                    if (zAxisLine != null)
                                    {
                                        GameObject.Destroy(zAxisLine);
                                        zAxisLine = null;
                                        isZAxisVisible = false;
                                    }

                                    //unparent the object so it doesn't move with the tango device anymore
                                    selectedItem.transform.parent = null;
                                    selectedItem.GetComponent<Rigidbody>().isKinematic = false;

                                    //turn it back to opaque
                                    selectedItem.GetComponent<MeshRenderer>().material = originalMaterial;

                                    //Unlock the constraints
                                    selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                                    //turn on gravity
                                    selectedItem.GetComponent<Rigidbody>().useGravity = true;

                                    //desyn the object to be in place and no longer moving with the camera and the device
                                    selectedItem = null;

                                    objectIsSelected = false;
                                    acumTime = 0;
                                }
                                else //it could be a double tap
                                {
                                    if (firstTouchActivated == false)
                                    {
                                        firstTouchActivated = true;
                                    }
                                    else //user already tapped once already, this is the second tap
                                    {
                                        firstTouchActivated = false;
                                        acumTime = 0;

                                        //check if the user has actually touched a game object
                                        if (selectedItem.tag.Equals("gameObject"))
                                        {
                                            if (Vector3.Distance(ARCamera.transform.position, selectedItem.transform.position) > maxDistFromUserInMeters)
                                            {
                                                //shift the object to the front of the user's view
                                                selectedItem.transform.position = ARCamera.transform.position + (ARCamera.transform.forward * objectDistanceFromCamera);

                                                //lock the constraints
                                                selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                                                //turn off gravity
                                                selectedItem.GetComponent<Rigidbody>().useGravity = false;
                                            }
                                        }
                                    }
                                }//end of checking for double tap
                            }//end of touch.ended                    
                        } //end of checking for touch space
                    }//end of checking for touches
                }//end of checking if it's a gameobject    
            }//end of raycasting to see if object is hit
        }//END OF MY MODIFIED IVI CONTROL MODE--------------------------------------------------------------------------------------------------
    } //END OF UPDATE FUNCTION------------------------------------------------------------------------------------------------------------------


    //LATE UPDATE DRAWS LAST TO AVOID THE FLICKERING ISSUE ON RENDERING OBJECTS DURING TRANSLATION
    private void LateUpdate()
    {
        //TRANSLATE THE SHOWN AXIS LINES CORRECTLY WHEN USING 3D TOUCH CONTROLS-------------------------------------------------------------
        if ((selectedItem != null) && (objectIsSelected) && isthreeDTouchControlMode)
        {
            //shift an objct to the z plane to do movement
            if (selectedItem != null)
            {
                zPlane.transform.position = selectedItem.transform.position;
                zPlane.transform.forward = ARCamera.transform.forward;
            }

            //Do comparisons to find out where the user is looking
            horizontalAngle = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.forward);
            verticleAngle = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.up);

            if (horizontalAngle < 35f || horizontalAngle > 135f) //x is the visible axis
            {
                //move on x axis only for touch input
                isXAxisVisible = true;
                isZAxisVisible = false;
                zAxisLine.GetComponent<LineRenderer>().enabled = false;

                if (horizontalAngle > 135f)
                {
                    isFacingBack = true;
                    isFacingForward = false;
                }
                else
                {
                    isFacingBack = false;
                    isFacingForward = true;
                }
            }
            else //z is the most visible axis
            {
                //move on z axis only for touch input
                isZAxisVisible = true;
                isXAxisVisible = false;
                xAxisLine.GetComponent<LineRenderer>().enabled = false;

                if (Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.right) > 90f)
                {
                    isFacingLeft = true;
                    isFacingRight = false;
                }
                else //object is to the right of the object
                {
                    isFacingLeft = false;
                    isFacingRight = true;
                }
            }

            if ((verticleAngle < 35f) || (verticleAngle > 135f)) //
            {
                //turn off y axis as the z axis will now take input for touch slides
                isXAxisVisible = true;
                isYAxisVisible = false;
                yAxisLine.GetComponent<LineRenderer>().enabled = false;

                if (verticleAngle < 45f)
                {
                    isFacingUp = true;
                }
                else
                {
                    isFacingUp = false;
                }
            }
            else //move on the y axis as the axis will take up/down slide input
            {
                isYAxisVisible = true;
            }

            //draw the axis of translation
            if ((xAxisLine != null) && isXAxisVisible)
            {
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                xAxisLine.GetComponent<LineRenderer>().enabled = true;
                xline.SetPosition(0, new Vector3(selectedItem.transform.position.x - currentLineLength, selectedItem.transform.position.y, selectedItem.transform.position.z));
                xline.SetPosition(1, new Vector3(selectedItem.transform.position.x + currentLineLength, selectedItem.transform.position.y, selectedItem.transform.position.z));
            }
            if ((yAxisLine != null) && isYAxisVisible)
            {
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                yAxisLine.GetComponent<LineRenderer>().enabled = true;
                yline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y - currentLineLength, selectedItem.transform.position.z));
                yline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y + currentLineLength, selectedItem.transform.position.z));
            }
            if ((zAxisLine != null) && isZAxisVisible)
            {
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();
                zAxisLine.GetComponent<LineRenderer>().enabled = true;
                zline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z - currentLineLength));
                zline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z + currentLineLength));
            }
        }//----------------END OF 3D TOUCH SPECIFIC CODE-----------------------------------------------------------------------------------


        //HOMER-S specific code for moving objects and lines of axis---------------------------------------------------
        if ((selectedItem != null) && (objectIsSelected) && isHOMERControlMode)
        {
            //draw the axis of translation
            if ((xAxisLine != null))
            {
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                xAxisLine.GetComponent<LineRenderer>().enabled = true;
                xline.SetPosition(0, new Vector3(selectedItem.transform.position.x - currentLineLength, selectedItem.transform.position.y, selectedItem.transform.position.z));
                xline.SetPosition(1, new Vector3(selectedItem.transform.position.x + currentLineLength, selectedItem.transform.position.y, selectedItem.transform.position.z));
            }
            if ((yAxisLine != null))
            {
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                yAxisLine.GetComponent<LineRenderer>().enabled = true;
                yline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y - currentLineLength, selectedItem.transform.position.z));
                yline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y + currentLineLength, selectedItem.transform.position.z));
            }
            if ((zAxisLine != null))
            {
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();
                zAxisLine.GetComponent<LineRenderer>().enabled = true;
                zline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z - currentLineLength));
                zline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z + currentLineLength));
            }

            //return childs lock position to correct values
            if (isHomersTranslationMode)
            {
                selectedItem.transform.rotation = lockRot;
            }
            else if (isHomersRotationMode)
            {
                selectedItem.transform.position = lockPos;
            }
            else { }
        } //END OF HOMER-S Controls----------------------------------------------------------------------------------


        //HYBRID CONTROL SPECIFIC CODE-------------------------------------------------------------------------------
        if ((selectedItem != null) && (objectIsSelected) && isHybridMode)
        {
            if (!translateOnly)
            {
                Quaternion original = new Quaternion(worldCordinateSystem.transform.rotation.x, worldCordinateSystem.transform.rotation.y,
                    worldCordinateSystem.transform.rotation.z, worldCordinateSystem.transform.rotation.w);

                Vector3 oldSpot = new Vector3(worldCordinateSystem.transform.position.x, worldCordinateSystem.transform.position.y, 
                    worldCordinateSystem.transform.position.z);

                worldCordinateSystem.transform.position = selectedItem.transform.position;

                //shift an objct to the z plane to do movement
                if (selectedItem != null)
                {
                    zPlane.transform.position = selectedItem.transform.position;
                    zPlane.transform.forward = ARCamera.transform.forward;
                }

                if (Input.touchCount >= 2)
                {
                    prevFrameWas2Touch = true;
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);

                    //if at least one finger has moved
                    if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                    {
                        //check the delta distance between them
                        pinchDistance = Vector2.Distance(touch1.position, touch2.position);
                        float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
                                                              touch2.position - touch2.deltaPosition);
                        pinchDistanceDelta = pinchDistance - prevDistance;

                        //if it's greater than a minimum threshold, it's a pinch!
                        if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance)
                        {
                            pinchDistanceDelta *= pinchRatio;
                        }
                        else
                        {
                            pinchDistance = pinchDistanceDelta = 0;
                        }

                        //or check the delta angle between them
                        turnAngle = Angle(touch1.position, touch2.position);
                        float prevTurn = Angle(touch1.position - touch1.deltaPosition,
                                               touch2.position - touch2.deltaPosition);
                        turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

                        //if it's greater than a minimum threshold, it's a turn!
                        if (Mathf.Abs(turnAngleDelta) > minTurnAngle)
                        {
                            turnAngleDelta *= pinchTurnRatio;
                        }
                        else
                        {
                            turnAngle = turnAngleDelta = 0;
                        }

                        if (pinchDistanceDelta != 0)
                        {
                            float moveDist = pinchDistanceDelta * 500;
                            Vector3.MoveTowards(selectedItem.transform.position, ARCamera.transform.position, moveDist);
                        }

                        if (turnAngleDelta != 0)
                        {
                            selectedItem.transform.parent = worldCordinateSystem.transform;
                            worldCordinateSystem.transform.Rotate(ARCamera.transform.forward, turnAngleDelta);

                            selectedItem.transform.parent = null;
                            worldCordinateSystem.transform.rotation = original;
                        }
                    }
                }
                else
                {
                    horizontalAngle = Vector3.Angle(ARCamera.transform.forward, Vector3.forward);
                    verticleAngle = Vector3.Angle(ARCamera.transform.forward, Vector3.up);

                    if (horizontalAngle < 35f || horizontalAngle > 135f) //x is the visible axis
                    {
                        //move on x axis only for touch input
                        isXAxisVisible = true;
                        isZAxisVisible = false;

                        if (horizontalAngle > 135f)
                        {
                            isFacingBack = true;
                            isFacingForward = false;
                        }
                        else
                        {
                            isFacingBack = false;
                            isFacingForward = true;
                        }
                    }
                    else //z is the most visible axis
                    {
                        //move on z axis only for touch input
                        isZAxisVisible = true;
                        isXAxisVisible = false;

                        if (Vector3.Angle(ARCamera.transform.forward, Vector3.right) > 90f)
                        {
                            isFacingLeft = true;
                            isFacingRight = false;
                        }
                        else //object is to the right of the object
                        {
                            isFacingLeft = false;
                            isFacingRight = true;
                        }
                    }

                    if ((verticleAngle < 35f) || (verticleAngle > 135f))
                    {
                        //turn off y axis as the z axis will now take input for touch slides
                        isXAxisVisible = true;
                        isYAxisVisible = false;

                        if (verticleAngle < 45f)
                        {
                            isFacingUp = true;
                        }
                        else
                        {
                            isFacingUp = false;
                        }
                    }
                    else //move on the y axis as the axis will take up/down slide input
                    {
                        isYAxisVisible = true;
                    }

                    if ((Input.touchCount == 1)) //user did do a tap and not a long touch   
                    {
                        if (prevFrameWas2Touch)
                        {
                            prevFrameWas2Touch = false;
                        }
                        else if (Input.GetTouch(0).phase == TouchPhase.Moved && (Time.time - touchTime > touchTimeThreshold))
                        {
                            float horzPosShift = (initialTouch.position.x - Input.GetTouch(0).position.x) / 50;
                            float vertPosShift = (initialTouch.position.y - Input.GetTouch(0).position.y) / 50;

                            selectedItem.transform.transform.parent = worldCordinateSystem.transform;

                            if (isXAxisVisible && isYAxisVisible) //x is the horz,  y is the vert
                            {
                                if (isFacingBack) //user is facing the front of the object looking in the same direction as world.back
                                {//swiping left makes the object rotate right in world cordinate
                                    if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, vertPosShift);
                                    }
                                    else
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                    }
                                }
                                else //user facing the back of object looking in the same directio as world.forward
                                {//swiping left makes the object rotate left in the world cordinate system 
                                    if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -vertPosShift);
                                    }
                                    else
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                    }
                                }
                            }
                            else if (isZAxisVisible && isYAxisVisible) //z is the horz, y is the vert
                            {
                                if (isFacingLeft)
                                {
                                    if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -vertPosShift);
                                    }
                                    else
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                    }
                                }
                                else
                                {
                                    if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.up, horzPosShift);
                                    }
                                    else
                                    {
                                        worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, vertPosShift);
                                    }
                                }
                            }
                            else if (isXAxisVisible && isZAxisVisible) 
                            {
                                //check for the actual direction of the camera
                                float angleFromForward = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.forward);
                                float angleFromRight = Vector3.Angle(ARCamera.transform.forward, worldCordinateSystem.transform.right);
                                float angleFromLeft = Vector3.Angle(ARCamera.transform.forward, -worldCordinateSystem.transform.right);
                                float angleFromBack = Vector3.Angle(ARCamera.transform.forward, -worldCordinateSystem.transform.forward);

                                if (isFacingUp) //rotate on x axis is transfrom.right, rotate on y is transform.up, z axis is transform.forward
                                {
                                    //check for facing mostly forward only
                                    if ((angleFromForward <= angleFromRight) && (angleFromForward <= angleFromLeft) && (angleFromForward <= angleFromBack))
                                    {
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -vertPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -horzPosShift);
                                        }
                                    }
                                    else if ((angleFromRight <= angleFromForward) && (angleFromRight <= angleFromLeft) && (angleFromRight <= angleFromBack)) //mostly facing the right in world space
                                    {
                                        if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -horzPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, vertPosShift);
                                        }
                                    }
                                    else if ((angleFromLeft <= angleFromForward) && (angleFromLeft <= angleFromRight) && (angleFromLeft <= angleFromBack))//mostly facing the left in world space
                                    {
                                        if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, horzPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -vertPosShift);
                                        }
                                    }
                                    else //the back is the most smallest angle, which means user is mostly facing the back in wolrd space
                                    {
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, vertPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, horzPosShift);
                                        }
                                    }
                                }
                                else //facing down
                                {
                                    //check for facing mostly forward only
                                    if ((angleFromForward <= angleFromRight) && (angleFromForward <= angleFromLeft) && (angleFromForward <= angleFromBack))
                                    {
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -vertPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, +horzPosShift);
                                        }
                                    }
                                    else if ((angleFromRight <= angleFromForward) && (angleFromRight <= angleFromLeft) && (angleFromRight <= angleFromBack)) //mostly facing the right in world space
                                    {
                                        if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, horzPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, vertPosShift);
                                        }
                                    }
                                    else if ((angleFromLeft <= angleFromForward) && (angleFromLeft <= angleFromRight) && (angleFromLeft <= angleFromBack))//mostly facing the left in world space
                                    {
                                        if (Math.Abs(vertPosShift) < Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, -horzPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -vertPosShift);
                                        }
                                    }
                                    else //the back is the most smallest angle, which means user is mostly facing the back in wolrd space
                                    {
                                        if (Math.Abs(vertPosShift) > Math.Abs(horzPosShift))
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.right, vertPosShift);
                                        }
                                        else
                                        {
                                            worldCordinateSystem.transform.Rotate(worldCordinateSystem.transform.forward, -horzPosShift);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                UnityEngine.Debug.Log("Possible combinations of visible axes of rotation not found");
                            }
                        }
                        else //do nothing for three or more screen touches
                        { }

                        //clear all flags used
                        isXAxisVisible = false;
                        isYAxisVisible = false;
                        isZAxisVisible = false;
                        selectedItem.transform.parent = null;
                        worldCordinateSystem.transform.rotation = original;
                        worldCordinateSystem.transform.position = oldSpot;
                    }//end of checking for one touch
                }//end of only one touch
            } //end of translation mode

            //draw the axis of translation
            if ((yAxisLine != null))
            {
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                yAxisLine.GetComponent<LineRenderer>().enabled = true;
                yline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y - currentLineLength, selectedItem.transform.position.z));
                yline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z));
            }

        } //END OF HYBRID MODE--------------------------------------------------------------------------------------------------


        //MODIFIED INTEGRATED VIEW INPUT SPECIFIC CODE--------------------------------------------------------------------------
        if ((selectedItem != null) && (objectIsSelected) && isModifiedIVIMode)
        {
            //move the object closer or farther from the users perspective
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                float distOnYaxis = (initialTouch.position.y - Input.GetTouch(0).position.y) / 3250;

                //if the user pulls finger down or up while moving
                if (Math.Abs(initialTouch.position.y - Input.GetTouch(0).position.y) > 8)
                {
                    float distance = Vector3.Distance(selectedItem.transform.position, ARCamera.transform.position);
                    infoDisplay.text = "Distance is: " + distance.ToString();
                    
                    //person moved finger up on the screen
                    if ((Input.GetTouch(0).position.y < Input.GetTouch(0).deltaPosition.y)) 
                    {
                        if (distance < 3.5f)
                        {
                            selectedItem.transform.position = (ARCamera.transform.position + (ARCamera.transform.forward * ((distance + distOnYaxis))));
                        }
                        else if (distance > 3.5f)
                        {
                            selectedItem.transform.position = (ARCamera.transform.position + (ARCamera.transform.forward * 3.49f));
                        }
                        else
                        { }
                        
                    }
                    else //person moved finger down on the screen
                    {
                        if (distance > 0.5f)
                        {
                            selectedItem.transform.position = (ARCamera.transform.position + (ARCamera.transform.forward * ((distance - distOnYaxis))));
                        }
                        else if (distance < 0.5f)
                        {
                            selectedItem.transform.position = (ARCamera.transform.position + (ARCamera.transform.forward * 0.599f));
                        }
                        else { }
                    }         
                }
            }

            //draw axes if applicable
            if ((xAxisLine != null) && (yAxisLine != null) && (zAxisLine != null))
            {
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();

                xline.SetPosition(0, new Vector3(selectedItem.transform.position.x - defaultLineLength, selectedItem.transform.position.y, selectedItem.transform.position.z));
                xline.SetPosition(1, new Vector3(selectedItem.transform.position.x + defaultLineLength, selectedItem.transform.position.y, selectedItem.transform.position.z));

                yline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y - defaultLineLength, selectedItem.transform.position.z));
                yline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y + defaultLineLength, selectedItem.transform.position.z));

                zline.SetPosition(0, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z - defaultLineLength));
                zline.SetPosition(1, new Vector3(selectedItem.transform.position.x, selectedItem.transform.position.y, selectedItem.transform.position.z + defaultLineLength));
            }
        }
    } // END OF LATE UPDATE LOOP


    //public function for the "Switch AR/VR Mode Button"
    public void flipARVRCameraMode()
    {
        if (isARMode) //switch to VR mode
        {
            this.SwitchCameraMode(VRCameraEnum);
        }
        else //switch to AR mode
        {
            this.SwitchCameraMode(ARCameraEnum);
        }
    }


    //Helper function for flipping the camera mode for the function (flipARVRCameraMode())
    private void SwitchCameraMode(int cameraEnum)
    {
        if ((cameraEnum == ARCameraEnum) && (isARMode == false))
        {
            if (isVRMode)
            {

                //Set the position of the ARCamera the same as the VR Camera
                ARCamera.transform.position = new Vector3(VRCamera.transform.position.x, VRCamera.transform.position.y, VRCamera.transform.position.z);
                ARCamera.transform.rotation = new Quaternion(VRCamera.transform.rotation.x, VRCamera.transform.rotation.y, VRCamera.transform.rotation.z, VRCamera.transform.rotation.w);

                //set the camera's on and off
                VRCamera.SetActive(false);
                ARCamera.SetActive(true);

                //set the flags accordingly
                isVRMode = false;
                isARMode = true;
            }
        }
        else if ((cameraEnum == VRCameraEnum) && (isVRMode == false))
        {
            if (isARMode)
            {
                //Set the position of the ARCamera the same as the VR Camera
                VRCamera.transform.position = new Vector3(ARCamera.transform.position.x, ARCamera.transform.position.y, ARCamera.transform.position.z);
                VRCamera.transform.rotation = new Quaternion(ARCamera.transform.rotation.x, ARCamera.transform.rotation.y, ARCamera.transform.rotation.z, ARCamera.transform.rotation.w);

                //set the camera's on and off
                ARCamera.SetActive(false);
                VRCamera.SetActive(true);

                //set the flags accordingly
                isARMode = false;
                isVRMode = true;
            }
        }
        else
        {
            UnityEngine.Debug.Log("ERROR in switching camera mode, unknown camera enum found");
        }

    }


    public void SwitchControlMode(int controlEnum)
    {
        //clear all flags for all modes that have extra options
        isThreeDTouchRotationMode = false;
        isThreeDTouchTranslationMode = false;
        isHomersRotationMode = false;
        isHomersTranslationMode = false;
        if (xAxisLine != null)
        {
            GameObject.Destroy(xAxisLine);
            xAxisLine = null;
            isXAxisVisible = false;
        }
        if (yAxisLine != null)
        {
            GameObject.Destroy(yAxisLine);
            yAxisLine = null;
            isYAxisVisible = false;
        }
        if (zAxisLine != null)
        {
            GameObject.Destroy(zAxisLine);
            zAxisLine = null;
            isZAxisVisible = false;
        }

        if ((controlEnum == threeDTouchControlMode) && (isthreeDTouchControlMode == false))
        {
            //Turn off gaze reticle as to not confuse the user
            centerGazeReticle.SetActive(false);

            isthreeDTouchControlMode = true;
            isIVICenterControlMode = false;
            isHOMERControlMode = false;
            isHybridMode = false;
            isModifiedIVIMode = false;
            currentSelectionDisplay.text = "obj. manipulation mode:\n - 3DTouchControls";
            currentSelectionDisplay.color = Color.green;
        }
        else if ((controlEnum == IVICenterMode) && (isIVICenterControlMode == false))
        {
            //Turn on gaze reticle for the user to use
            centerGazeReticle.SetActive(true);

            isIVICenterControlMode = true;
            isthreeDTouchControlMode = false;
            isHOMERControlMode = false;
            isHybridMode = false;
            isModifiedIVIMode = false;
            currentSelectionDisplay.text = "obj. manipulation mode:\n - IntegratedViewInput Center Controls";
            currentSelectionDisplay.color = Color.blue;
        }
        else if ((controlEnum == HOMERSMode) && (isHOMERControlMode == false))
        {
            //Turn off gaze reticle for the user
            centerGazeReticle.SetActive(false);

            isHOMERControlMode = true;
            isthreeDTouchControlMode = false;
            isIVICenterControlMode = false;
            isHybridMode = false;
            isModifiedIVIMode = false;
            currentSelectionDisplay.text = "obj. manipulation mode:\n - HOMER-S Controls";
            currentSelectionDisplay.color = Color.red;
        }
        else if ((controlEnum == hybridMode) && (isHybridMode == false))
        {
            //Turn off gaze reticle as to not confuse the user
            centerGazeReticle.SetActive(false);

            isHybridMode = true;
            isHOMERControlMode = false;
            isthreeDTouchControlMode = false;
            isIVICenterControlMode = false;
            isModifiedIVIMode = false;
            currentSelectionDisplay.text = "obj. manipulation mode:\n - Hybrid Controls";
            currentSelectionDisplay.color = Color.yellow;
        }
        else if ((controlEnum == modifiedIVICenterMode) && (isModifiedIVIMode == false))
        {
            //Turn on gaze reticle for the user to use
            centerGazeReticle.SetActive(true);

            isModifiedIVIMode = true;
            isHybridMode = false;
            isHOMERControlMode = false;
            isthreeDTouchControlMode = false;
            isIVICenterControlMode = false;
            currentSelectionDisplay.text = "obj. manipulation mode:\n - Modified IVI Controls";
            currentSelectionDisplay.color = Color.cyan;
        }
        else
        {
            UnityEngine.Debug.Log("ERROR during changing control modes: bad enum found or already on proper controls");
        }
    }

    public void SwitchThreeDTouchControlMode(int threeDTouchControlEnum)
    {
        if (isthreeDTouchControlMode) //check that we are acutally using multi touch controls
        {
            if (threeDTouchControlEnum == threeDTouchTranslationMode)
            {
                isThreeDTouchTranslationMode = true;
                isThreeDTouchRotationMode = false;
                threeDTouchModeDisplay.text = "Translation Mode";

                if (xAxisLine != null)
                {
                    GameObject.Destroy(xAxisLine);
                    xAxisLine = null;
                    isXAxisVisible = false;
                }
                if (yAxisLine != null)
                {
                    GameObject.Destroy(yAxisLine);
                    yAxisLine = null;
                    isYAxisVisible = false;
                }
                if (zAxisLine != null)
                {
                    GameObject.Destroy(zAxisLine);
                    zAxisLine = null;
                    isZAxisVisible = false;
                }

                //create lines to cast out from object denoting proper axes of control
                xAxisLine = new GameObject();
                yAxisLine = new GameObject();
                zAxisLine = new GameObject();
                xAxisLine.AddComponent<LineRenderer>();
                yAxisLine.AddComponent<LineRenderer>();
                zAxisLine.AddComponent<LineRenderer>();
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();
                xline.material = new Material(Shader.Find("Standard"));
                yline.material = new Material(Shader.Find("Standard"));
                zline.material = new Material(Shader.Find("Standard"));
                xline.material.color = Color.red;
                yline.material.color = Color.green;
                zline.material.color = Color.blue;
                xline.startWidth = defaultLineWidth;
                xline.endWidth = defaultLineWidth;
                yline.startWidth = defaultLineWidth;
                yline.endWidth = defaultLineWidth;
                zline.startWidth = defaultLineWidth;
                zline.endWidth = defaultLineWidth;

                //set the length of the line to be long
                currentLineLength = defaultLineLength;

                //Unlock the translation constraints but keep the rotation restraint
                selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

                //remove the manipulation display
                threeDTouchControlPanel.SetActive(false);
                isThreeDTouchPanelVisible = false;
            }
            else if (threeDTouchControlEnum == threeDTouchRotationMode)
            {
                isThreeDTouchRotationMode = true;
                isThreeDTouchTranslationMode = false;
                threeDTouchModeDisplay.text = "Rotation Mode";

                if (xAxisLine != null)
                {
                    GameObject.Destroy(xAxisLine);
                    xAxisLine = null;
                    isXAxisVisible = false;
                }
                if (yAxisLine != null)
                {
                    GameObject.Destroy(yAxisLine);
                    yAxisLine = null;
                    isYAxisVisible = false;
                }
                if (zAxisLine != null)
                {
                    GameObject.Destroy(zAxisLine);
                    zAxisLine = null;
                    isZAxisVisible = false;
                }

                //create lines to cast out from object denoting proper axes of rotation,
                //MAKE THEM SMALLER TO DIFFERENTIATE MOVEMENT
                xAxisLine = new GameObject();
                yAxisLine = new GameObject();
                zAxisLine = new GameObject();
                xAxisLine.AddComponent<LineRenderer>();
                yAxisLine.AddComponent<LineRenderer>();
                zAxisLine.AddComponent<LineRenderer>();
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();
                xline.material = new Material(Shader.Find("Standard"));
                yline.material = new Material(Shader.Find("Standard"));
                zline.material = new Material(Shader.Find("Standard"));
                xline.material.color = Color.red;
                yline.material.color = Color.green;
                zline.material.color = Color.blue;
                currentLineWidth = 0.015f;
                xline.startWidth = currentLineWidth;
                xline.endWidth = currentLineWidth;
                yline.startWidth = currentLineWidth;
                yline.endWidth = currentLineWidth;
                zline.startWidth = currentLineWidth;
                zline.endWidth = currentLineWidth;

                //make the line be small to denote rotation
                currentLineLength = .3f;

                //Unlock the rotation constraints but freeze the translation
                selectedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

                //remove the manipulation display
                threeDTouchControlPanel.SetActive(false);
                isThreeDTouchPanelVisible = false;
            }
            else if (threeDTouchControlEnum == threeDTouchClearAllModes)
            {
                //delete all lines of axes
                if (xAxisLine != null)
                {
                    GameObject.Destroy(xAxisLine);
                    xAxisLine = null;
                    isXAxisVisible = false;
                }
                if (yAxisLine != null)
                {
                    GameObject.Destroy(yAxisLine);
                    yAxisLine = null;
                    isYAxisVisible = false;
                }
                if (zAxisLine != null)
                {
                    GameObject.Destroy(zAxisLine);
                    zAxisLine = null;
                    isZAxisVisible = false;
                }

                //set all flags false
                threeDTouchControlPanel.SetActive(false);
                isThreeDTouchPanelVisible = false;
                isThreeDTouchTranslationMode = false;
                isThreeDTouchRotationMode = false;
                threeDTouchModeDisplay.text = "";
            }

            else
            {
                UnityEngine.Debug.Log("UNKNOWN ENUM FOUND FOR CHANGING 3D TOUCH CONTROLS");
            }
        }
    }


    public void SwitchHomerTouchControlMode(int homerSControlEnum)
    {
        if (isHOMERControlMode) //check that we are acutally using HOMER-S touch controls
        {
            if (homerSControlEnum == homerSClearAllModes)
            {
                //delete all lines of axes
                if (xAxisLine != null)
                {
                    GameObject.Destroy(xAxisLine);
                    xAxisLine = null;
                    isXAxisVisible = false;
                }
                if (yAxisLine != null)
                {
                    GameObject.Destroy(yAxisLine);
                    yAxisLine = null;
                    isYAxisVisible = false;
                }
                if (zAxisLine != null)
                {
                    GameObject.Destroy(zAxisLine);
                    zAxisLine = null;
                    isZAxisVisible = false;
                }

                //clear all flags
                threeDTouchModeDisplay.text = "";
                isHomersTranslationMode = false;
                isHomersRotationMode = false;
                threeDTouchControlPanel.SetActive(false);
                isThreeDTouchPanelVisible = false;
            }
            else if (homerSControlEnum == homerSTranslationMode)
            {
                isHomersTranslationMode = true;
                isHomersRotationMode = false;
                threeDTouchModeDisplay.text = "Translation Mode";

                //delete all lines of axes
                if (xAxisLine != null)
                {
                    GameObject.Destroy(xAxisLine);
                    xAxisLine = null;
                    isXAxisVisible = false;
                }
                if (yAxisLine != null)
                {
                    GameObject.Destroy(yAxisLine);
                    yAxisLine = null;
                    isYAxisVisible = false;
                }
                if (zAxisLine != null)
                {
                    GameObject.Destroy(zAxisLine);
                    zAxisLine = null;
                    isZAxisVisible = false;
                }

                //create lines to cast out from object denoting proper axes of control
                xAxisLine = new GameObject();
                yAxisLine = new GameObject();
                zAxisLine = new GameObject();
                xAxisLine.AddComponent<LineRenderer>();
                yAxisLine.AddComponent<LineRenderer>();
                zAxisLine.AddComponent<LineRenderer>();
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();
                xline.material = new Material(Shader.Find("Standard"));
                yline.material = new Material(Shader.Find("Standard"));
                zline.material = new Material(Shader.Find("Standard"));
                xline.material.color = Color.red;
                yline.material.color = Color.green;
                zline.material.color = Color.blue;
                xline.startWidth = defaultLineWidth;
                xline.endWidth = defaultLineWidth;
                yline.startWidth = defaultLineWidth;
                yline.endWidth = defaultLineWidth;
                zline.startWidth = defaultLineWidth;
                zline.endWidth = defaultLineWidth;

                //set the length of the line to be long
                currentLineLength = defaultLineLength;

                //remove the manipulation display
                threeDTouchControlPanel.SetActive(false);
                isThreeDTouchPanelVisible = false;

                //Unlock the translation constraints but keep the rotation restraint
                lockRot = selectedItem.transform.rotation;

                selectedItem.transform.parent = ARCamera.transform;
                selectedItem.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if (homerSControlEnum == homerSRotationMode)
            {
                isHomersTranslationMode = false;
                isHomersRotationMode = true;
                threeDTouchModeDisplay.text = "Rotation Mode";

                //delete all lines of axes
                if (xAxisLine != null)
                {
                    GameObject.Destroy(xAxisLine);
                    xAxisLine = null;
                    isXAxisVisible = false;
                }
                if (yAxisLine != null)
                {
                    GameObject.Destroy(yAxisLine);
                    yAxisLine = null;
                    isYAxisVisible = false;
                }
                if (zAxisLine != null)
                {
                    GameObject.Destroy(zAxisLine);
                    zAxisLine = null;
                    isZAxisVisible = false;
                }

                //create lines to cast out from object denoting proper axes of rotation, 
                //MAKE THEM SMALLER TO DIFFERENTIATE MOVEMENT
                xAxisLine = new GameObject();
                yAxisLine = new GameObject();
                zAxisLine = new GameObject();
                xAxisLine.AddComponent<LineRenderer>();
                yAxisLine.AddComponent<LineRenderer>();
                zAxisLine.AddComponent<LineRenderer>();
                LineRenderer xline = xAxisLine.GetComponent<LineRenderer>();
                LineRenderer yline = yAxisLine.GetComponent<LineRenderer>();
                LineRenderer zline = zAxisLine.GetComponent<LineRenderer>();
                xline.material = new Material(Shader.Find("Standard"));
                yline.material = new Material(Shader.Find("Standard"));
                zline.material = new Material(Shader.Find("Standard"));
                xline.material.color = Color.red;
                yline.material.color = Color.green;
                zline.material.color = Color.blue;
                currentLineWidth = 0.015f;
                xline.startWidth = currentLineWidth;
                xline.endWidth = currentLineWidth;
                yline.startWidth = currentLineWidth;
                yline.endWidth = currentLineWidth;
                zline.startWidth = currentLineWidth;
                zline.endWidth = currentLineWidth;

                //make the line be small to denote rotation
                currentLineLength = .3f;

                //Unlock the translation constraints but keep the rotation restraint
                lockPos = selectedItem.transform.position;

                selectedItem.transform.parent = ARCamera.transform;
                selectedItem.GetComponent<Rigidbody>().isKinematic = true;

                //remove the manipulation display
                threeDTouchControlPanel.SetActive(false);
                isThreeDTouchPanelVisible = false;
            }
            else
            {
                UnityEngine.Debug.Log("UNKNOWN ENUM FOUND FOR CHANGING HOMER-S CONTROLS");
            }
        }
    }


    //helper function to help with calculating angles
    private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }

}// END OF CLASS

