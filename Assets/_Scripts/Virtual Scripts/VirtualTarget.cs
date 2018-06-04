using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualTarget : MonoBehaviour {

    public GameObject[] targetPositions;

	// Use this for initialization
	void Start () {
        // Put the target at the indicated location
        transform.position = targetPositions[GlobalControl.Instance.targetPositionIndex].transform.position;
	}
}
