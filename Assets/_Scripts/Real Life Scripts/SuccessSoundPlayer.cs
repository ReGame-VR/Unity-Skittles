using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessSoundPlayer : MonoBehaviour {

    // The array that contains all the sound effects for the skittles game.
    // When a sound is played, pick a source from the array at random. This
    // ensures that the sound effects aren't too repetitive and dont get stale.
    public AudioSource[] successSounds;

    public void PlaySuccessSound()
    {
        AudioSource sound = successSounds[Random.Range(0, successSounds.Length)];
        sound.PlayOneShot(sound.clip);
    }
}
