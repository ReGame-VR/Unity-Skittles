using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ManusVR.Scripts.PhysicalInteraction;

// A class that serves to limit the capability of the Manus
// gloves. This is used in a task where redundancy is being studied.
public class ManusLimiter : MonoBehaviour {

    // The red material that displays when the hand cannot be released
    [SerializeField]
    private Material redMat;

    // The green material that indicates freedom to release
    [SerializeField]
    private Material greenMat;

    // The constant height limit. If the Manus gloves is above this, it will not be able to release objects.
    [SerializeField]
    private float heightThreshold;

    // Hand and arm game objects. Used to find their position and to change the displaying material
    [SerializeField]
    private GameObject leftHand;
    [SerializeField]
    private GameObject leftArm;
    [SerializeField]
    private GameObject rightHand;
    [SerializeField]
    private GameObject rightArm;

    [SerializeField]
    private GameObject objectGrabberLeft;
    [SerializeField]
    private GameObject objectGrabberRight;

    // assigned at beginning of scene. 
    private GameObject correctHand;
    private GameObject correctArm;
    private GameObject correctObjectGrabber;

    void Awake ()
    {
        correctHand = GetCorrectHand();
        correctArm = GetCorrectArm();
        correctObjectGrabber = GetCorrectObjectGrabber();
    }

    // Use this for initialization
    void Start () {
        correctHand.GetComponent<SkinnedMeshRenderer>().material = redMat;
        correctArm.GetComponent<SkinnedMeshRenderer>().material = redMat;
    }
	
	// Update is called once per frame
	void Update () {

        if (GlobalControl.Instance.limitingManus)
        {
            Debug.Log("limiting");

            // If the hand is above the threshold, turn it red and disable releasing
            if (correctHand.transform.position.y > heightThreshold)
            {
                correctHand.GetComponent<SkinnedMeshRenderer>().material = redMat;
                correctArm.GetComponent<SkinnedMeshRenderer>().material = redMat;
                correctObjectGrabber.GetComponent<ObjectGrabber>().DisableRelease();
            }
            else // otherwise turn it green and allow releasing
            {
                correctHand.GetComponent<SkinnedMeshRenderer>().material = greenMat;
                correctArm.GetComponent<SkinnedMeshRenderer>().material = greenMat;
                correctObjectGrabber.GetComponent<ObjectGrabber>().EnableRelease();
            }
        }		
	}
    
    // Get correct hand, either left or right
    private GameObject GetCorrectHand()
    {
        if (GlobalControl.Instance.rightHanded)
        {
            return rightHand;
        }
        else
        {
            return leftHand;
        }
    }

    // Get correct arm, either left or right
    private GameObject GetCorrectArm()
    {
        if (GlobalControl.Instance.rightHanded)
        {
            return rightArm;
        }
        else
        {
            return leftArm;
        }
    }

    // Get correct object grabber, either left or right
    private GameObject GetCorrectObjectGrabber()
    {
        if (GlobalControl.Instance.rightHanded)
        {
            return objectGrabberRight;
        }
        else
        {
            return objectGrabberLeft;
        }
    }

}
