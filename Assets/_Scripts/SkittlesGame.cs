using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkittlesGame : MonoBehaviour {

    // The state of the game
    // Pretrial - Ball not yet thrown
    // Swinging - Ball was thrown, is currently swinging on its trajectory
    // Posttrial - Ball has completed its trajectory. Feedback is given to user
	public enum GameState {PRE_TRIAL, SWINGING, POST_TRIAL};

    [SerializeField]
    private Ball ball;

    // The current game state
    private GameState curGameState = GameState.PRE_TRIAL;
    
    // Use this for initialization
	void Start () {
        ball.DeactivateBallTrail();
	}
	
	// Update is called once per frame
	void Update () {
        if (curGameState == GameState.PRE_TRIAL)
        {

        }
        else if (curGameState == GameState.SWINGING)
        {
            // RecordContinuousBallPosition();
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
    /// Advances the game state from pre-trial to swinging
    /// </summary>
    public void AdvanceToSwingingState()
    {
        // This method should only run if the state is currently pretrial
        if (curGameState == GameState.PRE_TRIAL)
        {
            curGameState = GameState.SWINGING;
            ball.ActivateBallTrail();
        }
    }

    /// <summary>
    /// Advances the game state from swinging to post-trial
    /// </summary>
    public void AdvanceToPostTrialState()
    {
        // This code should only run if the state is currently swinging
        if (curGameState == GameState.SWINGING)
        {
            curGameState = GameState.POST_TRIAL;
        }
    }

    /// <summary>
    /// Reset the post trial state to the pre trial state
    /// </summary>
    private void ResetTrial()
    {
        ball.ClearBallTrail();
        ball.DeactivateBallTrail();
        curGameState = GameState.PRE_TRIAL;
    }
    
}
