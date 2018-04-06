using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkittlesGame : MonoBehaviour {

    // The delegate that invokes recording of trial information
    public delegate void ContinuousDataRecording(float time, Vector3 ballPosition);
    public static ContinuousDataRecording OnRecordContinuousData;

    // The state of the game
    // Pretrial - Ball not yet thrown
    // Swinging - Ball was thrown, is currently swinging on its trajectory
    // Posttrial - Ball has completed its trajectory. Feedback is given to user
    public enum GameState {PRE_TRIAL, SWINGING, POST_TRIAL};

    // The ball object in the skittles game
    [SerializeField]
    private GameObject ball;

    // The target object in the skittles game
    [SerializeField]
    private GameObject target;

    // Reference to the canvas that gives feedback to the player
    [SerializeField]
    private FeedbackCanvas feedbackCanvas;

    // The list of distances from the ball to the target during
    // a trial. Use this to find the minimum distance and display
    // that to the user for feedback. This list gets reset every trial.
    private List<float> distanceList = new List<float>();

    // The current game state
    private GameState curGameState = GameState.PRE_TRIAL;
    
    // Use this for initialization
	void Start () {
        ball.GetComponent<Ball>().DeactivateBallTrail();
	}
	
	// Update is called once per frame
	void Update () {

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
        else // Trial completed
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetTrial();
            }
        }	
	}

    /// <summary>
    /// Advances the game state from pre-trial to swinging. Triggered by collision in ball.cs
    /// </summary>
    public void AdvanceToSwingingState()
    {
        // This method should only run if the state is currently pretrial
        if (curGameState == GameState.PRE_TRIAL)
        {
            curGameState = GameState.SWINGING;
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
    public void AdvanceToPostTrialState()
    {
        // This code should only run if the state is currently swinging
        if (curGameState == GameState.SWINGING)
        {
            curGameState = GameState.POST_TRIAL;
            
            // Find the minimum distance in the list, display it, and then reset list
            float minDistance = FindMinimumDistance();
            feedbackCanvas.DisplayDistanceFeedback(minDistance);
            distanceList = new List<float>();
        }
        else
        {
            throw new System.Exception("AdvanceToPostTrialState() was called while the state was not swinging");
        }
    }

    /// <summary>
    /// Reset the post trial state to the pre trial state
    /// </summary>
    private void ResetTrial()
    {
        ball.GetComponent<Ball>().ClearBallTrail();
        ball.GetComponent<Ball>().DeactivateBallTrail();
        curGameState = GameState.PRE_TRIAL;
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

    /// <summary>
    /// Finds the minimum distance between target and ball during a trial
    /// </summary>
    /// <returns></returns>
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
    
}
