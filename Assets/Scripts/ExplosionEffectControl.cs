﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectControl : MonoBehaviour
{
    public float lifeTime = 2f;
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

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
