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

    // The list of blocked positions
    private List<Vector3> blockedPositions = new List<Vector3>();

    // The radius of each blocked area surrounding a blocked point.
    private float blockedAreaRadius = 0.30f;

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

    // A class representing a throw
    class Throw {
        // Where was the hand during throw?
        public readonly Vector3 handPos;
        // Was the throw a successful hit?
        public readonly bool hit;

        public Throw(Vector3 handPos, bool hit)
        {
            this.handPos = handPos;
            this.hit = hit;
        }
    }

    // The list of throws that helps the limiter determine
    // when to make a new blocked position
    private List<Throw> throwList = new List<Throw>();

    // The following fraction denotes success, and therefore the limiter should take action:
    // The numerator of the success ratio. 
    private int successNumerator = 3;
    // The denominator of the success ratio
    private int successDenominator = 5;

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
            // If the hand is in a blocked area, turn it red and disable releasing
            if (HandWithinBlockedArea())
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

    // Returns true if the hand is within one of the blocked areas.
    private bool HandWithinBlockedArea()
    {
        Vector3 handPos = GetCorrectHand().transform.position;

        foreach (Vector3 blockedPos in blockedPositions)
        {
            if (Vector3.Distance(handPos, blockedPos) < blockedAreaRadius)
            {
                return true;
            }
        }
        return false;
    }

    // This is called to keep track of the user's throws
    // and successes. If the user has been relatively successful,
    // this will create a new blocked position.
    public void UpdateLimiter(Vector3 handPos, bool hit)
    {
        throwList.Add(new Throw(handPos, hit));

        if (throwList.Count > successDenominator)
        {
            throwList.RemoveAt(0);
        }
        if (throwList.Count == successDenominator)
        {
            int hitsCounted = CountHitsInThrowList();
            if (hitsCounted >= successNumerator)
            {
                Vector3 avgVector = AverageVectorInThrowList(hitsCounted);
                blockedPositions.Add(avgVector);
            }
        }
    }

    // Count the number of hits in the throw list
    private int CountHitsInThrowList()
    {
        int result = 0;
        foreach (Throw t in throwList)
        {
            if (t.hit)
            {
                result++;
            }
        }
        return result;
    }

    // Average all the successful vectors in the throw list
    // hitsCounted: the number of successful vectors in the throw list
    private Vector3 AverageVectorInThrowList(int hitsCounted)
    {
        Vector3 result = new Vector3(0,0,0);
        foreach (Throw t in throwList)
        {
            if (t.hit)
            {
                result = result + t.handPos;
            }
        }
        return result / hitsCounted;
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
