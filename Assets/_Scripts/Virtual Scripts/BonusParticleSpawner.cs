using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawner that spawns particles when user earns bonus points or does a bad throw
public class BonusParticleSpawner : MonoBehaviour {

    [SerializeField]
    private GameObject bonusParticles;

    [SerializeField]
    private GameObject badThrowParticles;

    public void SpawnBonusParticles()
    {
        Instantiate(bonusParticles, transform.position, Quaternion.identity);
    }

    public void SpawnBadThrowParticles()
    {
        Instantiate(badThrowParticles, transform.position, Quaternion.identity);
    }


}
