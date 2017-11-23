using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour, DamageAcceptor
{
    GameObject gpParent;
    Registry registry;

    public List<string> groups { get; set; }

    void Start ()
    {
        gpParent = GameObject.Find("GeneralPurposeParent");
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        groups = new List<string>();
        groups.Add("level");
    }

    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        Push(argInArgs.knockback);
    }

    public void Activate()
    {
        registry.damageAcceptors.AddDamageAcceptor(this);
        Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in rbs)
        {
            if (rb.isKinematic == true)
            {
                rb.isKinematic = false;
                Collider2D col = rb.GetComponent<Collider2D>();
                if (col != null)
                {
                    if (col.isTrigger == true)
                    {
                        col.isTrigger = false;
                    }
                }
            }
        }
        transform.SetParent(gpParent.transform);
        Destroy(this.gameObject, 30f);
        /////
    }

    public void Push(Vector2 knockback)
    {
        AddForceToRandomBones(knockback);
    }

    void AddForceToRandomBones(Vector2 knockback)
    {
        Rigidbody2D[] rbs = this.GetComponentsInChildren<Rigidbody2D>();
        if (rbs != null)
        {
            int rand1 = Random.Range(0, rbs.Length);
            int rand2 = Random.Range(0, rbs.Length);
            int rand3 = Random.Range(0, rbs.Length);
            rbs[rand1].AddForce(knockback / 3);
            rbs[rand2].AddForce(knockback / 4);
            rbs[rand3].AddForce(knockback / 5);
        }
    }
}
