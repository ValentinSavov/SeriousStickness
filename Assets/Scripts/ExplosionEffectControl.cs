using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectControl : MonoBehaviour
{
    SoundControl soundControl;
	void Start ()
    {
        soundControl = GameObject.FindObjectOfType<SoundControl>();
        if(soundControl)
        {
            soundControl.PlayExplosion();
        }
        else
        {
            GetComponentInChildren<AudioSource>().enabled = true;
        }
	}
}
