﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds functions for responding to and recording preferences on menu.
/// </summary>
public class MenuController : MonoBehaviour {

    // boolean that keeps track whether a participant ID has been entered at least once. This
    // script disallows moving to next scene if it hasn't
    private bool enteredID = false;

    // activates a text block that displays a warning if moving onto 
    public Text warning;

    // The toggle that tells whether Manus will be limited
    public GameObject limitToggle;

    /// <summary>
    /// Records an alphanumeric participant ID. Hit enter to record. May be entered multiple times
    /// but only last submission is used.
    /// </summary>
    /// <param name="arg0"></param>
    public void RecordID(string arg0)
    {
        GlobalControl.Instance.participantID = arg0;
        enteredID = true;
    }

    /// <summary>
    /// Records an integer indicating how many trials are going to 
    /// appear in the task.
    /// </summary>
    /// <param name="arg0"></param>The entered integer
    public void RecordTrialNum(string arg0)
    {
        int intAnswer = int.Parse(arg0);
        GlobalControl.Instance.numTrials = intAnswer;
    }

    /// <summary>
    /// Sets bool value that determines if participant is right handed
    /// </summary>
    public void SetRightHanded(bool rightHanded)
    {
        GlobalControl.Instance.rightHanded = rightHanded;
    }

    /// <summary>
    /// Sets bool value that determines which version (virtual or real) is run
    /// </summary>
    public void SetRealLife(bool realLife)
    {
        GlobalControl.Instance.isRealLife = realLife;
    }

    /// <summary>
    /// Sets bool value that determines if manus will be limited
    /// </summary>
    public void SetLimitManus(bool limitManus)
    {
        GlobalControl.Instance.limitingManus = limitManus;
    }

    // Show the manus limit toggle when the user is setting up a virtual skittles task
    public void ShowLimitToggle(bool showToggle)
    {
        limitToggle.SetActive(!showToggle);
    }

    // This is a bug fix. When the virtual toggle is chosen, the LimitManus toggle is also set to true.
    public void SetDefaultLimitToggle(bool toggle)
    {
        GlobalControl.Instance.limitingManus = !toggle;
        limitToggle.GetComponent<Toggle>().isOn = !toggle;
    }

    /// <summary>
    /// Loads next scene if wii is connected and participant ID was entered.
    /// </summary>
    public void NextScene()
    {
        if (!enteredID)
        {
            string errorMessage = "One or more errors in calibration:\n";

            if (!enteredID)
            {
                errorMessage += "Participant ID not assigned.\n";
            }

            warning.gameObject.SetActive(true);
            warning.text = errorMessage;
        }
        else
        {
            if (GlobalControl.Instance.isRealLife)
            {
                SceneManager.LoadScene("Real Life Skittles");
            }
            else
            {
                SceneManager.LoadScene("Virtual Skittles");
            }
        }
    }

	/// <summary>
    /// Disable VR for menu scene and hide warning text until needed.
    /// </summary>
	void Start () {
        // disable VR settings for menu scene
        UnityEngine.XR.XRSettings.enabled = false;
        warning.gameObject.SetActive(false);

        // Hide the Manus limit toggle
        limitToggle.SetActive(false);
	}

    /// <summary>
    /// Re-enable VR when this script is disabled (since it is disabled on moving into next scene).
    /// </summary>
    void OnDisable()
    {
        UnityEngine.XR.XRSettings.enabled = true;
    }
}
