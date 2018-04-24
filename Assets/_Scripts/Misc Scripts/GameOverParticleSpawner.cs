using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverParticleSpawner : MonoBehaviour
{

    // The locations at which the game over particles
    // will be spawned upon game over.
    public GameObject[] gameOverLocations;

    // The game over particles to be spawned
    [SerializeField]
    private GameObject gameOverParticles;

    public void SpawnGameOverParticles()
    {
        foreach (GameObject g in gameOverLocations)
        {
            Instantiate(gameOverParticles, g.transform.position, Quaternion.identity);
        }
    }
}

