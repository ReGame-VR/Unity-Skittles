using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkittlesGame : MonoBehaviour {

    // The state of the game
    // Pretrial - Ball not yet thrown
    // Swinging - Ball was thrown, is currently swinging on its trajectory
    // Posttrial - Bass has completed its trajectory. Feedback is given to user
	public enum GameState
    {PRE_TRIAL, SWINGING, POST_TRIAL};

    // The current game state
    private GameState curGameState = GameState.PRE_TRIAL;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
