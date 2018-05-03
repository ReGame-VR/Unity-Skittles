using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour {

    // The arrays that contain all the sound effects for the skittles game.
    // When a sound is played, pick a source from the array at random. This
    // ensures that the sound effects aren't too repetitive and dont get stale.
    public AudioSource[] successSounds;
    public AudioSource[] failSounds;
    public AudioSource[] pickupSounds;
    public AudioSource[] throwSounds;

    public void PlaySuccessSound()
    {
        AudioSource sound = successSounds[Random.Range(0, successSounds.Length)];
        sound.PlayOneShot(sound.clip);
    }

    public void PlayFailSound()
    {
        AudioSource sound = failSounds[Random.Range(0, failSounds.Length)];
        sound.PlayOneShot(sound.clip);
    }

    public void PlayPickupSound()
    {
        AudioSource sound = pickupSounds[Random.Range(0, pickupSounds.Length)];
        sound.PlayOneShot(sound.clip);
    }

    public void PlayThrowSound()
    {
        AudioSource sound = throwSounds[Random.Range(0, throwSounds.Length)];
        sound.PlayOneShot(sound.clip);
    }
}
