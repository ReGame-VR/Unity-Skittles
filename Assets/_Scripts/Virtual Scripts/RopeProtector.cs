using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ManusVR.Scripts.PhysicalInteraction;

// A class that protects a rope from being
// stretched too much
public class RopeProtector : MonoBehaviour {

    // The physics hand. This class will make the hands 
    // drop the ball if the rope is being pulled too tight
    [SerializeField]
    private GameObject leftPhysicsHand;
    [SerializeField]
    private GameObject rightPhysicsHand;

    // The maximum length that the rope is allowed to stretch
    [SerializeField]
    private float maxStretchLength = 1.5f;

    [SerializeField]
    private GameObject poleTop;
    [SerializeField]
    private GameObject ball;

	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(poleTop.transform.position, ball.transform.position) > maxStretchLength)
        {
            // Release ball from grip and reset it.
            leftPhysicsHand.GetComponent<ObjectGrabber>().ForceReleaseItem();
            rightPhysicsHand.GetComponent<ObjectGrabber>().ForceReleaseItem();
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            ball.GetComponent<BonusParticleSpawner>().SpawnBadThrowParticles();
            ball.GetComponent<VirtualBall>().ResetBall();

            // After the ball is reset, make sure the hand does not grab it again
            StartCoroutine(ForceReleaseItemAfterSeconds());
        }
	}

    // Force the hands to release an item after a short delay
    IEnumerator ForceReleaseItemAfterSeconds()
    {
        yield return new WaitForSeconds(0.1f);
        leftPhysicsHand.GetComponent<ObjectGrabber>().ForceReleaseItem();
        rightPhysicsHand.GetComponent<ObjectGrabber>().ForceReleaseItem();
        ball.GetComponent<VirtualBall>().ResetBall();
    }
}
