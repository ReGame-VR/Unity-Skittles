using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a class that goes on the REAL LIFE ball object. It is used to encourage 
// the user to try different strategies when throwing this ball.
public class RealLifeExplorationMode : MonoBehaviour {

    [SerializeField]
    private AudioSource notifySound;

    // The game script for this real life skittles game
    private SkittlesGame gameScript;

    // The current trajectory of this ball. It will be saved when a throw ends.
    private List<Vector3> currentTrajectory = new List<Vector3>();

    // A class representing a throw
    class Throw
    {
        // Was the throw a successful hit?
        public readonly bool hit;
        // The trajectory of the throw
        public readonly List<Vector3> trajectory;

        public Throw(bool hit, List<Vector3> trajectory)
        {
            this.hit = hit;
            this.trajectory = trajectory;
        }
    }

    // The list of throws that helps the limiter determine
    // when to make a new obstacle / blocked position/velocity
    private List<Throw> throwList = new List<Throw>();

    // The following fraction denotes success, and therefore the limiter should take action:
    // (Num hits / num recent trials) = (successNumerator / sucessDenominator)
    // The numerator of the success ratio. 
    private int successNumerator = 3;
    // The denominator of the success ratio
    private int successDenominator = 5;

    void Update()
    {
        // If the ball is currently being thrown, record its trajectory
        if (gameScript.getCurGameState() == SkittlesGame.GameState.SWINGING)
        {
            currentTrajectory.Add(transform.position);
        }
    }

    // This is called to keep track of the user's throws
    // and successes. If the user has been relatively successful,
    // we want to encourage them to try something new.
    public void UpdateLimiter(bool hit)
    {
        throwList.Add(new Throw(hit, currentTrajectory));
        currentTrajectory = new List<Vector3>();

        // Keep the throwlist at the correct length. Drop the oldest item
        if (throwList.Count > successDenominator)
        {
            throwList.RemoveAt(0);
        }
        if (throwList.Count == successDenominator)
        {
            // Did the user get a high enough success rate?
            int hitsCounted = CountHitsInThrowList();
            if (hitsCounted >= successNumerator)
            {
                EnactExplorationEffect();
                throwList = new List<Throw>();
            }
        }
    }

    // The user has been successful, so it's time to encourage them to try something new
    private void EnactExplorationEffect()
    {
        if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.FORCED)
        {

            // Spawn a new obstacle at a place that would intersect with the user's successful throws
            List<Vector3> potentialObstaclePositions = new List<Vector3>();
            foreach (Throw t in throwList)
            {
                if (t.hit)
                {
                    potentialObstaclePositions.Add(FindObstaclePosition(t.trajectory));
                }
            }
            Vector3 spawnPos = AverageVector3(potentialObstaclePositions);

            // TODO. Tell examiner to place obstacle at position
            notifySound.PlayOneShot(notifySound.clip);

            GetComponent<RealLifeExplorationRecording>().AddForcedData(Time.time, spawnPos, gameScript.getCurTrial());
        }
        else
        {
            // There is no exploration mode, but note that the user has achieved stability
            GetComponent<RealLifeExplorationRecording>().AddStabilityData(Time.time, gameScript.getCurTrial());
        }
    }

    private Vector3 FindObstaclePosition(List<Vector3> trajectory)
    {
        Vector3 poleTop = gameScript.getPoleTopPosition();

        foreach (Vector3 v in trajectory)
        {
            // If the position is in front of the pole
            if (v.z > poleTop.z)
            {
                return v;
            }
        }
        // The code should really never return this value.
        Debug.LogError("Couldnt find appropriate obstacle location");
        return poleTop;
    }

    // Count the number of hits in the throw list
    private int CountHitsInThrowList()
    {
        int result = 0;
        foreach (Throw t in throwList)
        {
            if (t.hit)
            {
                result++;
            }
        }
        return result;
    }

    // Average a list of Vector3
    private Vector3 AverageVector3(List<Vector3> list)
    {
        Vector3 result = new Vector3(0, 0, 0);
        foreach (Vector3 v in list)
        {
            result = result + v;
        }
        return result / list.Count;
    }
    
}
