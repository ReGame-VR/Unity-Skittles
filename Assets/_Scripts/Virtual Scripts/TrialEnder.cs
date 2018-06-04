using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialEnder : MonoBehaviour {

    // What is this trial ender? Is this the pole (0)? Or is it obstacle 3 (3)?
    private int IDNumber = 0;

    // Set up the trial ender with an IDNumber
    public void AddIDNumber(int IDNumber)
    {
        this.IDNumber = IDNumber;
    }

    // What is this trial ender? Pole or obstacle?
    public int GetIDNumber()
    {
        return IDNumber;
    }
}
