using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeGrabReset : MonoBehaviour {

    // The location where the cube was originally. It will be reset to here.
    private Vector3 origin;

	// Use this for initialization
	void Start () {
        origin = transform.position;
	}

    /// <summary>
    /// Put the cube back where it started
    /// </summary>
    public void ResetToOrigin()
    {
        transform.position = origin;
    }
}
