using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCorrectArm : MonoBehaviour {

    [SerializeField]
    private GameObject leftArm;

    [SerializeField]
    private GameObject rightArm;

    // The arm being tracked for this game
    private GameObject correctArm;

    void Start ()
    {
        if (GlobalControl.Instance.rightHanded)
        {
            correctArm = rightArm;
        }
        else
        {
            correctArm = leftArm;
        }
    }
	
	// Update is called once per frame
	void Update () {

        // Stay in the same position as the correct arm
        transform.position = correctArm.transform.position;
	}
}
