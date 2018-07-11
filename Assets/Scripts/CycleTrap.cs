using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleTrap : MonoBehaviour
{
    public float onTime = 1f;
    public float offTime = 1f;

    float cooldown = 0;

    GameObject damageZone;
    public GameObject preWarmEffect;
	void Start ()
    {
        damageZone = GetComponentInChildren<DamageZone>().gameObject;
        damageZone.SetActive(false);
        cooldown = offTime;
    }
    public float preWarmEffectTime = 0.2f;
	void Update ()
    {
        cooldown -= Time.deltaTime;
        if (damageZone.activeInHierarchy)
        {
            if (cooldown <= 0)
            {
                if (damageZone.activeInHierarchy)
                {
                    damageZone.SetActive(false);
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
                damageZone.SetActive(true);
                cooldown = onTime;
            }
        }

	}
}
