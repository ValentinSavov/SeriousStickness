using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleCollisionDetector : MonoBehaviour
{
    public delegate void ParticleCollisionDetectorEvent(GameObject other);
    public event ParticleCollisionDetectorEvent OnParticleCollisionLocal;

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Collision particle");
        //int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        //Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        //if(rb.GetComponent<PlayerTag>() != null)

        if(OnParticleCollisionLocal != null)
        {
            OnParticleCollisionLocal(other);
        }

    }

    void OnParticleTrigger()
    {
        Debug.Log("trigger");
    }

}
