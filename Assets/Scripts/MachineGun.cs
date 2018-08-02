using UnityEngine;
using System;
using System.Collections.Generic;

public class MachineGun : Weapon
{
    public float fireRate = 0.5f;
    float previousShotTime = 0f;
    //int shotsCounter = 0;
    AudioSource audioSource;
    ParticleSystem mgShootEffect;
    public MachineGun()
    {
        
    }

    void Start()
    {
        mgShootEffect = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        //registry.weapons.AddWeapon(this.gameObject); //AddToRegistry();
        gpParent = GameObject.Find("GeneralPurposeParent");
        //fireRate += Random.Range(-(fireRate * 0.1f), fireRate * 0.1f);
    }

    public override bool Engage(Vector3 newTarget)
    {
        bool result = false;
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            previousShotTime = Time.time;
            mgShootEffect.Play();
            float localdmg = damage;

            RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, newTarget - this.transform.position, 30f);
            Array.Sort(hits, delegate (RaycastHit2D hit1, RaycastHit2D hit2) 
            {return hit1.distance.CompareTo(hit2.distance);});
            List<DamageAcceptor> acceptors = new List<DamageAcceptor>();
            foreach (RaycastHit2D hit in hits)
            {
                DamageAcceptor acceptor = hit.collider.gameObject.GetComponent<DamageAcceptor>();

                if ( /*(hit.collider.isTrigger) ||*/ 
                    ((acceptor != null) && (acceptor == this.GetComponentInParent<DamageAcceptor>())) ||
                    (hit.collider.usedByEffector)
                    || (hit.collider.gameObject == this.gameObject))
                {
                    continue;
                }
                
                GameObject bulletHitEffect = Instantiate(Resources.Load("BulletHitEffect", typeof(GameObject)), gpParent.transform) as GameObject;
                bulletHitEffect.transform.position = new Vector3(hit.point.x, hit.point.y, -2f);
                
                if (acceptor != null)
                {
                    if (acceptors.Contains(acceptor))
                    {
                        continue;
                    }

                    acceptors.Add(acceptor);
                    registry.damageAcceptors.doTargetDamage(
                        acceptor,
                        GetComponentInParent<Tag>().gameObject,
                        (int)localdmg,
                        "normal",
                        (newTarget - this.transform.position).normalized * 500f);

                    //if ( (((Component)acceptor).gameObject.GetComponent<StickStats>() != null) || (((Component)acceptor).gameObject.GetComponent<Ragdoll>() != null))
                    {
                        //bulletHitEffect.GetComponent<ParticleSystem>().startColor = Color.red;
                        //bulletHitEffect.GetComponent<ParticleSystem>().startSize *= 2;
                    }
                }
                if(!hit.collider.isTrigger)
                {
                    localdmg *= 0.8f;
                }
                if(localdmg <= damage / 10)
                {
                    break;
                }
            }
            acceptors.Clear();
            audioSource.Play();
            result = true;
        }

        return result;
    }
}