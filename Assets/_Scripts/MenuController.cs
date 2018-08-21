using System.Collections;
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
    public GameObject explorationDropdown;

    // The toggle that changes the ball and target size of the REAL LIFE skittles
    public GameObject ballTargetSizeDropdown;

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
    public void SetRightHanded(int rightHanded)
    {
        if (rightHanded == 0)
        {
            GlobalControl.Instance.rightHanded = true;
        }
        else
        {
            GlobalControl.Instance.rightHanded = false;
        }
    }

    /// <summary>
    /// Sets ball target size for real life skittles
    /// </summary>
    public void SetBallTargetSize(int arg0)
    {
        if (arg0 == 0)
        {
            GlobalControl.Instance.ballTargetSize = GlobalControl.BallTargetSize.SMALL;
        }
        else if (arg0 == 1)
        {
            GlobalControl.Instance.ballTargetSize = GlobalControl.BallTargetSize.MEDIUM;
        }
        else
        {
            GlobalControl.Instance.ballTargetSize = GlobalControl.BallTargetSize.LARGE;
        }
    }

    /// <summary>
    /// Sets bool value that determines which version (virtual or real) is run
    /// </summary>
    public void SetRealLife(int realLife)
    {
        if (realLife == 0)
        {
            GlobalControl.Instance.isRealLife = true;
            ballTargetSizeDropdown.SetActive(true);
        }
        else
        {
            GlobalControl.Instance.isRealLife = false;
            ballTargetSizeDropdown.SetActive(false);
        }
    }

    /// <summary>
    /// Sets bool value that determines if manus will be limited
    /// </summary>
    public void SetExplorationMode(int explorationMode)
    {
        if (explorationMode == 0)
        {
            GlobalControl.Instance.explorationMode = GlobalControl.ExplorationMode.NONE;
        }
        else if (explorationMode == 1)
        {
            GlobalControl.Instance.explorationMode = GlobalControl.ExplorationMode.FORCED;
        }
        else
        {
            GlobalControl.Instance.explorationMode = GlobalControl.ExplorationMode.REWARD_BASED;
        }
    }

    /// <summary>
    /// Sets bool value that determines which version (virtual or real) is run
    /// </summary>
    public void SetTargetPosition(int positionIndex)
    {
        GlobalControl.Instance.targetPositionIndex = positionIndex;
    }

    // Show the exploration mode when the user is setting up a virtual skittles task
    public void ShowExplorationMode(int realLife)
    {
        if (realLife == 0)
        {
            explorationDropdown.SetActive(false);
        }
        else
        {
            explorationDropdown.SetActive(true);
        }
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

        // Set up the scene with default values
        explorationDropdown.SetActive(false);
        GlobalControl.Instance.explorationMode = GlobalControl.ExplorationMode.NONE;
        GlobalControl.Instance.isRealLife = true;
        GlobalControl.Instance.rightHanded = true;
        GlobalControl.Instance.targetPositionIndex = 0;

        // By default, real life skittles is default and so is small ball target size
        ballTargetSizeDropdown.SetActive(true);
        GlobalControl.Instance.ballTargetSize = GlobalControl.BallTargetSize.SMALL;
    }

    /// <summary>
    /// Re-enable VR when this script is disabled (since it is disabled on moving into next scene).
    /// </summary>
    void OnDisable()
    {
        UnityEngine.XR.XRSettings.enabled = true;
    }
}
