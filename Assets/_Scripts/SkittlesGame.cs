using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ManusVR.Scripts;

public class SkittlesGame : MonoBehaviour {

    // The delegate that invokes recording of trial information
    public delegate void ContinuousDataRecording(float time, Vector3 ballPosition);
    public static ContinuousDataRecording OnRecordContinuousData;

    // The state of the game
    // Pretrial - Ball not yet thrown
    // Swinging - Ball was thrown, is currently swinging on its trajectory
    // Posttrial - Ball has completed its trajectory. Feedback is given to user
    public enum GameState {PRE_TRIAL, SWINGING, HIT};

    // The ball object in the skittles game
    [SerializeField]
    private GameObject ball;

    // The target object in the skittles game
    [SerializeField]
    private GameObject target;

    // The target object in the skittles game
    [SerializeField]
    private GameObject wrist;

    // The HandData Script that keeps track of the Manus gloves
    [SerializeField]
    private HandData handData;

    // Reference to the canvas that gives feedback to the player
    [SerializeField]
    private FeedbackCanvas feedbackCanvas;

    [SerializeField]
    [Tooltip("If the ball is at least this distance (meters) away from the wrist, it will count as thrown")]
    private float wristThrownDistance = 0.3f;

    [SerializeField]
    [Tooltip("The open value of a manus glove that determines if it is open or closed. 0 = fully open, 1 = fully closed")]
    private float manusOpenThreshold = 0.3f;

    // The grip that the user had on the ball when they started this trial
    private float prevManusGripValue = 0f;

    // The list of distances from the ball to the target during
    // a trial. Use this to find the minimum distance and display
    // that to the user for feedback. This list gets reset every trial.
    private List<float> distanceList = new List<float>();

    // The current game state
    private GameState curGameState = GameState.PRE_TRIAL;
    
    // Use this for initialization
	void Start () {
        ball.GetComponent<Ball>().DeactivateBallTrail();
        feedbackCanvas.DisplayStartingText();
	}
	
	// Update is called once per frame
	void Update () {

        if (curGameState == GameState.PRE_TRIAL)
        {
            CheckThrown();
        }
        else if (curGameState == GameState.SWINGING)
        {
            // record continuous ball position
            Vector3 ballPos = ball.transform.position;
            OnRecordContinuousData(Time.time, ballPos);

            // Add the current target-ball distance to the distance list
            float distance = Vector3.Distance(ballPos, target.transform.position);
            distanceList.Add(distance);

            CheckReset();

        }
        else // The state is HIT
        {
            CheckReset();
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
        }
        else
        {
            throw new System.Exception("AdvanceToSwingingState() was called while the state was not pre-trial");
        }
    }

    /// <summary>
    /// Advances the game state from swinging to post-trial. Triggered by collision in ball.cs
    /// </summary>
    public void ResetTrialState()
    {
        curGameState = GameState.PRE_TRIAL;
        prevManusGripValue = GetCorrectManusAverage();
        ball.GetComponent<Ball>().DeactivateBallTrail();
        ball.GetComponent<Ball>().ClearBallTrail();

        if (curGameState == GameState.SWINGING)
        {
            // Find the minimum distance in the list and display it
            float minDistance = FindMinimumDistance();
            feedbackCanvas.DisplayDistanceFeedback(minDistance);
        }
        else if (curGameState == GameState.HIT)
        {
            // don't find min distance
        }
        else
        {
            throw new System.Exception("ResetTrialState() was called while the state was not SWINGING or HIT");
        }

        // Reset the distance list
        distanceList = new List<float>();
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

    // Checks if the ball has been thrown
    private void CheckThrown()
    {
        float handAverage = GetCorrectManusAverage();

        // If the ball is far away from the wrist or the grip value is significantly reduced,
        // the ball was thrown
        if (Vector3.Distance(wrist.transform.position, ball.transform.position) > wristThrownDistance
                || handAverage < (prevManusGripValue - 0.25f))
        {
            AdvanceToSwingingState();
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

    // The target was hit! Set the game to HIT state
    public void TargetHit()
    {
        feedbackCanvas.DisplayTargetHitText();
        curGameState = GameState.HIT;
    }

    // Check if the trial should be reset to Pre-Trial
    private void CheckReset()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetTrialState();
        }
    }   
}
