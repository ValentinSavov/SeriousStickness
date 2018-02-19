using UnityEngine;
using System;
using System.Collections.Generic;

public class MeleeWeapon : Weapon
{
    public float fireRate = 0.5f;
    float previousShotTime = 0f;

    public AudioClip gasSound;
    public AudioClip stopSound;
    
    AudioSource engineAudio;
    AudioSource cutAudio;
    GameObject barrel;
    public MeleeWeapon()
    {
        
    }

    void Start()
    {
        barrel = transform.Find("Barrel").gameObject;
        engineAudio = GetComponent<AudioSource>();
        cutAudio = barrel.GetComponent<AudioSource>();
        registry = GameObject.FindObjectOfType<Registry>();
        //registry.weapons.AddWeapon(this.gameObject); //AddToRegistry();
        gpParent = GameObject.Find("GeneralPurposeParent");
    }
    float gasCooldown = 0.3f;
    private void Update()
    {
        gasCooldown -= Time.deltaTime;
        if (gasCooldown <= 0)
        {
            if (engineAudio.isPlaying && engineAudio.clip == gasSound)
            {
                engineAudio.loop = false;
                engineAudio.clip = stopSound;
                engineAudio.Play();
            }
        }
    }
    public override bool Engage(Vector3 newTarget)
    {
        bool result = false;
        if ((Time.time - previousShotTime) >= (1f / fireRate))
        {
            
            if(engineAudio.isPlaying && engineAudio.clip == gasSound)
            {
                //if (engineAudio.volume < 1)
                {
                    //engineAudio.volume = Mathf.Clamp01(engineAudio.volume + Time.deltaTime * 30)/2;
                }
            }
            else
            {
                engineAudio.clip = gasSound;
                engineAudio.loop = true;
                engineAudio.Play();
                //engineAudio.volume = 0f;
            }
            
            gasCooldown = 0.3f;
            previousShotTime = Time.time;

            Collider2D[] cols = new Collider2D[5];
            int count = GetComponent<Collider2D>().GetContacts(cols);
            if (count > 0)
            {
                cutAudio.Play();
                GameObject bulletHitEffect = Instantiate(Resources.Load("BulletHitEffect", typeof(GameObject)), gpParent.transform) as GameObject;
                bulletHitEffect.transform.position = new Vector3(barrel.transform.position.x, barrel.transform.position.y, -2f);
                
                List<DamageAcceptor> acceptors = new List<DamageAcceptor>();
                for (int i = 0; (i < count) && (i < cols.Length); i++)
                {
                    DamageAcceptor acceptor = cols[i].gameObject.GetComponent<DamageAcceptor>();

                    if ((cols[i].isTrigger) ||
                        ((acceptor != null) && (acceptor == this.GetComponentInParent<DamageAcceptor>())) ||
                        (cols[i].usedByEffector))
                    {
                        continue;
                    }


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
                            damage,
                            "normal",
                            (newTarget - this.transform.position).normalized * 2000f,
                            groups);

                        if ((((Component)acceptor).gameObject.GetComponent<StickStats>() != null) || (((Component)acceptor).gameObject.GetComponent<Ragdoll>() != null))
                        {
                            bulletHitEffect.GetComponent<ParticleSystem>().startColor = Color.red;
                            bulletHitEffect.GetComponent<ParticleSystem>().startSize *= 2;
                        }
                    }
                }
                acceptors.Clear();
            }
            result = true;
        }

        return result;
    }
}