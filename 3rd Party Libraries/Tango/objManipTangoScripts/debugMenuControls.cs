using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debugMenuControls : MonoBehaviour
{
    //boolean to show and hide the debug menus
    bool isDebugMode = false;

    //reference to the control selection menu panel
    public GameObject debugSelectionControlPanel;

    //reference to the object manipulation debug display output
    public GameObject selectionModeDisplay;

    //reference to the camera mode button to switch on and off
    public GameObject cameraModeButton;

    //reference to the meshing objects for the scene
    public GameObject dynamicMesh;
    public GameObject meshButtons;
    public GameObject tangoPointCloudObject;

    //Reference to the debug panel to move out of the way
    public RectTransform debugPanel;
    private bool isSelectionMenuOpen = true;
    private Vector3 menuOpenLocation;
    private Vector3 menuClosedLocation;

    //Reference to the cameraManager (who is also the control manager) to change the selection controls----------------------
    public GameObject selectionControlManager;

    public GameObject userTestPanel;

	// Use this for initialization
	void Start ()
    {
        //by default, switch on the debug mode
        this.changeDebugModeONOFF();

        //calculate the position of the slection menu to hide and show
        menuOpenLocation = debugPanel.localPosition;
        menuClosedLocation = new Vector3(menuOpenLocation.x + 400, menuClosedLocation.y - 50);
        this.changeSelectionMenuState();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void changeSelectionMenuState()
    {
        if (isSelectionMenuOpen == false)
        {
            //shift the menu into the scene to show it
            debugPanel.localPosition = menuOpenLocation;
            isSelectionMenuOpen = true;
        }
        else
        {
            //shift the menu out of the scene to hide it
            debugPanel.localPosition = menuClosedLocation;
            isSelectionMenuOpen = false;
        } 
    }

    public void changeDebugModeONOFF()
    {
        if (isDebugMode) //currently debug mode is on, so switch it off
        {
            //deaactivate the control panel with buttons
            debugSelectionControlPanel.SetActive(false);
            selectionModeDisplay.SetActive(false);
            isDebugMode = false;
            cameraModeButton.SetActive(false);
            dynamicMesh.GetComponent<MeshRenderer>().enabled = false;

            //turn off the dynamic UI
            dynamicMesh.GetComponent<TangoDynamicMesh>().m_enableDebugUI = false;
            meshButtons.SetActive(false);
            tangoPointCloudObject.SetActive(false);

            //turn off the user test panel
            userTestPanel.SetActive(false);

}
        else //currently not debug mode, switch it on
        {
            //ctivate the selection control panel
            debugSelectionControlPanel.SetActive(true);
            selectionModeDisplay.SetActive(true);
            isDebugMode = true;
            cameraModeButton.SetActive(true);

            //activate the meshing objects
            dynamicMesh.GetComponent<MeshRenderer>().enabled = true;

            dynamicMesh.GetComponent<TangoDynamicMesh>().m_enableDebugUI = true;
            meshButtons.SetActive(true);
            tangoPointCloudObject.SetActive(true);

            //turn on the user test panel
            userTestPanel.SetActive(true);
        }

    }


    public void createTestCube() //used for testing purposes only with Taran over photon network, the button is still in scene but has been disabled - Keng
    {
        PhotonNetwork.Instantiate("Cube", new Vector3(), new Quaternion(), 0);
    }



}
