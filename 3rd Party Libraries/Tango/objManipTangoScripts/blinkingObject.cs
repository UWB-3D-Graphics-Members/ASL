using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blinkingObject : MonoBehaviour
{
    Color blinkingColor = new Color(251,255,0,255); //yellow
    Color original;

    float timer = 0;
    float timeTillBlinkInSeconds = 0.5f;
    bool isBlinking = false;

    // Use this for initialization
    void Start()
    {
        original = this.GetComponent<MeshRenderer>().material.color;
        timer = timeTillBlinkInSeconds;
    }

	// Update is called once per frame
	void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (isBlinking) //change it back to original color
            {
                this.GetComponent<MeshRenderer>().material.color = original;
            }
            else //change it to blinking color
            {
                this.GetComponent<MeshRenderer>().material.color = blinkingColor;
            }
            //flip the isblinking variable to switch coler next round
            isBlinking = !isBlinking;

            //reset timer
            timer = timeTillBlinkInSeconds;
        }
	}
}
