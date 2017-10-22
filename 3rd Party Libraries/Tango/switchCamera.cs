using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tango;

public class switchCamera : MonoBehaviour
{
    public GameObject ARCamera;
    public GameObject VRCamera;

    bool isARCameraOn = false;
    bool isVRCameraOn = false;


	// Use this for initialization
	void Start ()
    {
        //start program with AR camera
        ARCamera.gameObject.SetActive(true);
        isARCameraOn = true;

        VRCamera.gameObject.SetActive(false);
        isVRCameraOn = false;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isARCameraOn)
        {
            VRCamera.transform.position = new Vector3(ARCamera.transform.position.x, ARCamera.transform.position.y, ARCamera.transform.position.z);
            VRCamera.transform.rotation = new Quaternion(ARCamera.transform.rotation.x, ARCamera.transform.rotation.y, ARCamera.transform.rotation.z, ARCamera.transform.rotation.w);
        }
        else
        {
            ARCamera.transform.position = new Vector3(VRCamera.transform.position.x, VRCamera.transform.position.y, VRCamera.transform.position.z);
            ARCamera.transform.rotation = new Quaternion(VRCamera.transform.rotation.x, VRCamera.transform.rotation.y, VRCamera.transform.rotation.z, VRCamera.transform.rotation.w);
        }
	}



    public void flipCamera()
    {
        if (isARCameraOn) //turn it off since AR is on
        {
            ARCamera.gameObject.SetActive(false);
            isARCameraOn = false;

            VRCamera.gameObject.SetActive(true);
            isVRCameraOn = true;
        }
        else //the vr camera is on so turn it off
        {
            VRCamera.gameObject.SetActive(false);
            isVRCameraOn = false;

            ARCamera.gameObject.SetActive(true);
            isARCameraOn = true;
        }
    }

}
