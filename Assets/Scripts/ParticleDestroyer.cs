using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    public bool destroyAfterFinnish = true;
    ParticleSystem particle;
	void Start ()
    {
        particle = GetComponent<ParticleSystem>();
	}
	
	void Update ()
    {
        if (destroyAfterFinnish)
        {
            if (!particle.IsAlive())
            {
                Destroy(this.gameObject);
            }
        }
	}
}
