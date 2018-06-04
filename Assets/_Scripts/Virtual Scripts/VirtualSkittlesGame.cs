using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.ManusVR.Scripts;

public class VirtualSkittlesGame : MonoBehaviour {

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
    // Hit - Ball was swinging but then hit the target
    // Game over - game is over
    public enum GameState { PRE_GAME, PRE_TRIAL, SWINGING, HIT, GAME_OVER };

    // The ball object in the skittles game
    [SerializeField]
    private GameObject ball;

    // The target object in the skittles game
    [SerializeField]
    private GameObject target;

    // The left and right hand trackers
    [SerializeField]
    private GameObject leftHand;
    [SerializeField]
    private GameObject rightHand;
    private GameObject activeHand;

    // The arm tracker
    [SerializeField]
    private GameObject arm;

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
    private GameState curGameState = GameState.PRE_GAME;

    // Stored positions of the ball and wrist when thrown. Reset every trial
    private Vector3 ballPosition;
    private Vector3 wristPosition;

    // Stored velocity of ball when thrown. Reset every trial
    private Vector3 ballVelocity;

    // Angle between rope and pole upon throw. Reset every trial
    private float ropePoleAngle;

    // The score in this game
    private float score = 0f;

    // The bonus score in this game
    private float bonusScore = 0f;

    // Top of the pole in the game
    [SerializeField]
    private GameObject poleTop;

    // Use this for initialization
    void Start()
    {
        feedbackCanvas.DisplayStartingText();
        if (GlobalControl.Instance.rightHanded)
        {
            activeHand = rightHand;
        }
        else
        {
            activeHand = leftHand;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (curGameState == GameState.GAME_OVER)
        {
            return;
        }
        else if (curGameState == GameState.PRE_GAME)
        {
            PreGame();
            return;
        }

        if (curTrial > numTrials)
        {
            // Game just ended
            feedbackCanvas.DisplayGameOverText();
            curGameState = GameState.GAME_OVER;
            GetComponent<GameOverParticleSpawner>().SpawnGameOverParticles();
            return;
        }

        // Game is in progress.
        // Record IK data once per frame
        GetComponent<IKRecording>().AddJointData(curTrial);

        if (curGameState == GameState.PRE_TRIAL)
        {
            // Wait for ball to be thrown
        }
        else if (curGameState == GameState.SWINGING)
        {
            // record continuous ball position
            Vector3 ballPos = ball.transform.position;
            Vector3 wristPos = activeHand.transform.position;
            Vector3 armPos = arm.transform.position;
            OnRecordContinuousData(Time.time, curTrial, ballPos, wristPos, armPos);

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
            ball.GetComponent<VirtualBall>().ThrowBall();

            ballVelocity = ball.GetComponent<VirtualBall>().GetBallVelocity();
            ballPosition = ball.transform.position;
            wristPosition = activeHand.transform.position;

            ropePoleAngle = FindRopePoleAngle();
        }
        else
        {
            throw new System.Exception("AdvanceToSwingingState() was called while the state was not pre-trial");
        }
    }

    /// <summary>
    /// Resets the trial.
    /// </summary>
    /// IDNumber : the collider that caused this trial to reset. 0 = pole, 1 = obstacle 1, etc.
    public void ResetTrialState(int IDNumber)
    {

        if (curGameState == GameState.SWINGING)
        {
            // Find the minimum distance in the list and display it
            float minDistance = FindMinimumDistance();
            feedbackCanvas.DisplayDistanceFeedback(minDistance);

            // Record trial data considering a miss
            OnRecordTrialData(Time.time, curTrial, ballPosition, wristPosition,
                minDistance, ballVelocity.magnitude, poleTop.transform.position, ropePoleAngle, score, IDNumber);

            // If limiting is turned on, note that this trial was a miss
            ball.GetComponent<BallExplorationMode>().UpdateLimiter(wristPosition, ballVelocity, false);
        }
        else if (curGameState == GameState.HIT)
        {
            feedbackCanvas.DisplayTargetHitText();

            // Record data considering a hit
            OnRecordTrialData(Time.time, curTrial, ballPosition, wristPosition,
                0f, ballVelocity.magnitude, poleTop.transform.position, ropePoleAngle, score, IDNumber);

            // If limiting is turned on, note that this trial was a hit
            ball.GetComponent<BallExplorationMode>().UpdateLimiter(wristPosition, ballVelocity, true);
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
        GetComponent<SoundEffectPlayer>().PlaySuccessSound();
        curGameState = GameState.HIT;
        score = score + 10f;
        feedbackCanvas.UpdateScoreText(score);
    }

    public void AwardBonusPoints()
    {
        GetComponent<SoundEffectPlayer>().PlaySuccessSound();
        bonusScore = bonusScore + 5f;
        feedbackCanvas.UpdateBonusScoreText(bonusScore);
    }

    // Finds the angle in degrees between the rope and pole
    private float FindRopePoleAngle()
    {
        // Find the length of the hypotenuse and the side adjacent to angle
        float hyp = Vector3.Distance(ball.transform.position, poleTop.transform.position);
        float adj = poleTop.transform.position.y - ball.transform.position.y;

        // The angle is equal to the ArcCos of (adj / hyp). Convert to degrees.
        return Mathf.Rad2Deg * Mathf.Acos(adj / hyp);
    }

    // What happens before the game begins. Must press C to calibrate IK model before the game can begin.
    private void PreGame()
    {
        feedbackCanvas.DisplayCalibrateBodyText();

        if (Input.GetKeyUp(KeyCode.C))
        {
            //start game
            curGameState = GameState.PRE_TRIAL;
            feedbackCanvas.DisplayStartingText();
        }
    }

    // If another part of the game wants to know the current trial, use this
    public int GetCurTrial()
    {
        return curTrial;
    }
}