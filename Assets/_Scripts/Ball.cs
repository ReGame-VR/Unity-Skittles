﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private GameObject trailRenderer;

    [SerializeField]
    private SkittlesGame gameScript;

    // Shows the ball trail
    public void ActivateBallTrail()
    {
        trailRenderer.SetActive(true);
    }

    // Hides the ball trail
    public void DeactivateBallTrail()
    {
        trailRenderer.SetActive(false);
    }

    // Clears the ball trail
    public void ClearBallTrail()
    {
        trailRenderer.GetComponent<TrailRenderer>().Clear();
    }

    // Handles the collisions with the ball
    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Target" && (gameScript.getCurGameState() == SkittlesGame.GameState.SWINGING))
        {
            //gameScript.TargetHit();
        }
        else if (col.gameObject.tag == "TrialStarter" && (gameScript.getCurGameState() == SkittlesGame.GameState.PRE_TRIAL))
        {
            gameScript.AdvanceToSwingingState();
        }
        else if (col.gameObject.tag == "TrialEnder" && (gameScript.getCurGameState() == SkittlesGame.GameState.SWINGING))
        {
            gameScript.AdvanceToPostTrialState();
        }
        else
        {
            // The ball hit a random collider
        }
    }
}