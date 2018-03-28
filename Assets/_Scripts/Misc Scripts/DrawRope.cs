using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawRope : MonoBehaviour {

    [Tooltip("The game object that the rope is tied to")]
    [SerializeField]
    private GameObject ropeOrigin;

    [Tooltip("The object that is tied to the rope; the end of the rope")]
    [SerializeField]
    private GameObject ropeEnd;

    private LineRenderer lr;

    void Start ()
    {
        lr = GetComponent<LineRenderer>();
    }
	
	// Draw the line between the two points
	void Update () {
        lr.SetPosition(0, ropeOrigin.transform.position);
        lr.SetPosition(1, ropeEnd.transform.position);
	}
}
