﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.ManusVR.Scripts;

public class VirtualSkittlesGame : MonoBehaviour {

    // The delegate that invokes recording of continuous information
    public delegate void ContinuousDataRecording(float time, Vector3 ballPosition);
    public static ContinuousDataRecording OnRecordContinuousData;

    // The delegate that invokes recording of trial information
    public delegate void TrialDataRecording(float time, int curTrial, Vector3 ballPosition, Vector3 wristPosition,
        float errorDistance, float ballVelocity);
    public static TrialDataRecording OnRecordTrialData;

    // The state of the game
    // Pretrial - Ball not yet thrown
    // Swinging - Ball was thrown, is currently swinging on its trajectory
    // Hit - Ball was swinging but then hit the target
    public enum GameState { PRE_TRIAL, SWINGING, HIT };

    // The ball object in the skittles game
    [SerializeField]
    private GameObject ball;

    // The target object in the skittles game
    [SerializeField]
    private GameObject target;

    // The target object in the skittles game
    [SerializeField]
    private GameObject wrist;

    // Reference to the canvas that gives feedback to the player
    [SerializeField]
    private FeedbackCanvas feedbackCanvas;

    [SerializeField]
    private HandData handData;

    // The total number of trials in this game
    private int numTrials = GlobalControl.Instance.numTrials;

    // The current trial that the game is on
    private int curTrial = 1;

    // The list of distances from the ball to the target during
    // a trial. Use this to find the minimum distance and display
    // that to the user for feedback. This list gets reset every trial.
    private List<float> distanceList = new List<float>();

    // The current game state
    private GameState curGameState = GameState.PRE_TRIAL;

    // Stored positions of the ball and wrist when thrown. Reset every trial
    private Vector3 ballPosition;
    private Vector3 wristPosition;

    // Stored velocity of ball when thrown. Reset every trial
    private float ballVelocity;

    // Use this for initialization
    void Start()
    {
        ball.GetComponent<Ball>().DeactivateBallTrail();
        feedbackCanvas.DisplayStartingText();
    }

    // Update is called once per frame
    void Update()
    {

        if (curTrial > numTrials)
        {
            GameOver();
            return;
        }

        if (curGameState == GameState.PRE_TRIAL)
        {

        }
        else if (curGameState == GameState.SWINGING)
        {
            // record continuous ball position
            Vector3 ballPos = ball.transform.position;
            OnRecordContinuousData(Time.time, ballPos);

            // Add the current target-ball distance to the distance list
            float distance = Vector3.Distance(ballPos, target.transform.position);
            distanceList.Add(distance);

        }
        else // The state is HIT
        {
            // wait for trial to be reset
        }
    }

    /// <summary>
    /// Advances the game state from pre-trial to swinging.
    /// </summary>
    public void AdvanceToSwingingState()
    {
        // This method should only run if the state is currently pretrial
        if (curGameState == GameState.PRE_TRIAL)
        {
            curGameState = GameState.SWINGING;
            feedbackCanvas.DisplaySwingingText();
            ball.GetComponent<Ball>().ActivateBallTrail();

            ballVelocity = ball.GetComponent<VirtualBall>().GetBallVelocity();
            ballPosition = ball.transform.position;
            wristPosition = wrist.transform.position;
        }
        else
        {
            throw new System.Exception("AdvanceToSwingingState() was called while the state was not pre-trial");
        }
    }

    /// <summary>
    /// Resets the trial. This should only happen when the user is holding the ball and ready to throw
    /// </summary>
    public void ResetTrialState()
    {
        ball.GetComponent<Ball>().ClearBallTrail();

        if (curGameState == GameState.SWINGING)
        {
            // Find the minimum distance in the list and display it
            float minDistance = FindMinimumDistance();
            feedbackCanvas.DisplayDistanceFeedback(minDistance);

            OnRecordTrialData(Time.time, curTrial, ballPosition, wristPosition,
                minDistance, ballVelocity);
        }
        else if (curGameState == GameState.HIT)
        {
            feedbackCanvas.DisplayStartingText();

            OnRecordTrialData(Time.time, curTrial, ballPosition, wristPosition,
                0f, ballVelocity);
        }
        else
        {
            throw new System.Exception("ResetTrialState() was called while the state was not SWINGING or HIT");
        }

        // Reset the trial parameters
        distanceList = new List<float>();
        curGameState = GameState.PRE_TRIAL;
        curTrial++;
    }

    /// <summary>
    /// Return the current game state. We want to keep the gamestate variable relatively private
    /// aside from this method.
    /// </summary>
    /// <returns></returns>
    public GameState getCurGameState()
    {
        return curGameState;
    }

    // Finds the minimum distance between target and ball during a trial
    private float FindMinimumDistance()
    {
        if (distanceList.Count < 1)
        {
            throw new System.Exception("FindMinimumDistance() was called on an empty distanceList");
        }
        else
        {
            float minSoFar = distanceList[0];
            foreach (float f in distanceList)
            {
                if (f < minSoFar)
                {
                    minSoFar = f;
                }
            }
            return minSoFar;
        }
    }

    // Gets the correct manus average (either left or right hand)
    private float GetCorrectManusAverage()
    {
        if (GlobalControl.Instance.rightHanded)
        {
            return (float)handData.Average(device_type_t.GLOVE_RIGHT);
        }
        else
        {
            return (float)handData.Average(device_type_t.GLOVE_LEFT);
        }
    }

    // The target was hit! Set the game to HIT state. Triggered by collision in Ball.cs
    public void TargetHit()
    {
        feedbackCanvas.DisplayTargetHitText();
        curGameState = GameState.HIT;
    }

    // Game is over. Wait to restart scene.
    private void GameOver()
    {
        feedbackCanvas.DisplayGameOverText();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Menu");
        }
    }

}