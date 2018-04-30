using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ManusVR.Scripts;

public class CalibrateManus : MonoBehaviour {
    
    public GameObject handLeft;
    public GameObject handRight;

    void Start()
    {
        StartCoroutine(CalibrateManusQuickly());
    }

    IEnumerator CalibrateManusQuickly()
    {
        yield return new WaitForSeconds(0.1f); ;    //Wait a little bit before calibrating
        handLeft.GetComponent<RegularHand>().UpdateProcedureWithMandatoryCalibration();
        handRight.GetComponent<RegularHand>().UpdateProcedureWithMandatoryCalibration();
    }
}
