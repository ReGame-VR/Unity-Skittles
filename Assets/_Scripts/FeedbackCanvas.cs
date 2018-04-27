using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A canvas that displays feedback to a Skittles player
public class FeedbackCanvas : MonoBehaviour {

    // The main text displayed on this canvas
    [SerializeField]
    Text mainText;

    public void DisplayDistanceFeedback(float minDistance)
    {
        // convert m to cm
        mainText.text = "Missed by " + Mathf.Round(minDistance * 100) + "cm";
    }

    public void DisplaySwingingText()
    {
        mainText.text = "Ball was thrown!";
    }

    public void DisplayStartingText()
    {
        mainText.text = "Throw ball when ready!";
    }

    public void DisplayTargetHitText()
    {
        mainText.text = "Target Was Hit!";
    }

    public void DisplayGameOverText()
    {
        mainText.text = "Game Over";
    }

    public void DisplayCalibratePoleText()
    {
        mainText.text = "Calibrate Pole & Hold Ball";
    }
}
