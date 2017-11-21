using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class InteractableObject : MonoBehaviour, DamageAcceptor
{
    public bool destroyOnHit = false;
    public bool explodeOnDestroy = true;
    public float damage = 100f;
    public float damageRadius = 5f;
    public float knockback = 10000f;

    [Tooltip("Leave it empty for no effect")]
    public GameObject effectOnDestroy;

    public bool smashOnCollision = false;
    public bool explodeOnSmash = false;

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
        if (destroyOnHit)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Push(argInArgs.knockback);
        }
    }

    public void Push(Vector2 knockback)
    {
        rbd.AddForce(knockback, ForceMode2D.Force);
    }
    
    void Explode()
    {
        if (effectOnDestroy != null)
        {
            //if (explodeOnDestroy)
            {
                registry.damageAcceptors.doAreaDamage(this.gameObject, (Vector2)transform.position, damageRadius, damage, "normal", knockback);

                GameObject effect = Instantiate(effectOnDestroy, gpParent.transform);
                effect.transform.position = this.transform.position;
                effect.transform.localScale *= damageRadius;
                Destroy(effect, 2f);
            }
        }
    }

    void OnDestroy()
    {
        if (registry != null)
        {
            if (explodeOnDestroy)  Explode();
            registry.damageAcceptors.RemoveDamageAcceptor(this);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
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
                if ((collision.relativeVelocity.magnitude > 5f) && ((rbd.velocity.magnitude > 1) || (rbd.angularVelocity > 1)))
                {
                    //Debug.Log("SmashDamage");
                    //registry.damageAcceptors.doTargetDamage(acceptor, this.gameObject, 1000f, "", Vector2.zero, new List<string>());
                    if(explodeOnSmash)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }
}