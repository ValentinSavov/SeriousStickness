using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class InteractableObject : MonoBehaviour, DamageAcceptor
{
    [Header("Destruction")]
    public bool destructible = false;
    public bool destroyObjectOnDestruct = true;
    public float hitPoints = 20f;
    
    public delegate void DestructEvent();
    public event DestructEvent OnDestruct;

    [Tooltip("Leave it empty for no effect")]
    public GameObject effectOnDestruct;

    [Header("Explosion")]
    public bool doAreaDamageOnDestruct = false;
    public float damage = 50f;
    public float damageRadius = 5f;
    public float knockback = 10000f;
    

    //public bool smashOnCollision = false;
    //public bool destroyOnSmash = false;

    GameObject gpParent;
    Registry registry;
    Rigidbody2D rbd;

    public List<string> groups { get; set; }

    void Start ()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
        gpParent = GameObject.Find("GeneralPurposeParent");
        rbd = GetComponent<Rigidbody2D>();
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (destructible)
        {
            float locDamage = argInArgs.dmg;

            if (locDamage > 0)
            {
                if (hitPoints > locDamage)
                {
                    hitPoints -= locDamage;
                    locDamage = 0;
                }
                else
                {
                    hitPoints = 0;
                    Invoke("Destruct", 0.02f);
                    DamageProvider dp = argInArgs.source.GetComponent<DamageProvider>();
                    if (dp != null)
                    {
                        dp.ReportKill(this);
                    }
                }
            }
        }
        Push(argInArgs.knockback);
    }

    public void Push(Vector2 knockback)
    {
        rbd.AddForce(knockback, ForceMode2D.Force);
    }
    
    void Destruct()
    {
        if (OnDestruct != null)
        {
            OnDestruct();
        }
        if(doAreaDamageOnDestruct)
        {
            registry.damageAcceptors.doAreaDamage(this.gameObject, (Vector2)transform.position, damageRadius, damage, "demolition", knockback);
        }
        if (effectOnDestruct != null)
        {
            GameObject effect = Instantiate(effectOnDestruct, gpParent.transform);
            effect.transform.position = this.transform.position;
            effect.transform.localScale *= damageRadius;
            Destroy(effect, 2f);
        }
        if(destroyObjectOnDestruct)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }

    /*void OnCollisionEnter2D(Collision2D collision)
    {
        if (smashOnCollision)
        {
            if (collision.collider.GetComponent<InteractableObject>() != null)
            {
                return;
            }
            DamageAcceptor acceptor = collision.collider.GetComponent<DamageAcceptor>();
            if (acceptor != null)
            {
               // Debug.Log("relative: " + collision.relativeVelocity.magnitude + "rbd.vel: " + rbd.velocity.magnitude);
                if ((collision.relativeVelocity.magnitude > 15f) && ((rbd.velocity.magnitude > 5) || (rbd.angularVelocity > 120)))
                {
                    registry.damageAcceptors.doTargetDamage(acceptor, this.gameObject, damage, "normal", Vector2.zero, new List<string>());
                    if(destroyOnSmash)
                    {
                        Invoke("Explode", 0f);
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }*/
}