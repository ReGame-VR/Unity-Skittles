using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private GameObject trailRenderer;

    [SerializeField]
    private SkittlesGame gameScript;

    // The position of the ball at the beginning of the frame
    private Vector3 prevPosition;

    // current velocity of the ball
    private Vector3 currVel;

    void Update()
    {
        // Position at frame start
        prevPosition = transform.position;
        StartCoroutine(CalcVelocity(prevPosition));
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

    public float GetBallVelocity()
    {
        return currVel.magnitude;
    }

    // Handles the collisions with the ball
    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Target" && (gameScript.getCurGameState() == SkittlesGame.GameState.SWINGING))
        {
            gameScript.TargetHit();
        }
        else
        {
            // The ball hit a random collider
        }
    }

    IEnumerator CalcVelocity(Vector3 pos)
    {
        // Wait till it the end of the frame
        // Velocity = DeltaPosition / DeltaTime
        yield return new WaitForEndOfFrame();
        currVel = (pos - transform.position) / Time.deltaTime;
    }
}
