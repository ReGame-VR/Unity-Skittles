using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualBall : MonoBehaviour {

    // The trail renderer behind the ball
    [SerializeField]
    private GameObject trailRenderer;

    // The game logic script so the ball knows what's going on
    [SerializeField]
    private VirtualSkittlesGame gameScript;

    // The starting position of the ball. It will be reset here when the trial is reset.
    private Vector3 startingPosition;

    // The position of the ball at the beginning of the frame
    private Vector3 prevPosition;

    // current velocity of the ball
    private Vector3 currVel;

    //  rope in the game that will be hidden upon reset
    [SerializeField]
    private HideRope ropeToHide;

    // Particles that will be spawned at ball location on success or reset
    [SerializeField]
    private GameObject successParticles;
    [SerializeField]
    private GameObject resetParticles;

    void Start()
    {
        startingPosition = transform.position;
        FreezePosition();
    }

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

        if (col.gameObject.tag == "Target" && (gameScript.getCurGameState() == VirtualSkittlesGame.GameState.SWINGING))
        {
            gameScript.TargetHit();
            Instantiate(successParticles, transform.position, Quaternion.identity);
        }
        else if (col.gameObject.tag == "TrialEnder" && ((gameScript.getCurGameState() == VirtualSkittlesGame.GameState.SWINGING)
            || gameScript.getCurGameState() ==  VirtualSkittlesGame.GameState.HIT))
        {
            gameScript.ResetTrialState();
            ResetBall();
        }
        else
        {
            // The ball hit a random collider
        }
    }


    // Get the magnitude of this ball's current velocity
    public float GetBallVelocity()
    {
        return GetComponent<Rigidbody>().velocity.magnitude;
    }

    // Freeze the position of the ball
    public void FreezePosition()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    // Unfreeze the position of the ball
    public void UnfreezePosition()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    // Reset the ball to its starting position and freeze it
    public void ResetBall()
    {
        Instantiate(resetParticles, transform.position, Quaternion.identity);

        ropeToHide.HideRopeMesh();
        transform.position = startingPosition;
        FreezePosition();

        Instantiate(resetParticles, transform.position, Quaternion.identity);
    }
}
