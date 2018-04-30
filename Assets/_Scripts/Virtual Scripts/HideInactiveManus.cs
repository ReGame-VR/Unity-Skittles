using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInactiveManus : MonoBehaviour {

    [SerializeField]
    private GameObject LeftArmModel;

    [SerializeField]
    private GameObject LeftHandModel;

    [SerializeField]
    private GameObject RightArmModel;

    [SerializeField]
    private GameObject RightHandModel;


    void Start () {
		if (GlobalControl.Instance.rightHanded)
        {
            LeftArmModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
            LeftHandModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            RightArmModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
            RightHandModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
	}
}
