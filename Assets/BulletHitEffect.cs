using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEffect : MonoBehaviour
{
    ParticleSystem particle;
	void Awake ()
    {
        particle = GetComponent<ParticleSystem>();
	}
	
	void Update ()
    {
		//if(!particle.IsAlive())
        {
            //Destroy(particle.gameObject);
            //this.enabled = false;
        }
	}

    public void SetColorRed()
    {
        particle.startColor = Color.red;
    }
}
