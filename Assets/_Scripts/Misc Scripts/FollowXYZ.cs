using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a simple script to make one object follow the XYZ
/// of another object WITHOUT rotating.
/// </summary>
public class FollowXYZ : MonoBehaviour {

    // The object that this object will follow
    [SerializeField]
    private GameObject mainObject;
	
	// Update is called once per frame
	void Update () {

        // This object gets the XYZ position of the main object
        // (without rotating)
        Vector3 mainPos = mainObject.transform.position;
        transform.position = new Vector3(mainPos.x, mainPos.y, mainPos.z);
		
	}
}
