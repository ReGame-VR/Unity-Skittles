using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAndTargetSize : MonoBehaviour {

    // The ball and target objects whose sizes will change
    [SerializeField]
    private GameObject ballObject;
    [SerializeField]
    private GameObject targetObject;

    // Use this for initialization
    void Start () {
		if (GlobalControl.Instance.ballTargetSize == GlobalControl.BallTargetSize.SMALL)
        {
            ballObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            targetObject.transform.localScale = new Vector3(0.01f, 0.15f, 0.01f);
        }
        else if (GlobalControl.Instance.ballTargetSize == GlobalControl.BallTargetSize.MEDIUM)
        {
            ballObject.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            targetObject.transform.localScale = new Vector3(0.015f, 0.15f, 0.015f);
        }
        else
        {
            ballObject.transform.localScale = new Vector3(0.055f, 0.055f, 0.055f);
            targetObject.transform.localScale = new Vector3(0.025f, 0.15f, 0.025f);
        }
	}
}
