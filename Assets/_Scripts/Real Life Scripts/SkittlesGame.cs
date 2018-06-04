using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ManusVR.Scripts;
using UnityEngine.SceneManagement;

public class SkittlesGame : MonoBehaviour {

    // The delegate that invokes recording of continuous information
    public delegate void ContinuousDataRecording(float time, int trialNum, Vector3 ballPosition, Vector3 wristPosition, Vector3 armPosition);
    public static ContinuousDataRecording OnRecordContinuousData;

    // The delegate that invokes recording of trial information
    public delegate void TrialDataRecording(float time, int curTrial, Vector3 ballPosition, Vector3 wristPosition,
        float errorDistance, float ballVelocity, Vector3 poleTopPosition, float ropePoleAngle, float score, int IDNumber);
    public static TrialDataRecording OnRecordTrialData;

    // The state of the game
    // Pretrial - Ball not yet thrown
    // Swinging - Ball was thrown, is currently swinging on its trajectory
    // Hit - Ball hit the target. Waiting for the game to be reset
    public enum GameState {PRE_TRIAL, SWINGING, HIT};

    // The ball object in the skittles game
    [SerializeField]
    private GameObject ball;

    // The target object in the skittles game
    [SerializeField]
    private GameObject target;

    // The wrist being tracked
    [SerializeField]
    private GameObject wrist;

    // The arm object in the skittles game
    [SerializeField]
    private GameObject arm;

    // The tracker used to calibrate the pole
    [SerializeField]
    private GameObject poleTracker;

    // The position of the pole top determined during calibration
    private Vector3 poleTopPosition;

    // The HandData Script that keeps track of the Manus gloves
    [SerializeField]
    private HandData handData;

    // Reference to the canvas that gives feedback to the player
    [SerializeField]
    private FeedbackCanvas feedbackCanvas;

    [SerializeField]
    [Tooltip("If the ball is at least this distance (meters) away from the wrist, it will count as thrown")]
    private float wristThrownDistance = 0.3f;

    // The left and right Vive controllers
    [SerializeField]
    private ViveControllerInput leftController;
    [SerializeField]
    private ViveControllerInput rightController;

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

    private bool waitToStart = true;

    // The velocity of the ball on release. Reassigned every trial
    private float ballVelocity;

    // The angle between the rope and pole on ball thrown. Reset every trial
    private float ropePoleAngle;

    // Positions of ball and wrists on release. Reassigned every trial
    private Vector3 ballPosition;
    private Vector3 wristPosition;

    // score in this game
    private float score = 0f;
    
    // Use this for initialization
	void Start () {
        ball.GetComponent<Ball>().DeactivateBallTrail();
        feedbackCanvas.DisplayCalibratePoleText();
	}
	
	// Update is called once per frame
	void Update () {

        // Wait for calibration and button press to begin
        if (waitToStart)
        {
            WaitToStart();
            return;
        }

        if (curTrial > numTrials)
        {
            GameOver();
            return;
        }

        if (curGameState == GameState.PRE_TRIAL)
        {
            CheckThrown();
        }
        else if (curGameState == GameState.SWINGING)
        {
            // record continuous ball position
            Vector3 ballPos = ball.transform.position;
            Vector3 wristPos = wrist.transform.position;
            Vector3 armPos = arm.transform.position;
            OnRecordContinuousData(Time.time, curTrial, ballPos, wristPos, armPos);

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

            // Store data about ball and wrist on release
            ballVelocity = ball.GetComponent<Ball>().GetBallVelocity();
            ballPosition = ball.transform.position;
            wristPosition = wrist.transform.position;

            ropePoleAngle = FindRopePoleAngle();
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
        ball.GetComponent<Ball>().DeactivateBallTrail();
        ball.GetComponent<Ball>().ClearBallTrail();

        if (curGameState == GameState.SWINGING)
        {
            // Find the minimum distance in the list and display it
            float minDistance = FindMinimumDistance();
            feedbackCanvas.DisplayDistanceFeedback(minDistance);

            OnRecordTrialData(Time.time, curTrial, ballPosition, wristPosition,
                minDistance, ballVelocity, poleTopPosition, ropePoleAngle, score, 0);
        }
        else if (curGameState == GameState.HIT)
        {
            feedbackCanvas.DisplayStartingText();

            OnRecordTrialData(Time.time, curTrial, ballPosition, wristPosition,
                0f, ballVelocity, poleTopPosition, ropePoleAngle, score, 0);
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

    // Checks if the ball has been thrown
    private void CheckThrown()
    {

        // If the ball is far away from the wrist
        // the ball was thrown
        if (Vector3.Distance(wrist.transform.position, ball.transform.position) > wristThrownDistance)
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

    // The target was hit! Set the game to HIT state. Triggered by collision in Ball.cs
    public void TargetHit()
    {
        feedbackCanvas.DisplayTargetHitText();
        curGameState = GameState.HIT;
        score = score + 10f;
        GetComponent<SuccessSoundPlayer>().PlaySuccessSound();
    }

    // Check if the trial should be reset to Pre-Trial
    private void CheckReset()
    {
        if (FacilitatorInput())
        {
            ResetTrialState();
        }
    }
    
    // Game is over. Wait to restart scene.
    private void GameOver()
    {
        feedbackCanvas.DisplayGameOverText();

        if (FacilitatorInput())
        {
            SceneManager.LoadScene("Menu");
        }
    }

    // Game has just begun. Wait for button start to calibrate pole, and then begin
    private void WaitToStart()
    {

        if (FacilitatorInput())
        {
            feedbackCanvas.DisplayStartingText();
            poleTopPosition = poleTracker.transform.position;
            waitToStart = false;
        }
    }

    /// <summary>
    /// Determines the buttons that the facilitator will use to start the game and reset
    /// trials.
    /// </summary>
    /// <returns></returns>Returns true if the facilitator pressed a valid button
    private bool FacilitatorInput()
    {
        return Input.GetKeyDown(KeyCode.Space) || leftController.Controller.GetHairTriggerUp()
            || rightController.Controller.GetHairTriggerUp();
    }

    // Finds the angle in degrees between the rope and pole
    private float FindRopePoleAngle()
    {
        // Find the length of the hypotenuse and the side adjacent to angle
        float hyp = Vector3.Distance(ball.transform.position, poleTopPosition);
        float adj = poleTopPosition.y - ball.transform.position.y;

        // The angle is equal to the ArcCos of (adj / hyp). Convert to degrees.
        return Mathf.Rad2Deg * Mathf.Acos(adj / hyp);
    }
}
