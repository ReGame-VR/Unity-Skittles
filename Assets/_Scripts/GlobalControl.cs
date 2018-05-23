﻿using UnityEngine;

/// <summary>
/// Stores calibration data for trial use in a single place.
/// </summary>
public class GlobalControl : MonoBehaviour {

    public int numTrials = 10;

    public enum ExplorationMode {NONE, FORCED, REWARD_BASED}

    // if this is true, the game is the real life version of skittles
    public bool isRealLife = true;

    // if this is true, participant is right handed
    public bool rightHanded = true;

    // participant ID to differentiate data files
    public string participantID;

    // The single instance of this class
    public static GlobalControl Instance;

    // The kind of exploration mode that is happening in the virtual skittles
    public ExplorationMode explorationMode = ExplorationMode.NONE;

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
