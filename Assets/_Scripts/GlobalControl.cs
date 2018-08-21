using UnityEngine;

/// <summary>
/// Stores calibration data for trial use in a single place.
/// </summary>
public class GlobalControl : MonoBehaviour {

    public int numTrials = 10;

    public enum ExplorationMode {NONE, FORCED, REWARD_BASED}

    public enum BallTargetSize {SMALL, MEDIUM, LARGE}

    // if this is true, the game is the real life version of skittles
    public bool isRealLife = true;

    // if this is true, participant is right handed
    public bool rightHanded = true;

    // participant ID to differentiate data files
    public string participantID;

    // The index that determines which position the virtual target is going to be at
    public int targetPositionIndex;

    // The single instance of this class
    public static GlobalControl Instance;

    // The kind of exploration mode that is happening in the virtual skittles
    public ExplorationMode explorationMode = ExplorationMode.NONE;

    // The size of the ball and target in REAL LIFE skittles
    public BallTargetSize ballTargetSize = BallTargetSize.SMALL;

    /// <summary>
    /// Assign instance to this, or destroy it if Instance already exits and is not this instance.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
