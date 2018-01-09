using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    public AudioClip explosion;

    AudioSource speaker;
	void Start ()
    {
        speaker = GetComponent<AudioSource>();
	}
	
    public void PlayExplosion()
    {
        speaker.Stop();
        //speaker.clip = explosion;
        Debug.Log("PlayOneShot");
        speaker.PlayOneShot(explosion);
    }
}
