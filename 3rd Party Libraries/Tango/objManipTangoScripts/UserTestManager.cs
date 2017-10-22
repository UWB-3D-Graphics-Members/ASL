using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserTestManager : MonoBehaviour
{
    //References to all objects
    public GameObject cube;
    public GameObject trophy;
    public GameObject sphere;

    public GameObject trophy2;
    public GameObject trophy3;
    public GameObject trophy4;
    public GameObject trophy5;

    public GameObject smallCube;
    public GameObject smallCube2;
    public GameObject smallCube3;
    public GameObject smallDesignatedArea;
    public GameObject cylinder;

    public GameObject smallRamp;
    public GameObject stackOfCubes;
    public GameObject largeDesignatedArea;

    public GameObject ARCamera;

    public GameObject cubeShadow;
    public GameObject trophyShadow;
    public GameObject sphereShadow;
    public GameObject smallCubeShadow;
    public GameObject smallCube2Shadow;
    public GameObject smallCube3Shadow;
    public GameObject rampShadow;
    public GameObject cylinderShadow;

    public GameObject trophy2Shadow;
    public GameObject trophy3Shadow;
    public GameObject trophy4Shadow;
    public GameObject trophy5Shadow;

    public GameObject startTestButton;
    public GameObject starParticleSystem;


    //timer for timing the user
    private float timer = 0;
    private bool isTimerOn = false;

    private float cylinderTimer = 3;
    private float maxTimeUntilCyliderReset = 3;
    private bool cylinderPadIsActive = false;

    //used to display time results
    public Text timeDisplay;
    public Text successfulTestDisplay;

    //used to hold the original orientation of the objects
    private Quaternion boxOrientation;
    private Quaternion cylinderOrientation;
    private Quaternion designatedSpotOrientation;

    //Reference to the UserTest panel to move out of the way
    public RectTransform userTestPanel;
    private bool isSelectionMenuOpen = true;
    private Vector3 menuOpenLocation;
    private Vector3 menuClosedLocation;

    //enum for tests
    private static int test0 = 0;
    private static int test1 = 1;
    private static int test2 = 2;
    private static int test3 = 3;
    private static int test4 = 4;
    private static int test5 = 5;
    private static int test6 = 6;
    private static int test7 = 7;
    private static int test8 = 8;
    private int currentTestNumber = -1;

    private GameObject successPad;
    private String successMessage = "TEST COMPLETED! \n AMAZING JOB!";


    // Use this for initialization
    void Start ()
    {
        //hide the timer until user is finished
        timeDisplay.text = "";
        successfulTestDisplay.text = "";
        startTestButton.SetActive(false);
        //starParticleSystem.SetActive(false);
        starParticleSystem.transform.position = new Vector3(1000, 100, 100);

        //make all objects inactive
        this.clearAllObjects();

        //set the basic test scene true
        this.setTest(test0);

        //calculate the position of the user test menu to hide and show
        menuOpenLocation = userTestPanel.localPosition;
        menuClosedLocation = new Vector3(menuOpenLocation.x - 400, menuClosedLocation.y - 475);
        this.changeUserMenuState();

        boxOrientation = smallCube.transform.rotation;
        cylinderOrientation = cylinder.transform.rotation;
        designatedSpotOrientation = smallDesignatedArea.transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isTimerOn)
        {
            timer += Time.deltaTime; //increment the timer

            if (successPad != null) //the success pad is in the AR scene
            {
                Ray Upwards = new Ray(successPad.transform.position, Vector3.up);
                RaycastHit hit;

                if (currentTestNumber == test1)
                {
                    if (Physics.Raycast(Upwards, out hit, .015f)) //ray cast up 1.5cm, if it hits a game object
                    {
                        //if the object above the success pad is a gameobject
                        if (hit.transform.gameObject.tag.Equals("gameObject"))
                        {
                            //if the object is not moving and is stationary above the success pad
                            Vector3 vel = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
                            if ((Math.Abs(vel.magnitude) <= .1f) && hit.transform.gameObject.GetComponent<Rigidbody>().useGravity == true)
                            {
                                this.stopTimer(); //stop the timer becuase the task is accomplished
                            }
                        }
                    }
                }
                else if (currentTestNumber == test2)
                {
                    if (Physics.Raycast(Upwards, out hit, .065f)) //ray cast up 1.5cm, if it hits a game object
                    {
                        if (hit.transform.gameObject.tag.Equals("gameObject"))
                        {
                            Vector3 vel = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
                            if ((Math.Abs(vel.magnitude) <= .1f) && hit.transform.gameObject.GetComponent<Rigidbody>().useGravity == true)
                            {
                                this.stopTimer();
                            }
                        }
                    }
                }
                else if (currentTestNumber == test3)
                {
                    if (cylinderPadIsActive)
                    {
                        if (Physics.Raycast(Upwards, out hit, .015f)) //ray cast up 1.5cm, if it hits a game object
                        {
                            if (hit.transform.gameObject.tag.Equals("gameObject"))
                            {
                                Vector3 vel = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
                                if (hit.transform.gameObject.GetComponent<Rigidbody>().useGravity == true)
                                {
                                    this.stopTimer();
                                }
                            }
                        }
                        cylinderTimer -= Time.deltaTime;
                        if (cylinderTimer <= 0)
                        {
                            cylinderPadIsActive = false;
                            cylinderTimer = maxTimeUntilCyliderReset;
                        }
                    }
                    else
                    {
                        Upwards = new Ray(smallRamp.transform.GetChild(0).transform.position, Vector3.up);
                        if (Physics.Raycast(Upwards, out hit, .015f)) //ray cast up 1.5cm, if it hits a game object
                        {
                            if ((hit.transform.gameObject.tag.Equals("gameObject")) && (hit.transform.gameObject.GetComponent<Rigidbody>().useGravity == true))
                            {
                                cylinderPadIsActive = true;
                            }
                        }
                    }
                }
                else if (currentTestNumber == 5)
                {
                    if (Physics.Raycast(Upwards, out hit, .51f)) //ray cast up, if it hits a game object
                    {
                        if (hit.transform.gameObject.tag.Equals("gameObject"))
                        {
                            Vector3 vel = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
                            if ((Math.Abs(vel.magnitude) <= .1f) && hit.transform.gameObject.GetComponent<Rigidbody>().useGravity == true)
                            {
                                this.stopTimer();
                            }
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(Upwards, out hit, .03f)) //ray cast up 1.5cm, if it hits a game object
                    {
                        if (hit.transform.gameObject.tag.Equals("gameObject"))
                        {
                            Vector3 vel = hit.transform.gameObject.GetComponent<Rigidbody>().velocity;
                            if ((Math.Abs(vel.magnitude) <= .1f) && hit.transform.gameObject.GetComponent<Rigidbody>().useGravity == true)
                            {
                                this.stopTimer();
                            }
                        }
                    }
                }
            }
        }
	}

    public void setTest(int testEnum)
    {
        //cast a ray forward from the camera
        Ray forward = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
        RaycastHit spotToPlaceDestination;
        this.clearAllObjects();
        timeDisplay.text = "";
        successfulTestDisplay.text = "";

        //set the start test button to on
        startTestButton.SetActive(true);

        //set the success display to off until user completes the task
        starParticleSystem.transform.position = new Vector3(1000, 100, 100);

        currentTestNumber = testEnum;

        if (testEnum == test0)
        {
            //set the proper objects true
            cube.SetActive(true);
            trophy.SetActive(true);
            sphere.SetActive(true);

            trophy2.SetActive(true);
            trophy3.SetActive(true);
            trophy4.SetActive(true);
            trophy5.SetActive(true);


            //shift them to the proper spot
            cube.transform.position = new Vector3(-0.272f, 0, 0.8f);
            trophy.transform.position = new Vector3(0.2f, -0.3f, 0.4f);
            sphere.transform.position = new Vector3(0.633f, 0.172f, 1.3f);

            trophy2.transform.position = new Vector3(0.354f,-0.3f,0.402f);
            trophy3.transform.position = new Vector3(0.516f,-0.3f,0.397f);
            trophy4.transform.position = new Vector3(0.671f,-0.3f,0.388f);
            trophy5.transform.position = new Vector3(0.83f, -0.301f,0.388f);
            
            //set shadows
            cubeShadow.SetActive(true);
            trophyShadow.SetActive(true);
            sphereShadow.SetActive(true);

            trophy2Shadow.SetActive(true);
            trophy3Shadow.SetActive(true);
            trophy4Shadow.SetActive(true);
            trophy5Shadow.SetActive(true);

        }
        else if (testEnum == test1) //move small cube to a designated area 35cm away, 45 degrees to the corner
        {
            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                //place the designated area
                smallDesignatedArea.transform.position = spotToPlaceDestination.point + (Vector3.up * .001f);

                //drop the cube to move 35cm away from the designated area
                smallCube.transform.position = new Vector3(smallDesignatedArea.transform.position.x + 0.2479f, 
                    smallDesignatedArea.transform.position.y + 0.05f, smallDesignatedArea.transform.position.z - 0.2479f);

                //turn off gravity
                smallCube.GetComponent<Rigidbody>().useGravity = true;
                smallDesignatedArea.GetComponent<Rigidbody>().useGravity = true;

                //unlock the constraints
                smallCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                smallDesignatedArea.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                //rotate the objects accordingly
                smallCube.transform.rotation = boxOrientation;
                smallDesignatedArea.transform.rotation = designatedSpotOrientation;

                //set the objects to active
                smallCube.SetActive(true);
                smallDesignatedArea.SetActive(true);

                //set the shadows
                smallCubeShadow.SetActive(true);

                successPad = smallDesignatedArea;
            }
        }
        else if (testEnum == test2) //move small cube to the top of a stack of cubes 35cm away, 45 degrees
        {
            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                //place the stack of cubes in the area
                smallCube2.transform.position = spotToPlaceDestination.point + (Vector3.up * .0505f);
                smallCube3.transform.position = smallCube2.transform.position + (smallCube2.transform.up * .1025f);

                //move the target cube 35cm from the stack
                smallCube.transform.position = new Vector3(smallCube2.transform.position.x + 0.2479f, smallCube2.transform.position.y, 
                    smallCube2.transform.position.z - 0.2479f);

                //unlock the contraints
                smallCube.GetComponent<Rigidbody>().useGravity = true;

                //unlock the constraints
                smallCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                smallCube3.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                smallCube2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                smallCube.transform.rotation = boxOrientation;
                smallCube2.transform.rotation = designatedSpotOrientation;
                smallCube3.transform.rotation = designatedSpotOrientation;

                //set the object to exist
                smallCube2.SetActive(true);
                smallCube3.SetActive(true);
                smallCube.SetActive(true);

                //set the shadows
                smallCube2Shadow.SetActive(true);
                smallCube3Shadow.SetActive(true);
                smallCubeShadow.SetActive(true);

                successPad = smallCube3;
            }
        }
        else if (testEnum == test3) // move a cylinder 45 degrees and drop it at the top of the ramp to have it slide down to the designated area
        {
            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                //place the ramp in the area
                smallRamp.transform.position = new Vector3(spotToPlaceDestination.point.x, spotToPlaceDestination.point.y + .1f, spotToPlaceDestination.point.z);

                //place designated area
                smallDesignatedArea.transform.position = new Vector3(smallRamp.transform.position.x, smallRamp.transform.position.y - .08f, smallRamp.transform.position.z - 0.2f);

                //place cylinder 35cm away and rotate it 45 degrees
                cylinder.transform.position = new Vector3(smallDesignatedArea.transform.position.x + 0.2479f, smallDesignatedArea.transform.position.y + 0.05f, smallDesignatedArea.transform.position.z - 0.2479f);

                //unlock the constraints
                smallDesignatedArea.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                smallRamp.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                smallRamp.transform.GetChild(1).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                cylinder.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                //turn on the gravity on the object
                cylinder.GetComponent<Rigidbody>().useGravity = true;

                smallDesignatedArea.transform.rotation = designatedSpotOrientation;
                cylinder.transform.rotation = cylinderOrientation;

                //make the objects active
                smallRamp.SetActive(true);
                smallDesignatedArea.SetActive(true);
                cylinder.SetActive(true);

                //set the shadows
                cylinderShadow.SetActive(true);
                rampShadow.SetActive(true);

                successPad = smallDesignatedArea;
            }

        }
        else if (testEnum == test4) // move a large cube from one table to the next given a barrier is dividing the two tables
        {
            // TOP DOWN VIEW--------------------------------------------------
            //     - move cube from table1 to table2 with barrier
            //
            //        table1        barrier         table2
            //                         |=|
            //     |----------|        |=|        |----------|
            //     |          |        |=|        |          |
            //     |  [cube]  |        |=|        |          |
            //     |          |        |=|        |          |
            //     |----------|        |=|        |----------|
            //                         |=|
            //
            //---------------------------------------------------------------

            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                cube.transform.position = spotToPlaceDestination.point + (Vector3.up * .16f);

                //rotate it the cube accordingly
                cube.transform.rotation = boxOrientation;

                //set the constraints
                cube.GetComponent<Rigidbody>().useGravity = true;
                cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                //set the object active
                cube.SetActive(true);

                //set the shadow on the cube on
                cubeShadow.SetActive(true);
            }
        }
        else if (testEnum == test5) // move a large cube from one table in front of the user to stack onto another table behind the user
        {
            // TOP DOWN VIEW-------------------------------------------------------------
            //     - move cube from table1 to table 2 which is positioned behind the user
            //
            //        table1         User           table2
            //     |----------|    --\         |--------------|
            //     |          |       \==      |              |
            //     |  [cube]  |          }     |[stackofcubes]|
            //     |          |       /==      |              |
            //     |----------|    --/         |--------------|
            //
            //
            //---------------------------------------------------------------------------


            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                cube.transform.position = spotToPlaceDestination.point + (Vector3.up * .16f);

                //rotate it the cube accordingly
                cube.transform.rotation = boxOrientation;

                //set the constraints
                cube.GetComponent<Rigidbody>().useGravity = true;
                cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                //set the object active
                cube.SetActive(true);

                //set the shadow on the cube on
                cubeShadow.SetActive(true);


            }
        }
        else if (testEnum == test6) // move a large cube from one table to another table given there is a barrier occluding the user's view
        {
            // TOP DOWN VIEW-------------------------------------------------------------
            //     - move cube from table1 to table2 with barrier blocking user's view
            //
            //        table1                                      table2
            //     |----------|                                |----------|
            //     |          |                                |          |
            //     |  [cube]  |                                |          |
            //     |          |                                |          |
            //     |----------|                                |----------|
            //                             barrier
            //                    |=======================|
            //                    |=======================|
            //
            //
            //
            // 
            //                         \            /
            //                          \          /
            //                           |  user  |
            //                           |________|
            //
            //---------------------------------------------------------------------------

            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                cube.transform.position = spotToPlaceDestination.point + (Vector3.up * .16f);

                //rotate it the cube accordingly
                cube.transform.rotation = boxOrientation;

                //set the constraints
                cube.GetComponent<Rigidbody>().useGravity = true;
                cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                //set the object active
                cube.SetActive(true);

                //set the shadow on the cube on
                cubeShadow.SetActive(true);
            }
        }
        else if (testEnum == test7) // move a large object from one table to another table given that the user is standing far away from the two tables
        {
            // TOP DOWN VIEW-------------------------------------------------------------
            //     - move LARGE cube from table1 to table2 with barrier blocking user's view
            //
            //        table1                                      table2
            //     |----------|                                |----------|
            //     |          |                                |          |
            //     |  [cube]  |                                |          |
            //     |          |                                |          |
            //     |----------|                                |----------|
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            // 
            //                         \            /
            //                          \          /
            //                           |  user  |
            //                           |________|
            //
            //---------------------------------------------------------------------------

            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                cube.transform.position = spotToPlaceDestination.point + (Vector3.up * .16f);

                //rotate it the cube accordingly
                cube.transform.rotation = boxOrientation;

                //set the constraints
                cube.GetComponent<Rigidbody>().useGravity = true;
                cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                //set the object active
                cube.SetActive(true);

                //set the shadow on the cube on
                cubeShadow.SetActive(true);
            }
        }
        else if (testEnum == test8) // move a small object from one table to another table given that the user is standing far away from the two tables
        {
            // TOP DOWN VIEW-------------------------------------------------------------
            //     - move SMALL cube from table1 to table2 with barrier blocking user's view
            //
            //        table1                                      table2
            //     |-----------|                                |----------|
            //     |  (small)  |                                |          |
            //     |    [ ]    |                                |          |
            //     |           |                                |          |
            //     |-----------|                                |----------|
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            // 
            //                         \            /
            //                          \          /
            //                           |  user  |
            //                           |________|
            //
            //---------------------------------------------------------------------------

            //if the ray cast hit a spot
            if (Physics.Raycast(forward, out spotToPlaceDestination))
            {
                smallCube.transform.position = spotToPlaceDestination.point + (Vector3.up * .16f);
            
                //rotate it the cube accordingly
                smallCube.transform.rotation = boxOrientation;

                //set the constraints
                smallCube.GetComponent<Rigidbody>().useGravity = true;
                smallCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                //set the object active
                smallCube.SetActive(true);

                //set the shadow on the cube to on
                smallCubeShadow.SetActive(true);
            }
        }
        else
        {
            currentTestNumber = -1;
            UnityEngine.Debug.Log("BAD ENUM FOUND WHEN CHANGING USER TESTS...");
        }
    }

    public void setLargeSuccessArea()
    {
        //cast a ray forward from the camera
        Ray forward = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
        RaycastHit spotToPlaceDestination;

        //if the ray cast hit a spot
        if (Physics.Raycast(forward, out spotToPlaceDestination))
        {
            //place the designated area
            largeDesignatedArea.transform.position = spotToPlaceDestination.point + (Vector3.up * .001f);

            //lock contraints so the object doesn't move or rotate
            largeDesignatedArea.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            largeDesignatedArea.transform.rotation = designatedSpotOrientation;

            //set the object active and set it as the success pad for the timer
            largeDesignatedArea.SetActive(true);
            successPad = largeDesignatedArea;
        }

    }

    public void setLargeStack()
    {
        //cast a ray forward from the camera
        Ray forward = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
        RaycastHit spotToPlaceDestination;

        //if the ray cast hit a spot
        if (Physics.Raycast(forward, out spotToPlaceDestination))
        {
            //place the designated area
            stackOfCubes.transform.position = spotToPlaceDestination.point + (Vector3.up * .502f);

            //lock contraints so the object doesn't move or rotate
            stackOfCubes.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            stackOfCubes.transform.rotation = designatedSpotOrientation;

            //set the object active and set it as the success pad for the timer
            stackOfCubes.SetActive(true);
            currentTestNumber = test5;
            successPad = stackOfCubes;
        }

    }

    public void setSmallSuccessArea()
    {
        //cast a ray forward from the camera
        Ray forward = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
        RaycastHit spotToPlaceDestination;

        //if the ray cast hit a spot
        if (Physics.Raycast(forward, out spotToPlaceDestination))
        {
            //place the designated area
            smallDesignatedArea.transform.position = spotToPlaceDestination.point + (Vector3.up * .001f);

            //lock contraints so the object doesn't move or rotate
            smallDesignatedArea.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            smallDesignatedArea.transform.rotation = designatedSpotOrientation;

            //set the object active and set it as the success pad for the timer
            smallDesignatedArea.SetActive(true);
            successPad = smallDesignatedArea;
        }
    }

    private void clearAllObjects()
    {
        //set all objects false in the beginning
        smallCube.SetActive(false);
        smallCube2.SetActive(false);
        smallCube3.SetActive(false);
        smallDesignatedArea.SetActive(false);
        cylinder.SetActive(false);
        smallRamp.SetActive(false);
        stackOfCubes.SetActive(false);
        largeDesignatedArea.SetActive(false);
        cube.SetActive(false);
        trophy.SetActive(false);
        sphere.SetActive(false);

        trophy2.SetActive(false);
        trophy3.SetActive(false);
        trophy4.SetActive(false);
        trophy5.SetActive(false);


        //set off the gravity for all objects
        smallCube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        smallCube2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        smallCube3.GetComponent<Rigidbody>().velocity = Vector3.zero;
        smallDesignatedArea.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cylinder.GetComponent<Rigidbody>().velocity = Vector3.zero;
        smallRamp.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        stackOfCubes.GetComponent<Rigidbody>().velocity = Vector3.zero;
        largeDesignatedArea.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        trophy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        sphere.GetComponent<Rigidbody>().velocity = Vector3.zero;
        smallCube.GetComponent<Rigidbody>().useGravity = false;
        smallCube2.GetComponent<Rigidbody>().useGravity = false;
        smallCube3.GetComponent<Rigidbody>().useGravity = false;
        smallDesignatedArea.GetComponent<Rigidbody>().useGravity = false;
        cylinder.GetComponent<Rigidbody>().useGravity = false;
        smallRamp.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().useGravity = false;
        stackOfCubes.GetComponent<Rigidbody>().useGravity = false;
        largeDesignatedArea.GetComponent<Rigidbody>().useGravity = false;
        cube.GetComponent<Rigidbody>().useGravity = false;
        trophy.GetComponent<Rigidbody>().useGravity = false;
        sphere.GetComponent<Rigidbody>().useGravity = false;

        trophy2.GetComponent<Rigidbody>().useGravity = false;
        trophy3.GetComponent<Rigidbody>().useGravity = false;
        trophy4.GetComponent<Rigidbody>().useGravity = false;
        trophy5.GetComponent<Rigidbody>().useGravity = false;

        //set the shadows to off
        cubeShadow.SetActive(false);
        trophyShadow.SetActive(false);
        sphereShadow.SetActive(false);
        smallCubeShadow.SetActive(false);
        smallCube2Shadow.SetActive(false);
        smallCube3Shadow.SetActive(false);
        rampShadow.SetActive(false);
        cylinderShadow.SetActive(false);

        trophy2Shadow.SetActive(false); 
        trophy3Shadow.SetActive(false); 
        trophy4Shadow.SetActive(false); 
        trophy5Shadow.SetActive(false); 
}

    public void changeUserMenuState()
    {
        if (isSelectionMenuOpen) //it's open so hide it
        {
            userTestPanel.localPosition = menuClosedLocation;
            isSelectionMenuOpen = false;
        }
        else // it's closed so open it
        {
            userTestPanel.localPosition = menuOpenLocation;
            isSelectionMenuOpen = true;
        }
    }

    public void startTimer()
    {
        timer = 0;
        isTimerOn = true;

        //hide start button
        startTestButton.SetActive(false);
    }

    public void stopTimer()
    {
        isTimerOn = false;
        
        timeDisplay.text = "Time: " + timer;

        successfulTestDisplay.text = successMessage;
        
        //shift particle system up 3 meters so it looks like it's showering the AR scene
        starParticleSystem.transform.position = successPad.transform.position + (successPad.transform.up * 3f); 
        successPad = null;
    }

}//END OF USERTESTMANAGER CLASS
