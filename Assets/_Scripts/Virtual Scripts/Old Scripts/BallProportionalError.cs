using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProportionalError : MonoBehaviour {

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

    // If the position/velocity is different by more than this much,
    // then it will count as a poor, inaccurate, and invalid throw.
    private float positionErrorThreshold = 1f;
    private float velocityErrorThreshold = 5f;

    // These are the acceptable windows of position and velocity.
    // If a throw is within these windows, it counts as a full success.
    private float positionSuccessThreshold = 0.25f;
    private float velocitySuccessThreshold = 2f;

    // The rigidbody attached to this ball
    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }


    // Throw the ball! 
    // If the throwing is being limited by position and velocity, do so here.
    public void ThrowBall()
    {
        if ( true ) //GlobalControl.Instance.limitingManus)
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
