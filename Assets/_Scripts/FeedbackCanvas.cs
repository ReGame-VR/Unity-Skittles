using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A canvas that displays feedback to a Skittles player
public class FeedbackCanvas : MonoBehaviour {

    // The main text displayed on this canvas
    [SerializeField]
    Text mainText;

    // The text to display the user's score
    [SerializeField]
    Text scoreText;

    // The text to encourage the user to explore
    [SerializeField]
    Text explorationText;

    // The text that displays bonus score for the user in the reward-based game
    [SerializeField]
    Text bonusScoreText;

    // The text that prefaces the bonus score
    [SerializeField]
    Text bonusScoreTitleText;

    public void DisplayDistanceFeedback(float minDistance)
    {
        // convert m to cm
        mainText.text = "Missed by " + Mathf.Round(minDistance * 100) + "cm";
    }

    public void UpdateScoreText(float score)
    {
        scoreText.text = score.ToString();
    }

    public void DisplaySwingingText()
    {
        mainText.text = "Ball was thrown!";
    }

    public void DisplayStartingText()
    {
        mainText.text = "Throw ball when ready!";
    }

    public void DisplayTargetHitText()
    {
        mainText.text = "Target Was Hit!";
    }

    public void DisplayGameOverText()
    {
        mainText.text = "Game Over";
    }

    public void DisplayCalibratePoleText()
    {
        mainText.text = "Calibrate Pole & Hold Ball";
    }

    public void DisplayCalibrateBodyText()
    {
        mainText.text = "Calibrate IK Body";
    }

    public void ThrowDifferentWayBonusPoints()
    {
        explorationText.text = "Throw a different way to earn bonus points!";
    }

    public void YouThrewInNewWay()
    {
        explorationText.text = "You threw in a new way!";
    }

    public void GoodJobThrowNewWay()
    {
        explorationText.text = "Good job! Now you need to throw in a different way.";
    }

    public void UpdateBonusScoreText(float bonusScore)
    {
        bonusScoreTitleText.text = "Bonus Score:";
        bonusScoreText.text = bonusScore.ToString();
    }
}
