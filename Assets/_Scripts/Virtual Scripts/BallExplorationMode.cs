using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that serves to limit the capability of the Manus
// gloves. This is used in a task where redundancy is being studied.
public class BallExplorationMode : MonoBehaviour
{
    // The list of blocked positions
    private List<Vector3> blockedPositions = new List<Vector3>();

    // The list of blocked velocity vectors
    private List<Vector3> blockedVelocities = new List<Vector3>();

    // The radius of each blocked area surrounding a blocked point.
    private float blockedPositionRadius = 0.15f;

    // The radius of each blocked area surrounding a blocked point.
    private float blockedVelocityRadius = 3f;

    // The obstacle prefab that will be spawned
    [SerializeField]
    private GameObject obstacle;

    // The pole in the game
    [SerializeField]
    private GameObject pole;

    // The game script
    [SerializeField]
    private VirtualSkittlesGame gameScript;

    // The canvas that displays feedback to user
    [SerializeField]
    private FeedbackCanvas feedbackCanvas;

    private List<Vector3> currentTrajectory = new List<Vector3>();

    // A class representing a throw
    class Throw
    {
        // Where was the ball during throw?
        public readonly Vector3 handPos;
        // What was the velocity vector of the ball during throw?
        public readonly Vector3 velocity;
        // Was the throw a successful hit?
        public readonly bool hit;
        // The trajectory of the throw
        public readonly List<Vector3> trajectory;

        public Throw(Vector3 handPos, Vector3 velocity, bool hit, List<Vector3> trajectory)
        {
            this.handPos = handPos;
            this.velocity = velocity;
            this.hit = hit;
            this.trajectory = trajectory;
        }
    }

    // The list of throws that helps the limiter determine
    // when to make a new obstacle / blocked position/velocity
    private List<Throw> throwList = new List<Throw>();

    // The following fraction denotes success, and therefore the limiter should take action:
    // The numerator of the success ratio. 
    private int successNumerator = 3;
    // The denominator of the success ratio
    private int successDenominator = 5;

    void Update()
    {
        // If the ball is currently being thrown, record its trajectory
        if (gameScript.getCurGameState() == VirtualSkittlesGame.GameState.SWINGING)
        {
            currentTrajectory.Add(transform.position);
        }
    }

    // This is called to keep track of the user's throws
    // and successes. If the user has been relatively successful,
    // we want to encourage them to try something new.
    public void UpdateLimiter(Vector3 handPos, Vector3 velocity, bool hit)
    {
        if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.NONE)
        {
            return;
        }
        if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.REWARD_BASED)
        {
            // See if the game should award bonus points for the last throw
            CheckAwardBonusPoints(handPos, velocity);
        }

        throwList.Add(new Throw(handPos, velocity, hit, currentTrajectory));
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
        if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.REWARD_BASED)
        {
            feedbackCanvas.ThrowDifferentWayBonusPoints();

            // Add blocked areas where user gets no bonus points
            int numHits = CountHitsInThrowList();
            blockedPositions.Add(AveragePositionInThrowList(numHits));
            blockedVelocities.Add(AverageVelocityInThrowList(numHits));
        }
        else if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.FORCED)
        {
            feedbackCanvas.GoodJobThrowNewWay();

            // Spawn a new obstacle at a place that would intersect with the user's successful throws
            List<Vector3> potentialObstaclePositions = new List<Vector3>();
            foreach (Throw t in throwList)
            {
                if (t.hit)
                {
                    potentialObstaclePositions.Add(FindObstaclePosition(t.trajectory));
                }
            }
            Instantiate(obstacle, AverageVector3(potentialObstaclePositions), Quaternion.identity);
        }
    }

    /// <summary>
    /// Checks whether or not this reward-based game should give bonus
    /// points to the user for their last throw
    /// </summary>
    /// <param name="handPos"></param>The position of hand at throw
    /// <param name="velocity"></param>The velocity of hand at throw
    private void CheckAwardBonusPoints(Vector3 handPos, Vector3 velocity)
    {
        // The user has to have been successful at least once before
        // bonus poins come into play
        if (blockedPositions.Count > 1)
        {
            if (!Vector3WithinBlockedArea(velocity, blockedVelocities, blockedVelocityRadius))
            {
                gameScript.AwardBonusPoints();
            }
            if (!Vector3WithinBlockedArea(handPos, blockedPositions, blockedPositionRadius))
            {
                gameScript.AwardBonusPoints();
            }
        }
    }

    private Vector3 FindObstaclePosition(List<Vector3> trajectory)
    {
        foreach(Vector3 v in trajectory)
        {
            // If the position is in front of the pole
            if (v.z > pole.transform.position.z)
            {
                return v;
            }
        }
        // The code should really never return this value.
        Debug.Log("Couldnt find appropriate obstacle location");
        return pole.transform.position;
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

    // Average all the successful vectors in the throw list
    // hitsCounted: the number of successful vectors in the throw list
    private Vector3 AveragePositionInThrowList(int hitsCounted)
    {
        Vector3 result = new Vector3(0, 0, 0);
        foreach (Throw t in throwList)
        {
            if (t.hit)
            {
                result = result + t.handPos;
            }
        }
        return result / hitsCounted;
    }

    // Average all the successful vectors in the throw list
    // hitsCounted: the number of successful vectors in the throw list
    private Vector3 AverageVelocityInThrowList(int hitsCounted)
    {
        Vector3 result = new Vector3(0, 0, 0);
        foreach (Throw t in throwList)
        {
            if (t.hit)
            {
                result = result + t.velocity;
            }
        }
        return result / hitsCounted;
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

    /// <summary>
    /// Returns true if a vector is within a blocked area
    /// </summary>
    /// <param name="v"></param>The vector being analyzed
    /// <param name="list"></param>The list of blocked vectors
    /// <param name="radius"></param>The radius that determines how far the blocked vectors reach
    /// <returns></returns>
    private bool Vector3WithinBlockedArea(Vector3 v, List<Vector3> list, float radius)
    {
        foreach (Vector3 vInList in list)
        {
            if (Vector3.Distance(v, vInList) < radius)
            {
                return true;
            }
        }
        return false;
    }

}
