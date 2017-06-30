using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEffect : MonoBehaviour
{
    ParticleSystem particle;
	void Start ()
    {
        particle = GetComponent<ParticleSystem>();
	}
	
	void Update ()
    {
		if(!particle.IsAlive())
        {
            Destroy(this.gameObject);
        }
	}
}
