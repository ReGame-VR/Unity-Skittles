using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualBall : MonoBehaviour {

    // The trail renderer behind the ball
    [SerializeField]
    private GameObject trailRenderer;

    [Tooltip("Set this so that throwing a ball feels natural and fluid. Normal ball speed"
        + "(=1) feels too slow and unreal")]
    [SerializeField]
    private float ballSpeed = 1.2f;

    [Tooltip("This is the ideal release position when ball is thrown.")]
    [SerializeField]
    private GameObject correctReleasePosition;

    [Tooltip("This is the velocity vector on release that the left-handed user should aim for.")]
    [SerializeField]
    private Vector3 correctVelocityVectorLeft;

    [Tooltip("This is the velocity vector on release that the right-handed user should aim for.")]
    [SerializeField]
    private Vector3 correctVelocityVectorRight;

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

    // The rigidbody attached to this ball
    private Rigidbody rigidBody;

    // If the position/velocity is different by more than this much,
    // then it will count as a poor, inaccurate, and invalid throw.
    private float positionErrorThreshold = 1f;
    private float velocityErrorThreshold = 5f;

    // These are the acceptable windows of position and velocity.
    // If a throw is within these windows, it counts as a full success.
    private float positionSuccessThreshold = 0.25f;
    private float velocitySuccessThreshold = 2f;


    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

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
            gameScript.GetComponent<SoundEffectPlayer>().PlayFailSound();
        }
        else
        {
            // The ball hit a random collider
        }
    }


    // Get the magnitude of this ball's current velocity
    public float GetBallVelocity()
    {
        return rigidBody.velocity.magnitude;
    }

    // Freeze the position of the ball
    public void FreezePosition()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    // Unfreeze the position of the ball
    public void UnfreezePosition()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
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

    // Hides the rope attached to this ball.
    public void HideAttachedRope()
    {
        ropeToHide.HideRopeMesh();
    }

    // Shows the rope attached to this ball.
    public void ShowAttachedRope()
    {
        ropeToHide.RevealRopeMesh();
    }

    // Throw the ball! 
    // If the throwing is being limited by position and velocity, do so here.
    public void ThrowBall()
    {
        if (GlobalControl.Instance.limitingManus)
        {
            float velocityPercent = GetVelocityThresholdPercentage();
            float positionPercent = GetPositionThresholdPercentage();
            float averagePercent = (velocityPercent + positionPercent) / 2;

            rigidBody.velocity = rigidBody.velocity * ballSpeed;

            if (averagePercent == 1f)
            {
                // keep the ball velocity the same
            }
            else
            {
                // Add a random force to the ball proportional to error
                rigidBody.AddForce(GenerateRandomForce(averagePercent), ForceMode.Impulse);
            }
        }
        else
        {
            rigidBody.velocity = rigidBody.velocity * ballSpeed;
        }
    }

    // How close is the user to throwing at the correct
    // position? 0 = not at all, 1 = exactly on target
    private float GetPositionThresholdPercentage()
    {
        float distance = Vector3.Distance(correctReleasePosition.transform.position, transform.position);

        if (distance > positionErrorThreshold)
        {
            // The position is very off, give a low score
            return 0f;
        }
        else if (distance < positionSuccessThreshold)
        {
            // Yay! The position of release is within the sucess window
            return 1f;
        }
        else
        {
            return Mathf.Abs((distance / positionErrorThreshold) - 1);
        }
    }

    // How close is the user to throwing at the correct
    // velocity vector? 0 = not at all, 1 = exactly on target
    private float GetVelocityThresholdPercentage()
    {
        // Get the correct velocity vector, either left or right
        Vector3 correctVelocity;
        if (GlobalControl.Instance.rightHanded)
        {
            correctVelocity = correctVelocityVectorRight;
        }
        else
        {
            correctVelocity = correctVelocityVectorLeft;
        }

        float distance = Vector3.Distance(correctVelocity, rigidBody.velocity);

        // If the velocity vectors are too different, this method will give a lower score.
        if (distance > velocityErrorThreshold)
        {
            // Velocity vectors are very different
            return 0f;
        }
        else if (distance < velocitySuccessThreshold)
        {
            // Yay! The velocity is within the success window
            return 1f;
        }
        else
        {
            return Mathf.Abs((distance / velocityErrorThreshold) - 1);
        }
    }
    
    // Generates a random vector force with each vector leg min 0 max 3
    private Vector3 GenerateRandomForce(float averagePercent)
    {
        Vector3 result = new Vector3(5, 5, 5);
        while (result.magnitude > 7)
        {
            result = new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5));
            // When the error percentage is low, make this random force powerful
            result = result * (1 - averagePercent);
        }
        return result;
    }
}
