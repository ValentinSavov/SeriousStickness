using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleTrap : MonoBehaviour
{
    public float onTime = 1f;
    public float offTime = 1f;

    float cooldown = 0;
    ParticleSystem[] fires;
    GameObject damageZone;
    public GameObject preWarmEffect;
    bool firesActivated = false;
	void Start ()
    {
        damageZone = GetComponentInChildren<DamageZone>().gameObject;
        damageZone.GetComponent<Collider2D>().enabled = false;
        cooldown = offTime;
        fires = damageZone.GetComponentsInChildren<ParticleSystem>();
    }
    public float preWarmEffectTime = 0.2f;
	void Update ()
    {
        cooldown -= Time.deltaTime;
        if (firesActivated)
        {
            if (cooldown <= 0)
            {
                if (damageZone.activeInHierarchy)
                {
                    damageZone.GetComponent<Collider2D>().enabled = false;
                    DeactivateFires();
                    cooldown = offTime;
                }
            }
        }
        else
        {
            if (cooldown <= preWarmEffectTime)
            {
                if(preWarmEffect != null)
                {
                    preWarmEffect.SetActive(true);
                }
            }
            if (cooldown <= 0)
            {
                preWarmEffect.SetActive(false);
                ActivateFires();
                damageZone.GetComponent<Collider2D>().enabled = true;
                cooldown = onTime;
            }
        }

	}

    void ActivateFires()
    {
        foreach(ParticleSystem fire in fires)
        {
            fire.Play();
        }
        firesActivated = true;
    }

    void DeactivateFires()
    {
        foreach (ParticleSystem fire in fires)
        {
            fire.Stop();
        }
        firesActivated = false;
    }
}
